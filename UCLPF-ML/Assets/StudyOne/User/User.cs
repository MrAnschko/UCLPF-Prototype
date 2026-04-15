using MagicLeap.Android;
using MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    // Handles User Interaction
    public static User instance;
    public DirectionalIndicator DirIndicator;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError($"Tried making Two Users \n Deleting {this.gameObject}");
            Destroy(this);
        }
        instance = this;
        
        
    }

    private void OnDestroy()
    {   
        if(instance == this)
            instance = null;
    }
}
