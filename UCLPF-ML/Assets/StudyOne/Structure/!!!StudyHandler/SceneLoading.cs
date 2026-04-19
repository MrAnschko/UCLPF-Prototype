using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoading
{
    private static int PathIndexStart = 2;


    public static void LoadPath(ProcessInfos.Path path)
    {
        

        if (PathHandler.instance != null) 
        {
            PathHandler.instance.UnloadPath();
        }
        SceneManager.LoadScene(PathIndexStart + (int)path,LoadSceneMode.Additive);
        return;
    }

}
