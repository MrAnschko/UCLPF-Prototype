#include "AudioPluginUtil.h"

namespace FirstFilter
{
    enum Param
    {
        P_Freq,
        P_Q,
        P_UseLogScale,
        P_ShowSpectrum,
        P_SpectrumDecay,
        P_NUM
    };

    struct EffectData
    {
        struct Data
        {
            float p[P_NUM];
            AudioPluginUtil::BiquadFilter FilterP[8];
            AudioPluginUtil::BiquadFilter DisplayFilterCoeffs[1];
            float sr;
            AudioPluginUtil::Random random;
            AudioPluginUtil::FFTAnalyzer analyzer;
        };
        union
        {
            Data data;
            unsigned char pad[(sizeof(Data) + 15) & ~15]; // This entire structure must be a multiple of 16 bytes (and and instance 16 byte aligned) for PS3 SPU DMA requirements
        };
    };

    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
        int numparams = P_NUM;
        definition.paramdefs = new UnityAudioParameterDefinition[numparams];
        AudioPluginUtil::RegisterParameter(definition, "Freq", "Hz", 0.01f, 24000.0f, 4000.0f, 1.0f, 3.0f, P_Freq, "Cutoff frequency of the Filter");
        AudioPluginUtil::RegisterParameter(definition, "Q", "", 0.01f, 10.0f, 0.707f, 1.0f, 3.0f, P_Q, "Q-factor of the lowpass filter");
        AudioPluginUtil::RegisterParameter(definition, "UseLogScale", "", 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, P_UseLogScale, "Use logarithmic scale for plotting the filter curve frequency response and input/output spectra");
        AudioPluginUtil::RegisterParameter(definition, "ShowSpectrum", "", 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, P_ShowSpectrum, "Overlay input spectrum (green) and output spectrum (red)");
        AudioPluginUtil::RegisterParameter(definition, "SpectrumDecay", "dB/s", -50.0f, 0.0f, -10.0f, 1.0f, 1.0f, P_SpectrumDecay, "Hold time for overlaid spectra");
        return numparams;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData* effectdata = new EffectData;
        memset(effectdata, 0, sizeof(EffectData));
        effectdata->data.analyzer.spectrumSize = 4096;
        AudioPluginUtil::InitParametersFromDefinitions(InternalRegisterEffectDefinition, effectdata->data.p);
        state->effectdata = effectdata;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ReleaseCallback(UnityAudioEffectState* state)
    {
        EffectData* effectdata = state->GetEffectData<EffectData>();
        EffectData::Data* data = &effectdata->data;
        data->analyzer.Cleanup();
        delete effectdata;
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
        if (value != NULL)
            *value = data->p[index];
        if (valuestr != NULL)
            valuestr[0] = 0;
        return UNITY_AUDIODSP_OK;
    }

    static void SetupFilterCoeffs(EffectData::Data* data, AudioPluginUtil::BiquadFilter* filterP,  float samplerate)
    {
        filterP->SetupLowpass(data->p[P_Freq], samplerate, data->p[P_Q]);
    }

    int UNITY_AUDIODSP_CALLBACK GetFloatBufferCallback(UnityAudioEffectState* state, const char* name, float* buffer, int numsamples)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        if (strcmp(name, "InputSpec") == 0)
            data->analyzer.ReadBuffer(buffer, numsamples, true);
        else if (strcmp(name, "OutputSpec") == 0)
            data->analyzer.ReadBuffer(buffer, numsamples, false);
        else if (strcmp(name, "Coeffs") == 0)
        {
            SetupFilterCoeffs(data, &data->DisplayFilterCoeffs[0],(float)state->samplerate);
            data->DisplayFilterCoeffs[0].StoreCoeffs(buffer);
        }
        else
            memset(buffer, 0, sizeof(float) * numsamples);

        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;

        float sr = (float)state->samplerate;
        for (int i = 0; i < inchannels; i++)
            SetupFilterCoeffs(data, &data->FilterP[i], sr);

        float specDecay = powf(10.0f, 0.05f * data->p[P_SpectrumDecay] * length / sr);
        bool calcSpectrum = (data->p[P_ShowSpectrum] >= 0.5f);
        if (calcSpectrum)
            data->analyzer.AnalyzeInput(inbuffer, inchannels, length, specDecay);

        for (unsigned int n = 0; n < length; n++)
        {
            for (int i = 0; i < outchannels; i++)
            {
                float killdenormal = (float)(data->random.Get() & 255) * 1.0e-9f;
                float y = inbuffer[n * inchannels + i] + killdenormal;
                y = data->FilterP[i].Process(y);
                outbuffer[n * outchannels + i] = y;
            }
        }

        if (calcSpectrum)
            data->analyzer.AnalyzeOutput(outbuffer, outchannels, length, specDecay);

        return UNITY_AUDIODSP_OK;
    }
}
