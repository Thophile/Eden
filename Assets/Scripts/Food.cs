using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Interactable
{
    public Mesh[] meshes = new Mesh[9];
    public GameObject foodPiece = null;
    public float foodMultiplier;
    public int maxHealth = 200;
    public int health = 200;

    public string prefabName;

    void Start(){
        var rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = UserInterface.isGamePaused;
        int index = (8* health)/maxHealth;
        gameObject.GetComponent<MeshFilter>().sharedMesh = meshes[index];
    }
    public override void Interact(Ant ant)
    {
        base.Interact(ant);
        health -= ant.damage;
        if (health > 0){
            int index = (8* health)/maxHealth;
            gameObject.GetComponent<MeshFilter>().sharedMesh = meshes[index];
        }else{
            FoodSpawner.foodsInfo.Remove(gameObject);
            Destroy(gameObject);

        }
        if (foodPiece!= null){
            var load = Instantiate(foodPiece, ant.loadPos.position, Quaternion.identity);
            load.transform.parent = ant.loadPos;
            load.GetComponent<Carryable>().value = (int)(ant.damage * foodMultiplier);
            load.GetComponent<Carryable>().type = CarryableType.Food;
            ant.Load = load;
        }
    }

    void Update(){
        if (transform.position.y < -20){
            FoodSpawner.foodsInfo.Remove(gameObject);
            Destroy(gameObject);
        }
    }

}
