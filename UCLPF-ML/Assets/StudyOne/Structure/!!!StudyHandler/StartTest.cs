using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTest : MonoBehaviour
{
    const int GloabalSceneIndex = 1;

    [ContextMenu("Start")]
    public void StartStudy() {
        SceneManager.LoadScene(GloabalSceneIndex);
    }
}
