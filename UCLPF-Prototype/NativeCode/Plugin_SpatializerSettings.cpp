// Please note that this will only work on Unity 5.2 or higher.

#include "AudioPluginUtil.h"

extern float settingbuffer[];
extern float debugbuffer[]; // Buffer for debug Purposes. Saved in buffer Callback

namespace SpatializerSettings
{
    enum
    {
        P_TotalMix,
        P_QFactor, 
        P_CutoffInitFeq,
        P_HalfAngle,
        P_SeekSpeed,
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
        AudioPluginUtil::RegisterParameter(definition, "Q Factor", "", 0.001f, 20.0f, 0.707f, 1.0f, 1.0f, P_QFactor, "The Quality Factor of the lowpass filter.");
        AudioPluginUtil::RegisterParameter(definition, "Freq", "Hz", 0.01f, 24000.0f, 22000.0f, 1.0f, 3.0f, P_CutoffInitFeq, "Cutoff frequency of the Filter");
        AudioPluginUtil::RegisterParameter(definition, "Half Angle", "Degree", 0.01f, AudioPluginUtil::kPI, 0.5f, 180 / AudioPluginUtil::kPI, 1.0f, P_HalfAngle, "Angle at which the cutoff frequency is halved");
        AudioPluginUtil::RegisterParameter(definition, "SeekSpeed", "", 0.01f, AudioPluginUtil::kMaxSampleRate, 1.0f, 1.0f, 10.0f, P_SeekSpeed, "How much the cutoff frequency may maximally be increased by per sample");
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

        for(int param=0; param<P_NUM;param++)
            settingbuffer[param] = data->p[param]; // copy settings into the buffer
        return UNITY_AUDIODSP_OK;
    }
}
