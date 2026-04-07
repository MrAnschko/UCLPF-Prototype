using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{

    public static User instance;

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
