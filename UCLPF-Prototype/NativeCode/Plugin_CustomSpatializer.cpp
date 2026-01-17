// Please note that this will only work on Unity 5.2 or higher.

#include "AudioPluginUtil.h"

float settingbuffer[4] = { 0 };
float debugbuffer[16] = { 0 }; // Buffer for debug Purposes Currently transmits a Object matrix

namespace CustomSpatializer
{
    const float q_factor = 0.707f; // the quality factor of the filter
    const float cutoff_initial_freq = 2200; // the highest cutoff frequency
    const float half_angle = AudioPluginUtil::kPI/16; // the angle at which the cutoff frequency reaches half its highest value
    const float cutoff_scale_factor = 1/half_angle; // a scale factor for how much the distance affects the frequency
    const float seek_speed = 1.0f; // how fast the switch from previous filter to current filter goes in block_samples/seek_speed

    enum
    {
        P_AUDIOSRCATTN,
        P_FIXEDVOLUME,
        P_CUSTOMFALLOFF,
        P_NUM
    };

    const float GAINCORRECTION = 2.0f;

    

    struct EffectData
    {
        struct Data
        {
            float p[P_NUM];
            AudioPluginUtil::BiquadFilter lowpassFilter[2]; // The lowpass filter that's being controlled by head movements
            float cutoff_frequency_current; // In order to avoid audio artifacts (clicks) it is better when the cutoff frequency is not quickly changed
        };

        union
        {
            Data data;
            unsigned char pad[(sizeof(Data) + 15) & ~15]; // This entire structure must be a multiple of 16 bytes (and and instance 16 byte aligned) for PS3 SPU DMA requirements
        };
    };

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

    

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;

        // Check that I/O formats are right and that the host API supports this feature
        if (inchannels != 2 || outchannels != 2 ||
            !IsHostCompatible(state) || state->spatializerdata == NULL)
        {
            memcpy(outbuffer, inbuffer, length * outchannels * sizeof(float));
            return UNITY_AUDIODSP_OK;
        }
        float sr = (float)state->samplerate;

        float* m = state->spatializerdata->listenermatrix;
        float* s = state->spatializerdata->sourcematrix;

        memcpy(debugbuffer, m, 16 * sizeof(float)); // Copy the Listenermatrix. TODO: make thread safe. (?)

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
        ///// Angle end

        //// Based on Point on plane

        float goal_cutoff_frequency = cutoff_initial_freq * (1.0f / (1.0f + cutoff_scale_factor * angle));

        ////
        // --------

        

        float cutoff_frequency_current = data->cutoff_frequency_current;
        
        const float totalMix = settingbuffer[0];

        
        for (unsigned int n = 0; n < length; n++)
        {
            cutoff_frequency_current += AudioPluginUtil::FastClip(goal_cutoff_frequency - cutoff_frequency_current ,-seek_speed, seek_speed); // slowly go toward the goal frequency to avoid artifacts
            for (int i = 0; i < outchannels; i++)
            {
                // processing with the lowpass filters
                data->lowpassFilter[i].SetupLowpass(cutoff_frequency_current, sr, q_factor);
                float y = inbuffer[n * inchannels + i];
                y = data->lowpassFilter[i].Process(y);
                outbuffer[n * outchannels + i] = y*totalMix+(1-totalMix)*inbuffer[n * inchannels + i];
            }
        }

        data->cutoff_frequency_current = cutoff_frequency_current;
        //processing done

        return UNITY_AUDIODSP_OK;
    }
}
