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
    public float activationRadius;
    public int damage;
    public float speed;
    public int turnAngle;
    public float lerpSpeed;

    public Rigidbody rb;
    public LayerMask obstacleLayer;
    public LayerMask antLayer;
    public AntState state;
    

    public GameObject Load{
        get {return _load;}
        set{
            if(value != null){
                Target = GameObject.Find("Colony").GetComponent<Colony>().exit;
                _load = value;
            }
        }
    }
    GameObject _load = null;
    public Transform loadPos;
    public float maxLifeTime;
    float lifeTime;
    public Transform Target {
        get { return _target; }
        set { 
            if (value != null){
                _target = value;
                //state = AntState.Targeting;
            }
        }
    }
    Transform _target = null;
    Vector3 surfaceNormal = Vector3.up;
    Vector3 normal;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = UserInterface.isGamePaused;
        Physics.IgnoreLayerCollision(7,7);
    }

    void FixedUpdate()
    {
        if(!UserInterface.isGamePaused){

            // Go home after lifetime
            lifeTime += Time.deltaTime;
            if(lifeTime>maxLifeTime){
                Target = GameObject.Find("Colony").GetComponent<Colony>().exit;
            }

            // Surface Adaptation            
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast (transform.position + transform.up * 0.15f, transform.forward, out hit, climbDist, ~antLayer)) {
                surfaceNormal = hit.normal;
            }else if(Physics.Raycast (transform.position + transform.up*0.15f + transform.forward*0.1f, Quaternion.Euler(15, 0, 0) * -transform.up, out hit, Mathf.Infinity,~antLayer)){
                surfaceNormal = hit.normal;
            }
            // Ant direction Changes
            float dChange = 0;
            switch (state){
                case AntState.Wandering :
                        dChange = Random.Range(-turnAngle,turnAngle);
                    break;
                case AntState.Targeting :
                    // Try interacting with target
                    if(Target != null && Vector3.Magnitude(Target.position - transform.position) < activationRadius){
                        Target.gameObject.GetComponent<Interactable>().Interact(this);
                    }
                    if (Target == null) Debug.Log(Target);

                    var dir = Vector3.ProjectOnPlane(Target.position - transform.position, transform.up);
                    dChange = Vector3.SignedAngle(transform.forward, dir, transform.up);
                    break;
            }
            dChange = AvoidObstacles(dChange);

            // Ant Rotation
            var surfaceRight = Vector3.ProjectOnPlane(transform.right,surfaceNormal);
            var surfaceForward = Vector3.Cross(surfaceRight, surfaceNormal);
            var surfaceNewDir = Quaternion.AngleAxis(dChange, surfaceNormal) * surfaceForward;
            Quaternion targetRot = Quaternion.LookRotation(surfaceNewDir, surfaceNormal);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed*Time.deltaTime);
            // Ant Forward velocity
            rb.velocity = state == AntState.Still ? Vector3.zero : transform.forward * speed * Time.fixedDeltaTime;
            rb.AddForce(-transform.up * downForce);
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

     private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Food>() && Target == null){
            Target = other.transform;
        }
    }
}
