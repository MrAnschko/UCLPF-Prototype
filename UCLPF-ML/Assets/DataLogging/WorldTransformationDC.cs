using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class WorldTransformationDC : DataContainer
{
    [SerializeField] Matrix4x4 UnityToCustom;
    [SerializeField] Matrix4x4 CustomToUnity;

    public void Save()
    {
        if (CustomWorldOrigin.instance == null)
            return;
        CustomToUnity = CustomWorldOrigin.instance.transform.localToWorldMatrix;
        UnityToCustom = CustomWorldOrigin.instance.transform.worldToLocalMatrix;
        Save(ProcessInfos.UserID + "_Transformation");
    }
}
