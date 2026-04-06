using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSaver : MonoBehaviour
{
    public StudyInfo studyInfo;

    void Start()
    {
        studyInfo.Save();   
    }

}
