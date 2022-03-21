using Assets.Scripts.Managers;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class AntProfiler : MonoBehaviour
    {

        public Transform exit;
        public GameObject ant;
        public Transform gameManager;

        public int spawnDelay;
        float time = 0;
        public static List<GameObject> antsInfo = new List<GameObject>();

        private void Start()
        {
            gameManager = GameObject.Find("GameManager").transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.isPaused)
            {
                time += Time.deltaTime;
                if (time > spawnDelay)
                {
                    time -= spawnDelay;
                    for (int i = 0; i < 10; i++)
                    {
                        ObjectManager.Spawn(
                            new MonoBehaviourData(
                            "Ant",
                            new Proxies.TransformProxy(transform.position, Quaternion.AngleAxis(i * 36, Vector3.up))
                            )
                        );
                    }
                }
            }
        }




    }
}