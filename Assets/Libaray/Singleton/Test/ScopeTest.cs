using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScopeTest : MonoBehaviour
{
    public bool loadScene;
    public bool loadAdditive;
    private void Start()
    {
        var singleton = Singleton
            .GetSingleton<ScopeSingleton>()
            .GetScopeSingleton<TestScopeSingleton>()
            ;

        Debug.Log(singleton.number);
        
        if (loadScene)
        {
            if (loadAdditive)
            {
                SceneManager.LoadScene("ScopeTest_Sub2", LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.LoadScene("ScopeTest_Sub1");
            }
        }
    }
}
