using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Resource : Interactable
{
    public GameObject resourcePiece = null;
    public ResourceValue baseValues;
    public int totalHealth = 200;
    public int health = 200;

    MeshFilter meshFilter;
    public string prefabName;

    void Start(){
        var rb = gameObject.GetComponent<Rigidbody>();
        if(rb != null){
            rb.isKinematic = UserInterface.isGamePaused;
        }
    }
    public override void Interact(Ant ant)
    {
        base.Interact(ant);
        health -= ant.damage;
        if (health <= 0){
            ResourceSpawner.resourceInfo.Remove(gameObject);
            Destroy(gameObject);
        }
        if (resourcePiece!= null){
            var load = Instantiate(resourcePiece, ant.loadPos.position, Quaternion.identity);
            load.transform.parent = ant.loadPos;
            load.GetComponent<Carryable>().resourceValue = ant.damage * baseValues;
            ant.Load = load;
        }
    }

    void Update(){
        if (transform.position.y < -20){
            ResourceSpawner.resourceInfo.Remove(gameObject);
            Destroy(gameObject);
        }
    }

}
