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
                    
                    SpawnAnt(exit.position, exit.rotation, ant);
                }
            }
        }
    }
    public static void SpawnAnt(Vector3 pos, Quaternion rot, GameObject ant, GameObject load = null){
        var obj = Instantiate(ant, pos, rot);
        obj.transform.parent = GameObject.Find("WorldManager").transform;
        obj.GetComponent<Ant>().prefabName = ant.name;
        if(load != null) {
            obj.GetComponent<Ant>().Load = Instantiate(load, obj.GetComponent<Ant>().loadPos.position, obj.GetComponent<Ant>().loadPos.rotation);
            obj.GetComponent<Ant>().Load.transform.parent = obj.transform;
        }

        antsInfo.Add(obj);
    }
    public void DespawnAnt(GameObject ant){
        antsInfo.Remove(ant);
        Destroy(ant);
    }
}
