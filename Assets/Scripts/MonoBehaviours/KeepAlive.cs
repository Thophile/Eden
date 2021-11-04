using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MonoBehaviours
{
    public class KeepAlive : MonoBehaviour
    {
        public string baseSceneName;
        void Awake()
        {
            baseSceneName = SceneManager.GetActiveScene().name;
            DontDestroyOnLoad(gameObject);
        }
    }
}
