using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colony : MonoBehaviour
{
    public Transform exit;
    public GameObject ant;
    
    public int spawnDelay;
    float time = 0;
    public static List<GameObject> antsInfo = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if(!UserInterface.isGamePaused){
            time += Time.deltaTime;
            if (time > spawnDelay){
                time-=spawnDelay;
                if ((10*antsInfo.Count)/GameState.current.antNb <= 8){
                    var rand = Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up);
                    SpawnAnt(exit.position, Quaternion.LookRotation(rand, Vector3.up), ant);
                }
            }
        }
    }
    public static void SpawnAnt(Vector3 pos, Quaternion rot, GameObject ant, GameObject load = null){
        var obj = Instantiate(ant, pos, rot);
        obj.transform.parent = GameObject.Find("WorldManager").transform;
        var antComponent = obj.GetComponent<Ant>();
        antComponent.prefabName = ant.name;
        if(load != null) {
            antComponent.Load = Instantiate(load, antComponent.loadPos.position, antComponent.loadPos.rotation);
            antComponent.Load.transform.parent = obj.transform;
        }

        antsInfo.Add(obj);
        WorldManager.activeAnts.Add(antComponent);
    }
    public void DespawnAnt(GameObject ant){
        antsInfo.Remove(ant);
        WorldManager.activeAnts.Remove(ant.GetComponent<Ant>());
        Destroy(ant);
    }
}