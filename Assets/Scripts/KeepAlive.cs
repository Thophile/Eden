using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class KeepAlive : MonoBehaviour
{
    public string baseSceneName;
    void Awake()
    {
        baseSceneName = SceneManager.GetActiveScene().name;
        DontDestroyOnLoad(gameObject);
    }
}
