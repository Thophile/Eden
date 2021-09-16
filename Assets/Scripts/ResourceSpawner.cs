using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public GameObject resource;
    public float spawnRadius;
    public int spawnDelay;

    public static List<GameObject> resourceInfo = new List<GameObject>();

    float timer = 0.0f;

    void Update()
    {
        if(!UserInterface.isGamePaused){
            timer += Time.deltaTime;
            if(timer > spawnDelay){
                var coord = Random.insideUnitCircle * spawnRadius;
                var pos = transform.position + new Vector3(coord.x, 0, coord.y);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast (pos, -Vector3.up, out hit)) {
                    SpawnResource(pos, Quaternion.identity, resource);
                }
                timer -= spawnDelay;
            }
        }
    }

    public static void SpawnResource(Vector3 pos, Quaternion rot, GameObject resource, int health = 0){
        var obj = Instantiate(resource, pos, rot);
        obj.transform.parent = GameObject.Find("WorldManager").transform;
        obj.GetComponent<Resource>().prefabName = resource.name;
        if (health != 0){
            obj.GetComponent<Resource>().health = health;
        }
        resourceInfo.Add(obj);
    }

}
