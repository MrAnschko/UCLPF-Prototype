using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : MonoBehaviour
{
    public static Leader instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Leader Already in scene destroying most recent one");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }
}
