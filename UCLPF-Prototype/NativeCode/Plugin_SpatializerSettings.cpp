// Please note that this will only work on Unity 5.2 or higher.
// Learning Plugin that sends a sine wave based on the position of an object to another plugin

#include "AudioPluginUtil.h"

extern float settingbuffer[];
extern float debugbuffer[]; // Buffer for debug Purposes. Saved in buffer Callback

namespace SpatializerSettings
{
    enum
    {
        P_TotalMix,
        P_NUM
    };

    struct EffectData
    {
        float p[P_NUM];
    };


    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
        int numparams = P_NUM;
        definition.paramdefs = new UnityAudioParameterDefinition[numparams];
        AudioPluginUtil::RegisterParameter(definition, "Total Mix", "%", 0.0f, 1.0f, 0.0f, 100.0f, 1.0f, P_TotalMix, "How much of the Method should be mixed in");
        
        return numparams;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData* effectdata = new EffectData;
        memset(effectdata, 0, sizeof(EffectData));
        state->effectdata = effectdata;

        AudioPluginUtil::InitParametersFromDefinitions(InternalRegisterEffectDefinition, effectdata->p);
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
        EffectData* data = state->GetEffectData<EffectData>();
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        data->p[index] = value;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK GetFloatParameterCallback(UnityAudioEffectState* state, int index, float* value, char *valuestr)
    {
        EffectData* data = state->GetEffectData<EffectData>();
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
        if (strncmp(name, "SourcePos", 9) == 0) {
            if (numsamples != 16)
                return UNITY_AUDIODSP_ERR_UNSUPPORTED;
            memcpy(buffer, debugbuffer, sizeof(float) * 16);
        }
        return UNITY_AUDIODSP_OK;
    }



    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {

        memcpy(outbuffer, inbuffer, length * outchannels * sizeof(float));

        EffectData* data = state->GetEffectData<EffectData>();

        settingbuffer[P_TotalMix] = data->p[P_TotalMix];
        return UNITY_AUDIODSP_OK;
    }
}
