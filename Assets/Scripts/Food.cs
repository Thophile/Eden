using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Interactable
{
    public Mesh[] meshes = new Mesh[9];
    public GameObject foodPiece = null;
    public float foodMultiplier;
    public int maxHitPoint = 200;
    int health = 200;

    void Start(){
        var rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = UserInterface.isGamePaused;
    }
    public override void Interact(Ant ant)
    {
        base.Interact(ant);
        health -= ant.damage;
        if (health > 0){
            int index = (8* health)/maxHitPoint;
            gameObject.GetComponent<MeshFilter>().sharedMesh = meshes[index];
        }else{
            Destroy(gameObject);
        }
        if (foodPiece!= null){
            var load = Instantiate(foodPiece, ant.loadPos.position, Quaternion.identity);
            load.transform.parent = ant.loadPos;
            load.GetComponent<FoodPiece>().foodValue = (int)(ant.damage * foodMultiplier);
            ant.Load = load;
        }
    }

    void Update(){
        if (transform.position.y < -20){
            Destroy(gameObject);
        }
    }

}
