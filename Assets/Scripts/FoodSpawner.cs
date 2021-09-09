using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject Food;
    public float spawnRadius;
    public int spawnDelay;

    public static List<GameObject> foodsInfo = new List<GameObject>();

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
                    SpawnFood(pos, Quaternion.identity, Food);
                }
                timer -= spawnDelay;
            }
        }
    }

    public static void SpawnFood(Vector3 pos, Quaternion rot, GameObject food, int health = 0){
        var obj = Instantiate(food, pos, rot);
        obj.transform.parent = GameObject.Find("WorldManager").transform;
        obj.GetComponent<Food>().prefabName = food.name;
        if (health != 0){
            obj.GetComponent<Food>().health = health;
        }
        foodsInfo.Add(obj);
    }

}
