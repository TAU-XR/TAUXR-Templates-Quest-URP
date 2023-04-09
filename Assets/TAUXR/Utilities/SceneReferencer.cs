using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores references for everything needer to refer to in the scene.
public class SceneReferencer : MonoBehaviour
{
    #region Singelton Decleration
    private static SceneReferencer _instance;

    public static SceneReferencer Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion


}
