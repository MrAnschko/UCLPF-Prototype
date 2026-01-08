#include "AudioPluginUtil.h"
//using namespace AudioPluginUtil;

namespace FirstSteps
{
    enum Param
    {
        P_FREQ, //Frequency parameter
        P_MIX,  //Mix parameter
        P_NUM   //An extra value to keep track of length of the enum
    };



    struct EffectData
    {
        struct Data
        {
            float p[P_NUM]; // Parameters
            float s;        // Sine output of oscillator
            float c;        // Cosine output of oscillator
        };
        union
        {
            Data data;
            unsigned char pad[(sizeof(Data) + 15) & ~15];
        };
    };

    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
            int numparams = P_NUM;
        definition.paramdefs = new  UnityAudioParameterDefinition [numparams];
        AudioPluginUtil::RegisterParameter(definition, "Frequency", "Hz",
            0.0f, AudioPluginUtil::kMaxSampleRate, 1000.0f,
            1.0f, 3.0f,
            P_FREQ);
        AudioPluginUtil::RegisterParameter(definition, "Mix amount", "%",
            0.0f, 1.0f, 0.5f,
            100.0f, 1.0f,
            P_MIX);
        return numparams;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData* effectdata = new EffectData;
        memset(effectdata, 0, sizeof(EffectData));
        effectdata->data.c = 1.0f;
        state->effectdata = effectdata;
        AudioPluginUtil::InitParametersFromDefinitions(
            InternalRegisterEffectDefinition, effectdata->data.p);
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ReleaseCallback(UnityAudioEffectState* state)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        delete data;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;

        float w = 2.0f * sinf(AudioPluginUtil::kPI * data->p[P_FREQ] / state->samplerate);
        for (unsigned int n = 0; n < length; n++)
        {
            for (int i = 0; i < outchannels; i++)
            {
                outbuffer[n * outchannels + i] =
                    inbuffer[n * outchannels + i] *
                    (1.0f - data->p[P_MIX] + data->p[P_MIX] * data->s);
            }
            data->s += data->c * w; // cheap way to calculate a sine-wave
            data->c -= data->s * w;
        }

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

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK GetFloatParameterCallback(UnityAudioEffectState* state, int index, float* value, char* valuestr)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
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
}
