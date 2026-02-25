using UnityEngine;

// The code example shows how to implement a metronome that procedurally
// generates the click sounds via the OnAudioFilterRead callback.
// While the game is paused or suspended, this time will not be updated and sounds
// playing will be paused. Therefore developers of music scheduling routines do not have
// to do any rescheduling after the app is unpaused

[RequireComponent(typeof(AudioSource))]
public class CustomProcessing : MonoBehaviour
{

    private float gain = 0.5F;
    private float freq = 22050.0f;
    private float q = 0.707f;

    bool running = false;
    CustomLowpassFilter[] clpf = new CustomLowpassFilter[2];

    public float Freq 
    { 
        get => freq;
        set
        {
            freq = value;
            clpf[0].CutoffFrequency = freq;
            clpf[1].CutoffFrequency = freq;
        } 
    }
    public float Q 
    {
        get => q;
        set 
        {
            q = value;
            clpf[0].QFactor = q;
            clpf[1].QFactor = q;
        }
    }

    void Start()
    {
        running = true;
        clpf[0] = new CustomLowpassFilter(freq,q);
        clpf[1] = new CustomLowpassFilter(freq,q);


    }

    void OnAudioFilterRead(float[] data, int channels)
    {

        float attenuation = gain;
        if (!running)
            return;
        for (int i = 0; i < data.Length; )
        {
            
            for (int c = 0; c < channels; c++)
            {
                //Debug.Log($"Data at {i}: {data[i]}");
                data[i] = clpf[c].Process(data[i]);
                
                i++;

            }
            
        }
    }
}