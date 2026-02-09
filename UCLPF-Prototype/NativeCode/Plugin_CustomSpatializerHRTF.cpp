// Please note that this will only work on Unity 5.2 or higher.

#include "AudioPluginUtil.h"


float hrtf_settingbuffer[10] = { 0.0f }; // Make a buffer to save all the settings in.
float hrtf_debugbuffer[16] = { 0.0f }; // Buffer for debug Purposes Currently transmits a Object matrix
extern float hrtfSrcData[]; // Data for the HRTF. (Impulse responses for different elevations and azimuth degrees

namespace CustomSpatializerHRTF
{
    
    
    enum
    {
        P_AUDIOSRCATTN,
        P_FIXEDVOLUME,
        P_CUSTOMFALLOFF,
        P_NUM
    };

    
    //***************************************************************************************************************************************
    const int HRTFLEN = 512;

    const float GAINCORRECTION = 2.0f;

    class HRTFData
    {
        struct CircleCoeffs
        {
            int numangles;
            float* hrtf;
            float* angles;

            void GetHRTF(AudioPluginUtil::UnityComplexNumber* h, float angle, float mix)
            {
                int index1 = 0;
                while (index1 < numangles && angles[index1] < angle)
                    index1++;
                if (index1 > 0)
                    index1--;
                int index2 = (index1 + 1) % numangles;
                float* hrtf1 = hrtf + HRTFLEN * 4 * index1;
                float* hrtf2 = hrtf + HRTFLEN * 4 * index2;
                float f = (angle - angles[index1]) / (angles[index2] - angles[index1]);
                for (int n = 0; n < HRTFLEN * 2; n++)
                {
                    h[n].re += (hrtf1[0] + (hrtf2[0] - hrtf1[0]) * f - h[n].re) * mix;
                    h[n].im += (hrtf1[1] + (hrtf2[1] - hrtf1[1]) * f - h[n].im) * mix;
                    hrtf1 += 2;
                    hrtf2 += 2;
                }
            }
        };

    public:
        CircleCoeffs hrtfChannel[2][14];

    public:
        HRTFData()
        {
            float* p = hrtfSrcData;
            for (int c = 0; c < 2; c++)
            {
                for (int e = 0; e < 14; e++)
                {
                    CircleCoeffs& coeffs = hrtfChannel[c][e];
                    coeffs.numangles = (int)(*p++);
                    coeffs.angles = p;
                    p += coeffs.numangles;
                    coeffs.hrtf = new float[coeffs.numangles * HRTFLEN * 4];
                    float* dst = coeffs.hrtf;
                    AudioPluginUtil::UnityComplexNumber h[HRTFLEN * 2];
                    for (int a = 0; a < coeffs.numangles; a++)
                    {
                        memset(h, 0, sizeof(h));
                        for (int n = 0; n < HRTFLEN; n++)
                            h[n + HRTFLEN].re = p[n];
                        p += HRTFLEN;
                        AudioPluginUtil::FFT::Forward(h, HRTFLEN * 2, false);
                        for (int n = 0; n < HRTFLEN * 2; n++)
                        {
                            *dst++ = h[n].re;
                            *dst++ = h[n].im;
                        }
                    }
                }
            }
        }
    };

    static HRTFData sharedData;

    struct InstanceChannel
    {
        AudioPluginUtil::UnityComplexNumber h[HRTFLEN * 2];
        AudioPluginUtil::UnityComplexNumber h_prev[HRTFLEN * 2];
        AudioPluginUtil::UnityComplexNumber x[HRTFLEN * 2];
        AudioPluginUtil::UnityComplexNumber y[HRTFLEN * 2];
        float buffer[HRTFLEN * 2];
    };


    static void GetHRTF(int channel, AudioPluginUtil::UnityComplexNumber* h, float azimuth, float elevation)
    {
        float e = AudioPluginUtil::FastClip(elevation * 0.1f + 4, 0, 12);
        float f = floorf(e);
        int index1 = (int)f;
        if (index1 < 0)
            index1 = 0;
        else if (index1 > 12)
            index1 = 12;
        int index2 = index1 + 1;
        if (index2 > 12)
            index2 = 12;
        sharedData.hrtfChannel[channel][index1].GetHRTF(h, azimuth, 1.0f);
        sharedData.hrtfChannel[channel][index2].GetHRTF(h, azimuth, e - f);
    }

//*********************************************************************************************************

