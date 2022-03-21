using Assets.Scripts.Managers;
using Assets.Scripts.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class AntSpawner : MonoBehaviour
    {
        public Transform exit;
        public GameObject ant;
        public static Transform gameManager;
    
        public int spawnDelay;
        float time = 0;

        private void Start()
        {
            gameManager = GameObject.Find("GameManager").transform;
        }

        // Update is called once per frame
        void Update()
        {
            if(!GameManager.isPaused){
                time += Time.deltaTime;
                if (time > spawnDelay){
                    time-=spawnDelay;
                    if (GameManager.antInstances.Count < GameManager.gameState.antNb){
                        var rand = Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up);
                        ObjectManager.Spawn(
                            new MonoBehaviourData(
                            "Ant",
                            new Proxies.TransformProxy(exit.position, Quaternion.LookRotation(rand, Vector3.up))
                            )
                        );
                    }
                }
            }
        }
    }
}
