using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class Listener : MonoBehaviour
{
    public static Listener mainListener;
    
    // Start is called before the first frame update
    void Start()
    {
        if(mainListener != null)
        {
            Debug.LogError("Tried to initialise two Listeners");
            return;
        }

        mainListener = this;
    }

    
}
