using Assets.Scripts.Model;
using Assets.Scripts.Ui;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class Resource : Interactable
    {
        public GameObject resourcePiece = null;
        public ResourceProperties baseValues;
        public int totalHealth = 200;
        public int health = 200;

        MeshFilter meshFilter;
        public string prefabName;

        void Start(){
            var rb = gameObject.GetComponent<Rigidbody>();
            if(rb != null){
                rb.isKinematic = GameManager.isPaused;
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
                var shipement = Instantiate(resourcePiece, ant.loadPos.position, Quaternion.identity);
                shipement.transform.parent = ant.loadPos;
                shipement.GetComponent<Carryable>().resourceValue = ant.damage * baseValues;
                ant.Shipement = shipement;
            }
        }

        void Update(){
            if (transform.position.y < -20){
                ResourceSpawner.resourceInfo.Remove(gameObject);
                Destroy(gameObject);
            }
        }

    }
}
