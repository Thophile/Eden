using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colony : MonoBehaviour
{
    public Transform exit;
    public GameObject ant;
    public int spawnDelay;
    float time = 0;
    public List<GameObject> antsOut = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if(!UserInterface.isGamePaused){
            time += Time.deltaTime;
            if (time > spawnDelay){
                time-=spawnDelay;
                if ((10*antsOut.Count)/GameState.current.antNb <= 8){
                    var dir = Random.Range(-90,90);
                    SpawnAnt(exit.position, Quaternion.Euler(0, dir, 0) * exit.rotation);
                }
            }
        }
    }
    public void SpawnAnt(Vector3 pos, Quaternion rot){
        var obj = Instantiate(ant, pos, rot);
        antsOut.Add(obj);
            
        obj.transform.parent = gameObject.transform;
    }
    public void DespawnAnt(GameObject ant){
        antsOut.Remove(ant);
        Destroy(ant);
    }
}
