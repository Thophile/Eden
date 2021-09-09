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
    public string prefabName;
    public float downForce;
    public float climbDist;
    public float pheroDetectionRange;
    public float pheroThreshold;
    public Transform rightSensor;
    public Transform leftSensor;
    public float activationRadius;
    public int damage;
    public float speed;
    public int turnAngle;
    public float lerpSpeed;

    public Rigidbody rb;
    public LayerMask obstacleLayer;
    public LayerMask antLayer;
    public AntState state;
    
    float dirUpdateTimer = 0f;
    public float dirUpdateDelay;

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
    public Transform Target {
        get { return _target; }
        set { 
            if (value != null){
                _target = value;
                state = AntState.Targeting;
            }
        }
    }
    Transform _target = null;
    Vector3 surfaceNormal = Vector3.up;
    Vector3 normal;
    Vector3 oldPheroPos;

    public bool isGrounded;


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

            // Surface Adaptation
            surfaceNormal = GetTargetSurfaceNormal();            
            
            // Obstacle check
            float dChange = AvoidObstacles();

            // Ant direction Changes
            dirUpdateTimer += Time.fixedDeltaTime;
            if(dChange == 0f){
                switch (state){
                    case AntState.Wandering :
                        // Sensing phero
                        if(dirUpdateTimer > dirUpdateDelay){
                            dirUpdateTimer = 0;
                            dChange = GetPheroDir(surfaceNormal);
                        }
                        break;

                    case AntState.Targeting :
                        if(CheckTarget()){
                            MarkPath();
                            if(dirUpdateTimer > dirUpdateDelay){
                                dirUpdateTimer = 0;
                                dChange = GetTargetDir(surfaceNormal);
                            }
                        }
                        break;
                }
            }
            
            

            //Ant down force
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast (transform.position, -transform.up, out hit, 0.8f, ~antLayer)) {
                isGrounded=true;
                // Ant Rotation
                var surfaceRight = Vector3.ProjectOnPlane(transform.right,surfaceNormal);
                var surfaceForward = Vector3.Cross(surfaceRight, surfaceNormal);
                Quaternion targetRot = Quaternion.LookRotation(surfaceForward, surfaceNormal);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed*Time.deltaTime);
                transform.Rotate(0, dChange, 0);
                rb.AddForce(-transform.up * downForce);
            }else{
                isGrounded=false;
                var globalForward = Vector3.ProjectOnPlane(transform.forward,Vector3.up);
                Quaternion targetRot = Quaternion.LookRotation(globalForward, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed*Time.deltaTime);
                rb.AddForce(-Vector3.up * downForce);
            }

            // Ant Forward velocity
            rb.velocity = state == AntState.Still ? Vector3.zero : transform.forward * speed * Time.fixedDeltaTime;
        }else{
            rb.velocity = Vector3.zero;
        }
        
    }
    Vector3 GetTargetSurfaceNormal(){
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast (transform.position - transform.up*0.1f, Quaternion.AngleAxis(-5,transform.up)*transform.forward, out hit, climbDist, ~antLayer) || Physics.Raycast (transform.position - transform.up*0.06f, Quaternion.AngleAxis(5,transform.up)*transform.forward, out hit, climbDist, ~antLayer)) {
            return hit.normal;
        }else if(Physics.Raycast (transform.position - transform.up*0.06f+ transform.forward*0.03f, Quaternion.Euler(15, 0, 0) * -transform.up, out hit, Mathf.Infinity,~antLayer)){
            return hit.normal;
        }else{
            return surfaceNormal;
        }
    }

    // Try interacting with target and go back to wandering if target is destroyed
    bool CheckTarget(){
        if(Target == null){
            state = AntState.Wandering;
            return false;
        }else if(Vector3.Magnitude(Target.position - transform.position) < activationRadius){
            Target.gameObject.GetComponent<Interactable>().Interact(this);
        }
        return true;
    }

    // Getting phero path direction
    float GetPheroDir(Vector3 surfaceNormal){
        var rightStrenght = GameState.current.pheromonesMap.getPheromonesValue(rightSensor.position,pheroDetectionRange);
        var leftStrenght = GameState.current.pheromonesMap.getPheromonesValue(leftSensor.position,pheroDetectionRange);
        if((rightStrenght - leftStrenght) != 0){
            var exitDir = Vector3.ProjectOnPlane(GameObject.Find("Colony").GetComponent<Colony>().exit.position - transform.position, surfaceNormal);
            var dirAngle =  turnAngle*((rightStrenght - leftStrenght) / Mathf.Abs (rightStrenght - leftStrenght));
            return dirAngle + Random.Range(- turnAngle/3,turnAngle/3);
        }else{
            return Random.Range(- turnAngle/2,turnAngle/2);
        }
    }

    // Getting target path direction
    float GetTargetDir(Vector3 surfaceNormal){
        var dir = Vector3.ProjectOnPlane(Target.position - transform.position, surfaceNormal);
        if(Vector3.Angle(surfaceNormal, dir) < 45){
            return 0f;
        }
        var dirAngle = Vector3.SignedAngle(transform.forward, dir, surfaceNormal);
        return dirAngle + Random.Range(- turnAngle/3,turnAngle/3);
    }

    // Spitting phero to indicates route every 1 meter
    void MarkPath(){
        
        var dist = oldPheroPos == null ? 1f : (transform.position - oldPheroPos).magnitude;
        if(dist >= 1f){
            oldPheroPos = transform.position;
            GameState.current.pheromonesMap.Mark(transform.position);
        }
    }

    // Return a rotation to avoid end of map
    float AvoidObstacles(){
        // Obstacle Avoidance
        var front = new Vector3(transform.forward.x,0,transform.forward.z);
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast (transform.position, front, out hit, 5f, obstacleLayer)){
            var right = Quaternion.Euler(0, turnAngle, 0) * front;
            var left = Quaternion.Euler(0, -turnAngle, 0) * front;
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
            return turnAngle * Mathf.Sign(rightDist - leftDist) ;
        }else {
            return 0f;
        }
    }

     private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Food>() && Target == null){
            Target = other.transform;
        }
    }
}
