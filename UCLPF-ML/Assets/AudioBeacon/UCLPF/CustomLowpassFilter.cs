using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomLowpassFilter
{
    [SerializeField] private float currentCutoffFrequency;
    [SerializeField] private float goalCutoffFrequency;
    [SerializeField] private float seekRate = 1.0f;
    [SerializeField] private float qFactor;
    [SerializeField] private float sampleRate = 48000.0F;

    public CustomLowpassFilter()
    {
        this.currentCutoffFrequency = 22050;
        this.qFactor = 0.707f;
        sampleRate = 48000;
        SetFactors(qFactor, CutoffFrequency);
    }

    public CustomLowpassFilter(float cutoffFrequency, float qFactor)
    {
        this.CutoffFrequency = cutoffFrequency;
        this.qFactor = qFactor;
        sampleRate = AudioSettings.outputSampleRate;
        SetFactors(qFactor,cutoffFrequency);
    }

    public float CutoffFrequency { 
        get => goalCutoffFrequency;
        set { 

            goalCutoffFrequency = Mathf.Min(value,sampleRate*0.95f);
        }
    }
    public float QFactor {
        get => qFactor;
        set
        {
            qFactor = value;
            SetFactors(CutoffFrequency,QFactor);
        }
    }

    public float SampleRate 
    { 
        get => sampleRate; 
        set 
        {
            sampleRate = value;
            SetFactors(CutoffFrequency, QFactor);
        }

    }


    // I use the Transposed Canonical form as suggested in "Designing Audio Effect Plugins in C++"
    private float[] a_factors = new float[3];
    private float[] b_factors = new float[2];

    // Registers
    private float[] registers = new float[2];

    private void SetFactors(float cutoffFrequency, float qFactor)
    {
        float om_c = 2 * Mathf.PI * cutoffFrequency / SampleRate;
        float beta = 0.5f * (1.0f - 1.0f / (2.0f * qFactor) * Mathf.Sin(om_c))/ (1.0f + 1.0f / (2.0f * qFactor) * Mathf.Sin(om_c));
        float gamma = (0.5f+beta)*Mathf.Cos(om_c);

        a_factors[0] = (0.5f + beta - gamma) / 2.0f;
        a_factors[1] = (0.5f + beta - gamma);
        a_factors[2] = (0.5f + beta - gamma) / 2.0f;

        b_factors[0] = -2*gamma;
        b_factors[1] = 2*beta;
    }

    public float Process(float x)
    {
        if (currentCutoffFrequency != goalCutoffFrequency)
        {
            currentCutoffFrequency = currentCutoffFrequency+Mathf.Clamp(-seekRate, goalCutoffFrequency - currentCutoffFrequency, seekRate);
            SetFactors(currentCutoffFrequency, qFactor);
        }
        float y = 0;
        y = x * a_factors[0] + registers[0];

        registers[0] = x * a_factors[1] + registers[1] - b_factors[0] * y;
        registers[1] = x * a_factors[2] - b_factors[1] * y;

        return y;
    }



}
