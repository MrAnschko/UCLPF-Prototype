using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathHandler : MonoBehaviour
{
    public static PathHandler instance;

    [SerializeField]
    private Scene PathScene;
    [SerializeField]
    private PathData pData;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError($"Tried Making Multiple Path Handler! \n Deleting most recent: {this}");
            Destroy(this);
        }
        instance = this;
        PathScene = gameObject.scene;

    }

    private void OnDestroy()
    {
        instance = null;
    }


    public void UnloadPath()
    {
        SceneManager.UnloadSceneAsync(PathScene);
        
    }
}
