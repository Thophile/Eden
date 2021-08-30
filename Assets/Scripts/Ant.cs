using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AntState {
        Wandering,
        Targeting,
        Still,
    }
public class Ant : MonoBehaviour
{
    // Parameters
    public float downForce;
    public float climbDist;
    public int detectionRadius;
    public float activationRadius;
    public int damage;
    public float speed;
    public float reactionTime;
    public int turnAngle;
    public float lerpSpeed;

    public Rigidbody rb;
    public LayerMask obstacleLayer;
    public AntState state;
    

    public GameObject Load{
        get {return _load;}
        set{
            if(value != null){
                Target(GameObject.Find("Colony").GetComponent<Colony>().exit);
                _load = value;
            }
        }
    }
    public GameObject _load = null;
    public Transform loadPos;
    public float maxLifeTime;
    float lifeTime;
    float wanderingTimer = 0;
    Transform target = null;
    Vector3 surfaceNormal;
    Vector3 normal;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(7,7);
    }

    void FixedUpdate()
    {
        if(!UserInterface.isGamePaused){

            // Go home after lifetime
            lifeTime += Time.deltaTime;
            if(lifeTime>maxLifeTime){
                Target(GameObject.Find("Colony").GetComponent<Colony>().exit);
            }

            // Surface Adaptation
            normal = transform.up;
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast (transform.position + (transform.up * 0.15f), transform.forward, out hit, climbDist)) {
                surfaceNormal = hit.normal;
            }else if(Physics.Raycast (transform.position + transform.up*0.15f + transform.forward*0.1f, Quaternion.Euler(15, 0, 0) * -transform.up, out hit, Mathf.Infinity)){
                surfaceNormal = hit.normal;
            }
            Quaternion targetRot = Quaternion.LookRotation(Vector3.Cross(transform.right,surfaceNormal), surfaceNormal);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed*Time.deltaTime);


            // Ant direction Changes
            float dChange = 0;
            switch (state){
                case AntState.Wandering :
                    wanderingTimer += Time.deltaTime;
                    if(wanderingTimer > reactionTime){
                        wanderingTimer -= reactionTime;
                        dChange = Random.Range(-turnAngle,turnAngle);
                    } 
                    break;
                case AntState.Targeting :
                    var dir = Vector3.ProjectOnPlane(new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z), normal);

                    Debug.DrawRay(transform.position, dir, Color.blue);
                    Debug.DrawRay(transform.position, transform.forward * 5f, Color.red);
                    dChange = Vector3.SignedAngle(transform.forward, dir, normal);
                    break;
            }
            transform.Rotate(0, AvoidObstacles(dChange), 0);
            
            // Ant Forward velocity
            rb.velocity = state == AntState.Still ? Vector3.zero : transform.forward * speed * Time.fixedDeltaTime;
            rb.AddForce(-normal * downForce);
        }else{
            rb.velocity = Vector3.zero;
        }
        
    }

    // Return a rotation to avoid end of map
    float AvoidObstacles(float dChange){
        // Obstacle Avoidance
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast (transform.position, transform.forward, out hit, 5f, obstacleLayer)){
            var right = Quaternion.Euler(0, turnAngle, 0) * transform.forward;
            var left = Quaternion.Euler(0, -turnAngle, 0) * transform.forward;
            float leftDist = 0f;
            float rightDist = 0f;
            RaycastHit leftHit = new RaycastHit();
            if(Physics.Raycast (transform.position, left, out leftHit, Mathf.Infinity, obstacleLayer)){
                leftDist = leftHit.distance;
            }
            RaycastHit rightHit = new RaycastHit();
            if(Physics.Raycast (transform.position, right, out rightHit, Mathf.Infinity, obstacleLayer)){
                rightDist = rightHit.distance;
            }
            //Sign of right-left gives direction of rotation
            dChange = turnAngle * (rightDist - leftDist) / Mathf.Abs(rightDist - leftDist) ;
        }
        return dChange;
    }

    void Target(Transform point){
        state = AntState.Targeting;
        target = point;
        Debug.Log(Vector3.Magnitude(transform.position - point.position));
        if(Vector3.Magnitude(transform.position - point.position) < activationRadius){
            target.gameObject.GetComponent<Interactable>().Interact(this);
        } 
    }

     private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Food>() && target == null){
            Target(other.transform);
        }
    }
}
