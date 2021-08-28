using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject Food;
    public float spawnRadius;
    public int spawnDelay;

    float timer = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if(!UserInterface.isGamePaused){
            timer += Time.deltaTime;
            if(timer > spawnDelay){
                Spawn();
                timer -= spawnDelay;
            }
        }
    }

    void Spawn(){
        var coord = Random.insideUnitCircle * spawnRadius;
        var pos = transform.position + new Vector3(coord.x, 0, coord.y);

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast (pos, -Vector3.up, out hit)) {
            var foo = Instantiate(Food, pos, Quaternion.identity);
        }
    }
}
