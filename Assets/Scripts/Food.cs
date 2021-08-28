using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public Mesh[] meshes = new Mesh[9];
    public GameObject foodPiece = null;
    public int maxHitPoint = 200;
    public int health = 200;

    public GameObject Pick(int damage){
        health -= damage;
        if (health > 0){
            int index = (8* health)/maxHitPoint;
            gameObject.GetComponent<MeshFilter>().sharedMesh = meshes[index];
        }else{
            Destroy(gameObject);
        }
        return foodPiece;

    }

    void Update(){
        if (transform.position.y < -20){
            Destroy(gameObject);
        }
    }

}
