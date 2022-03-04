using Assets.Scripts.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class AntSpawner : MonoBehaviour
    {
        public Transform exit;
        public GameObject ant;
    
        public int spawnDelay;
        float time = 0;
        public static List<GameObject> antsInfo = new List<GameObject>();

        // Update is called once per frame
        void Update()
        {
            if(!GameManager.isPaused){
                time += Time.deltaTime;
                if (time > spawnDelay){
                    time-=spawnDelay;
                    if (GameManager.activeAnts.Count < GameManager.gameState.antNb){
                        var rand = Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up);
                        SpawnAnt(exit.position, Quaternion.LookRotation(rand, Vector3.up), ant);
                    }
                }
            }
        }
        public static void SpawnAnt(Vector3 pos, Quaternion rot, GameObject ant, GameObject load = null, List<TimedPosition> previousPositions = null){
            var obj = Instantiate(ant, pos, rot);
            obj.transform.parent = GameObject.Find("GameManager").transform;
            var antComponent = obj.GetComponent<Ant>();
            antComponent.prefabName = ant.name;
            if(load != null) {
                antComponent.Load = Instantiate(load, antComponent.loadPos.position, antComponent.loadPos.rotation);
                antComponent.Load.transform.parent = obj.transform;
            }
            if(previousPositions != null)
            {
                antComponent.previousPositions = previousPositions;
            }

            antsInfo.Add(obj);
            GameManager.activeAnts.Add(antComponent);
        }
        public void DespawnAnt(GameObject ant){
            antsInfo.Remove(ant);
            GameManager.activeAnts.Remove(ant.GetComponent<Ant>());
            Destroy(ant);
        }
    }
}