    struct EffectData
    {
        struct Data
        {
            float p[P_NUM];
            AudioPluginUtil::BiquadFilter lowpassFilter[2]; // The lowpass filter that's being controlled by head movements
            float cutoff_frequency_current; // In order to avoid audio artifacts (clicks) it is better when the cutoff frequency is not quickly changed
            InstanceChannel ch[2];

        };

        union
        {
            Data data;
            unsigned char pad[(sizeof(Data) + 15) & ~15]; // This entire structure must be a multiple of 16 bytes (and and instance 16 byte aligned) for PS3 SPU DMA requirements
        };
    };

    static inline float crossfade_function(float t) {
        return AudioPluginUtil::FastClip(t, 0.0f, 1.0f);; // TODO: write better function.
    }

    inline bool IsHostCompatible(UnityAudioEffectState* state)
    {
        // Somewhat convoluted error checking here because hostapiversion is only supported from SDK version 1.03 (i.e. Unity 5.2) and onwards.
        // Since we are only checking for version 0x010300 here, we can't use newer fields in the UnityAudioSpatializerData struct, such as minDistance and maxDistance.
        return
            state->structsize >= sizeof(UnityAudioEffectState) &&
            state->hostapiversion >= 0x010300;
    }

    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
        int numparams = P_NUM;
        definition.paramdefs = new UnityAudioParameterDefinition[numparams];
        AudioPluginUtil::RegisterParameter(definition, "AudioSrc Attn", "", 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, P_AUDIOSRCATTN, "AudioSource distance attenuation");
        AudioPluginUtil::RegisterParameter(definition, "Fixed Volume", "", 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, P_FIXEDVOLUME, "Fixed volume amount");
        AudioPluginUtil::RegisterParameter(definition, "Custom Falloff", "", 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, P_CUSTOMFALLOFF, "Custom volume falloff amount (logarithmic)");
        definition.flags |= UnityAudioEffectDefinitionFlags_IsSpatializer;
        return numparams;
    }

    static UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK DistanceAttenuationCallback(UnityAudioEffectState* state, float distanceIn, float attenuationIn, float* attenuationOut)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        *attenuationOut =
            data->p[P_AUDIOSRCATTN] * attenuationIn +
            data->p[P_FIXEDVOLUME] +
            data->p[P_CUSTOMFALLOFF] * (1.0f / AudioPluginUtil::FastMax(1.0f, distanceIn));
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData* effectdata = new EffectData;
        memset(effectdata, 0, sizeof(EffectData));
        state->effectdata = effectdata;
        if (IsHostCompatible(state))
            state->spatializerdata->distanceattenuationcallback = DistanceAttenuationCallback;
        AudioPluginUtil::InitParametersFromDefinitions(InternalRegisterEffectDefinition, effectdata->data.p);
        effectdata->data.cutoff_frequency_current = 0.0f;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ReleaseCallback(UnityAudioEffectState* state)
    {   
        EffectData* data = state->GetEffectData<EffectData>();
        delete data;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK SetFloatParameterCallback(UnityAudioEffectState* state, int index, float value)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        data->p[index] = value;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK GetFloatParameterCallback(UnityAudioEffectState* state, int index, float* value, char *valuestr)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        if (value != NULL)
            *value = data->p[index];
        if (valuestr != NULL)
            valuestr[0] = 0;
        return UNITY_AUDIODSP_OK;
    }

    int UNITY_AUDIODSP_CALLBACK GetFloatBufferCallback(UnityAudioEffectState* state, const char* name, float* buffer, int numsamples)
    {
        return UNITY_AUDIODSP_OK;
    }

    
    // Function for processing with HRTF
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessHRTF(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;

        //check whether the settings are already populated.
        if (hrtf_settingbuffer[0] == 0.0f) {
            memcpy(outbuffer, inbuffer, length * outchannels * sizeof(float));
            return UNITY_AUDIODSP_OK;

        }

        // Check that I/O formats are right and that the host API supports this feature
        if (inchannels != 2 || outchannels != 2 ||
            !IsHostCompatible(state) || state->spatializerdata == NULL)
        {
            memcpy(outbuffer, inbuffer, length * outchannels * sizeof(float));
            return UNITY_AUDIODSP_OK;
        }

        // Get Settings from the other Plugin
        const float totalMix =      hrtf_settingbuffer[1];
        float q_factor =            hrtf_settingbuffer[3]; // the quality factor of the filter
        float cutoff_initial_freq = hrtf_settingbuffer[4];; // the highest cutoff frequency
        float half_angle =          hrtf_settingbuffer[5]; // the angle at which the cutoff frequency reaches half its highest value
        float seek_speed =          hrtf_settingbuffer[6]; // how fast the switch from previous filter to current filter goes in block_samples/seek_speed
        float pdist_factor =        hrtf_settingbuffer[7]; // Factor by which point distance is scaled
        float cdist_factor =        hrtf_settingbuffer[8]; // Factor by which circle distance is scaled 
        float crossfade_samples =   hrtf_settingbuffer[9]; // percentage of num samples at which the signal should be fully crossfaded to the new impulse 

        float sr = (float)state->samplerate;

        float* m = state->spatializerdata->listenermatrix;
        float* s = state->spatializerdata->sourcematrix;

        //memcpy(debugbuffer, m, 16 * sizeof(float)); // Copy the Listenermatrix to the debug buffer. TODO: make thread safe. (?)

        //**** Copied from spatializer example 
        // Currently we ignore source orientation and only use the position
        float px = s[12];
        float py = s[13];
        float pz = s[14];

        float dir_x = m[0] * px + m[4] * py + m[8] * pz + m[12];
        float dir_y = m[1] * px + m[5] * py + m[9] * pz + m[13];
        float dir_z = m[2] * px + m[6] * py + m[10] * pz + m[14];
        //******************************************************

        // Distance Calculation
        //// Based on Angle
        // forward vector is (0,0,1)
        // dot product of fwd vector and sourcedir accordingly is simply the z direction
        float angle = fabsf(acosf(dir_z/sqrtf(dir_x * dir_x + dir_y * dir_y + dir_z * dir_z + 0.001f))); //angle is given in Radians
        
        float cutoff_scale_factor = 1 / half_angle; // a scale factor for how much the distance affects the frequency

        
        ///// Angle end

        //// Based on Point on plane
         

        //Position (of the listener) Creating the last column of the inverse of m. (-> a matrix that transforms from listener to world coordinates.)
        float l_x = -(m[12] * m[0] + m[13] * m[1] + m[14] * m[2]);
        float l_y = -(m[12] * m[4] + m[13] * m[5] + m[14] * m[6]);
        float l_z = -(m[12] * m[8] + m[13] * m[9] + m[14] * m[10]);


        //direction of view 
        // a forward view vector is (0,0,1,0) in listener coordinates
        // (last one is zero to avoid translation) to transform that forward vector from listener to world the inverse of m is used.
        // the inverse of the upper left 3x3 block of m (homogenous matrix) is simply its transpose
        // accordingly multiplication results in the 3rd column vector of m.
        float d_x = m[2];
        float d_y = m[6];
        float d_z = m[10];
        // Intersection
        float alpha = (d_y < -0.001f) ? (-1 - l_y) / d_y : 0; // set the alpha to zero in case there is no (positive) intersection

        // position on the plane

        float g_x =  alpha * d_x + l_x;
        float g_z = alpha * d_z + l_z;
        hrtf_debugbuffer[2] = g_x;
        hrtf_debugbuffer[3] = g_z;

        // distance from point: (on plane)
        float p_dist = sqrtf((g_x - px) * (g_x - px) + (g_z - pz) * (g_z - pz));

        //// Point on plane end

        // Circle:
        // distance of object to listener (along plane)
        float l_dist = sqrtf((l_x - px) * (l_x - px) + (l_z - pz) * (l_z - pz));
        // distance between gaze point and listener
        float l_p_dist = sqrtf((g_x - l_x) * (g_x - l_x) + (g_z - l_z) * (g_z - l_z));

        float c_dist = fabsf(l_dist - l_p_dist);
        hrtf_debugbuffer[0] = c_dist;
        // End Circle

        float goal_cutoff_frequency = cutoff_initial_freq * (1.0f / (1.0f + c_dist*cdist_factor+p_dist*pdist_factor+cutoff_scale_factor * angle));
        hrtf_debugbuffer[1] = goal_cutoff_frequency;

        ////
        // --------
        
        
        
        //****************************************************** HRTF PROCESSING (Taken from example Spatializer)
        static const float kRad2Deg = 180.0f / AudioPluginUtil::kPI;

        float azimuth = (fabsf(dir_z) < 0.001f) ? 0.0f : atan2f(dir_x, dir_z);
        if (azimuth < 0.0f)
            azimuth += 2.0f * AudioPluginUtil::kPI;
        azimuth = AudioPluginUtil::FastClip(azimuth * kRad2Deg, 0.0f, 360.0f);


        float elevation = atan2f(dir_y, sqrtf(dir_x * dir_x + dir_z * dir_z) + 0.001f) * kRad2Deg;
        float spatialblend = state->spatializerdata->spatialblend;
        float reverbmix = state->spatializerdata->reverbzonemix;

        // From the FMOD documentation:
        //   A spread angle of 0 makes the stereo sound mono at the point of the 3D emitter.
        //   A spread angle of 90 makes the left part of the stereo sound place itself at 45 degrees to the left and the right part 45 degrees to the right.
        //   A spread angle of 180 makes the left part of the stero sound place itself at 90 degrees to the left and the right part 90 degrees to the right.
        //   A spread angle of 360 makes the stereo sound mono at the opposite speaker location to where the 3D emitter should be located (by moving the left part 180 degrees left and the right part 180 degrees right). So in this case, behind you when the sound should be in front of you!
        // Note that FMOD performs the spreading and panning in one go. We can't do this here due to the way that impulse-based spatialization works, so we perform the spread calculations on the left/right source signals before they enter the convolution processing.
        // That way we can still use it to control how the source signal downmixing takes place.
        float spread = cosf(state->spatializerdata->spread * AudioPluginUtil::kPI / 360.0f);
        float spreadmatrix[2] = { 2.0f - spread, spread };


        GetHRTF(0, data->ch[0].h, azimuth, elevation);
        GetHRTF(1, data->ch[1].h, azimuth, elevation);


        

        for (unsigned int sampleOffset = 0; sampleOffset < length; sampleOffset += HRTFLEN)
        {
            float cutoff_frequency_current;
            
            for (int c = 0; c < 2; c++)
            {
                /// UCLPF. Frequency seeking is a bit different here (compared to plugin withoug hrtf), since the audio channels aren't in the inner loop. (need to 'reset' at the start of new channel)
                cutoff_frequency_current = data->cutoff_frequency_current;
                ///

                
                // stereopan is in the [-1; 1] range, this acts the way fmod does it for stereo
                float stereopan = 1.0f - ((c == 0) ? AudioPluginUtil::FastMax(0.0f, state->spatializerdata->stereopan) : AudioPluginUtil::FastMax(0.0f, -state->spatializerdata->stereopan));

                InstanceChannel& ch = data->ch[c];

                for (int n = 0; n < HRTFLEN; n++)
                {
                    float left = inbuffer[n * 2];
                    float right = inbuffer[n * 2 + 1];
                    ch.buffer[n] = ch.buffer[n + HRTFLEN]; // save previous results and shift them "Back" by one hrtflen (this seems to be a overlap-save convolution already?
                    // UCLPF PROCESSING ------------------------------------------------------------
                    cutoff_frequency_current += AudioPluginUtil::FastClip(goal_cutoff_frequency - cutoff_frequency_current, -seek_speed, seek_speed);
                    data->lowpassFilter[c].SetupLowpass(cutoff_frequency_current, sr, q_factor);
                    float y = left * spreadmatrix[c] + right * spreadmatrix[1 - c];
                    y = totalMix*data->lowpassFilter[c].Process(y)+(1-totalMix)*y;
                    ch.buffer[n + HRTFLEN] = y;
                    /// ----------------------------------------------------------------------------------
                }

                for (int n = 0; n < HRTFLEN * 2; n++)
                {
                    ch.x[n].re = ch.buffer[n];
                    ch.x[n].im = 0.0f;
                }

                AudioPluginUtil::FFT::Forward(ch.x, HRTFLEN * 2, false);

                for (int n = 0; n < HRTFLEN * 2; n++)
                    AudioPluginUtil::UnityComplexNumber::Mul<float, float, float>(ch.x[n], ch.h[n], ch.y[n]);

                AudioPluginUtil::FFT::Backward(ch.y, HRTFLEN * 2, false);

                for (int n = 0; n < HRTFLEN; n++)
                {
                    float s = inbuffer[n * 2 + c] * stereopan;
                    float y = s + (ch.y[n].re * GAINCORRECTION - s) * spatialblend;
                    outbuffer[n * 2 + c] = crossfade_function((n+ sampleOffset)/ crossfade_samples)*y;
                    
                }

                //calculate factor from previous hrtf
                for (int n = 0; n < HRTFLEN * 2; n++)
                    AudioPluginUtil::UnityComplexNumber::Mul<float, float, float>(ch.x[n], ch.h_prev[n], ch.y[n]);

                AudioPluginUtil::FFT::Backward(ch.y, HRTFLEN * 2, false);

                for (int n = 0; n < HRTFLEN; n++)
                {
                    float s = inbuffer[n * 2 + c] * stereopan;
                    float y = s + (ch.y[n].re * GAINCORRECTION - s) * spatialblend;
                    outbuffer[n * 2 + c] += crossfade_function(1-(n + sampleOffset) / crossfade_samples) * y;

                }


                

            }


            data->cutoff_frequency_current = cutoff_frequency_current; // save current position when through with both channels
            inbuffer += HRTFLEN * 2;
            outbuffer += HRTFLEN * 2;
        }


        //*********************************************************************************************************

        memcpy(data->ch[0].h_prev, data->ch[0].h, sizeof(AudioPluginUtil::UnityComplexNumber) * HRTFLEN * 2);
        memcpy(data->ch[1].h_prev, data->ch[1].h, sizeof(AudioPluginUtil::UnityComplexNumber) * HRTFLEN * 2);
        

        //processing done

        return UNITY_AUDIODSP_OK;
    }


    // Function for processing without HRTF
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessNatural(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;

        //check whether the settings are already populated.
        if (hrtf_settingbuffer[0] == 0.0f) {
            memcpy(outbuffer, inbuffer, length * outchannels * sizeof(float));
            return UNITY_AUDIODSP_OK;

        }

        // Check that I/O formats are right and that the host API supports this feature
        if (inchannels != 2 || outchannels != 2 ||
            !IsHostCompatible(state) || state->spatializerdata == NULL)
        {
            memcpy(outbuffer, inbuffer, length * outchannels * sizeof(float));
            return UNITY_AUDIODSP_OK;
        }

        // Get Settings from the other Plugin
        const float totalMix = hrtf_settingbuffer[1];
        float q_factor = hrtf_settingbuffer[3]; // the quality factor of the filter
        float cutoff_initial_freq = hrtf_settingbuffer[4];; // the highest cutoff frequency
        float half_angle = hrtf_settingbuffer[5]; // the angle at which the cutoff frequency reaches half its highest value
        float seek_speed = hrtf_settingbuffer[6]; // how fast the switch from previous filter to current filter goes in block_samples/seek_speed
        float pdist_factor = hrtf_settingbuffer[7]; // Factor by which point distance is scaled
        float cdist_factor = hrtf_settingbuffer[8]; // Factor by which circle distance is scaled 
        float crossfade_samples = hrtf_settingbuffer[9]; // percentage of num samples at which the signal should be fully crossfaded to the new impulse 

        float sr = (float)state->samplerate;

        float* m = state->spatializerdata->listenermatrix;
        float* s = state->spatializerdata->sourcematrix;
        //memcpy(debugbuffer, m, 16 * sizeof(float)); // Copy the Listenermatrix to the debug buffer. TODO: make thread safe. (?)

        //**** Copied from spatializer example 
        // Currently we ignore source orientation and only use the position
        float px = s[12];
        float py = s[13];
        float pz = s[14];

        float dir_x = m[0] * px + m[4] * py + m[8] * pz + m[12];
        float dir_y = m[1] * px + m[5] * py + m[9] * pz + m[13];
        float dir_z = m[2] * px + m[6] * py + m[10] * pz + m[14];
        //******************************************************

        // Distance Calculation
        //// Based on Angle
        // forward vector is (0,0,1)
        // dot product of fwd vector and sourcedir accordingly is simply the z direction
        float dist = sqrtf(dir_x * dir_x + dir_y * dir_y + dir_z * dir_z + 0.001f);
        float angle = fabsf(acosf(dir_z / dist)); //angle is given in Radians

        float cutoff_scale_factor = 1 / half_angle; // a scale factor for how much the distance affects the frequency


        ///// Angle end

        //// Based on Point on plane


        //Position (of the listener) Creating the last column of the inverse of m. (-> a matrix that transforms from listener to world coordinates.)
        float l_x = -(m[12] * m[0] + m[13] * m[1] + m[14] * m[2]);
        float l_y = -(m[12] * m[4] + m[13] * m[5] + m[14] * m[6]);
        float l_z = -(m[12] * m[8] + m[13] * m[9] + m[14] * m[10]);


        //direction of view 
        // a forward view vector is (0,0,1,0) in listener coordinates
        // (last one is zero to avoid translation) to transform that forward vector from listener to world the inverse of m is used.
        // the inverse of the upper left 3x3 block of m (homogenous matrix) is simply its transpose
        // accordingly multiplication results in the 3rd column vector of m.
        float d_x = m[2];
        float d_y = m[6];
        float d_z = m[10];
        // Intersection
        float alpha = (d_y < -0.001f) ? (-1 - l_y) / d_y : 0; // set the alpha to zero in case there is no (positive) intersection

        // position on the plane

        float g_x = alpha * d_x + l_x;
        float g_z = alpha * d_z + l_z;
        
        // distance from point: (on plane)
        float p_dist = sqrtf((g_x - px) * (g_x - px) + (g_z - pz) * (g_z - pz));

        //// Point on plane end

        // Circle:
        // distance of object to listener (along plane)
        float l_dist = sqrtf((l_x - px) * (l_x - px) + (l_z - pz) * (l_z - pz));
        // distance between gaze point and listener
        float l_p_dist = sqrtf((g_x - l_x) * (g_x - l_x) + (g_z - l_z) * (g_z - l_z));

        float c_dist = fabsf(l_dist - l_p_dist);
        // End Circle

        float goal_cutoff_frequency = cutoff_initial_freq * (1.0f / (1.0f + c_dist * cdist_factor + p_dist * pdist_factor + cutoff_scale_factor * angle));
        
        ////
        // --------

        // Stereo panning according to sine-cosine panning law
        float spread = cosf(state->spatializerdata->spread * AudioPluginUtil::kPI / 360.0f);
        float spreadmatrix[2] = { 2.0f - spread, spread };
        float azimuth = (fabsf(dir_z) < 0.001f) ? 0.0f : atan2f(dir_x, dir_z);

        
        //


        float cutoff_frequency_current = data->cutoff_frequency_current;


        for (unsigned int n = 0; n < length; n++)
        {
            cutoff_frequency_current += AudioPluginUtil::FastClip(goal_cutoff_frequency - cutoff_frequency_current, -seek_speed, seek_speed); // slowly go toward the goal frequency to avoid artifacts
            for (int c = 0; c < 2; c++)
            {
                float stereopan = 1.0f - ((c == 0) ? AudioPluginUtil::FastMax(0.0f, state->spatializerdata->stereopan) : AudioPluginUtil::FastMax(0.0f, -state->spatializerdata->stereopan));

                // processing with the lowpass filters
                data->lowpassFilter[c].SetupLowpass(cutoff_frequency_current, sr, q_factor);
                float left = inbuffer[n * 2];
                float right = inbuffer[n * 2 + 1];
                
                float spatial = left * spreadmatrix[c] + right * spreadmatrix[1 - c];
                outbuffer[n * 2 + c] = (y * totalMix + (1 - totalMix) * spatial);
            }
        }

        data->cutoff_frequency_current = cutoff_frequency_current;
        //processing done

        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels) 
    {
        const float enable_hrtf = hrtf_settingbuffer[2]; // Wheter the HRTF is enabled.
        if (enable_hrtf > 0.5f) {
            return ProcessHRTF(state, inbuffer, outbuffer, length, inchannels, outchannels);
        }
        return ProcessNatural(state, inbuffer, outbuffer, length, inchannels, outchannels);
    }
}
