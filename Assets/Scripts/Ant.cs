using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AntState {
        Wandering,
        GoingHome,
        Still,
    }
public class Ant : MonoBehaviour
{
    // Parameters
    public string prefabName;
    public int pheroDetectionRange;

    // Stats
    public float activationRadius;
    public float downForce;
    public float climbDist;
    public int damage;
    public int turnAngle;
    public LayerMask obstacleLayer;
    public LayerMask antLayer;

    public Rigidbody rb;
    public AntState state;
    

    public GameObject Load{
        get {return _load;}
        set{
            if(value != null){
                state = AntState.GoingHome;
                _load = value;
            }
        }
    }
    GameObject _load = null;
    public Transform loadPos;
    public List<GameObject> Targets = new List<GameObject>();
    Vector3 surfaceNormal = Vector3.up;
    Vector3 oldPheroPos;
    public bool isGrounded;

    // Serialized
    public float maxVelocity = 2f;
    public float steerStrength = 2f;
    public float wanderStrenght = 0.5f;

    Vector3 desiredDirection;
    void FixedUpdate(){
        if(!UserInterface.isGamePaused){
            // Try interacting with target
            var target = PickTarget();
            TryInteract(target);

            // Surface Adaptation
            surfaceNormal = GetTargetSurfaceNormal();

            Vector3 directionChange = AvoidObstacles();
            directionChange += directionChange == Vector3.zero ? Random.insideUnitSphere * wanderStrenght + GetDir() : Vector3.zero;
            desiredDirection = (desiredDirection + directionChange).normalized;
            Vector3 desiredVelocity = desiredDirection * maxVelocity;
            Vector3 desiredSteeringForce = (desiredVelocity - rb.velocity) * steerStrength;
            Vector3 acceleration = Vector3.ClampMagnitude(desiredSteeringForce, steerStrength);

            rb.velocity = Vector3.ClampMagnitude(rb.velocity + acceleration * Time.fixedDeltaTime, maxVelocity);
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(desiredDirection, surfaceNormal),surfaceNormal);


            //Ant down force
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast (transform.position, -transform.up, out hit, 0.8f, ~antLayer)) {
                isGrounded=true;
                rb.AddForce(-surfaceNormal * downForce);
            }else{
                isGrounded=false;
                rb.AddForce(-Vector3.up * downForce);
            }
        }
    }

    void Start()
    {
        desiredDirection = transform.forward;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = UserInterface.isGamePaused;
        Physics.IgnoreLayerCollision(7,7);
    }

    /*void FixedUpdate()
    {
        if(!UserInterface.isGamePaused){
            // try interacting with preferred potential target
            var target = PickTarget();
            TryInteract(target);

            // Surface Adaptation
            surfaceNormal = GetTargetSurfaceNormal();            
            
            // Obstacle check
            float dChange = AvoidObstacles();

            // Ant direction Changes
            dirUpdateTimer += Time.fixedDeltaTime;
            if(dChange == 0f){
                switch (state){
                    case AntState.Wandering :
                        if(dirUpdateTimer > dirUpdateDelay){
                            dirUpdateTimer = 0;
                            dChange = GetPheroDir(surfaceNormal);
                        }
                        break;

                    case AntState.GoingHome :
                        if(dirUpdateTimer > dirUpdateDelay){
                            dirUpdateTimer = 0;
                            dChange = GetPheroDir(surfaceNormal);
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
        
    }*/
    GameObject PickTarget(){
        float? minDist = null;
        GameObject closest = null;
        foreach (var item in Targets)
        {
            if(minDist == null || (transform.position - item.transform.position).magnitude < minDist ){
                minDist = (transform.position - item.transform.position).magnitude;
                closest = item;
            }
        }
        return closest;
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

    // Try interacting with target
    void TryInteract(GameObject target){
        
        if(target!= null && Vector3.Magnitude(target.transform.position - transform.position) < activationRadius){
            target.GetComponent<Interactable>().Interact(this);
        }
    }

    Vector3 GetDir(){
        if(Targets.Count > 0){
            var target = PickTarget();
             return (target.transform.position - transform.position).normalized;
        }else{
            const float magnitude = 2f;
            Vector3 center = transform.forward * magnitude;
            Vector3 left = Quaternion.AngleAxis(-15,transform.up)*transform.forward* magnitude;
            Vector3 right = Quaternion.AngleAxis(15,transform.up)*transform.forward* magnitude;

            var centerPhero = GameState.current.pheromonesMap.ComputeZone(transform.position+center, pheroDetectionRange);
            var leftPhero = GameState.current.pheromonesMap.ComputeZone(transform.position+left, pheroDetectionRange);
            var rightPhero = GameState.current.pheromonesMap.ComputeZone(transform.position+right, pheroDetectionRange);

            float centerValue = 0f, leftValue = 0f, rightValue = 0f;

            switch(state){
                case AntState.Wandering:
                    centerValue = centerPhero.x-centerPhero.z;
                    leftValue = leftPhero.x-leftPhero.z;
                    rightValue = rightPhero.x-rightPhero.z;
                    break;
                case AntState.GoingHome:
                    centerValue = centerPhero.y;
                    leftValue = leftPhero.y;
                    rightValue = rightPhero.y;
                    break;

            }

            if(centerValue >= leftValue && centerValue >= rightValue) return center.normalized;
            else if(leftValue >= centerValue && leftValue >= rightValue) return left.normalized;
            else return right.normalized;
        }

        



    }

    // Spitting phero to indicates route every 1 meter
    void MarkPath(){
        MarkerType type = MarkerType.Wander;
        if(state == AntState.GoingHome) type = MarkerType.Resource;

        var dist = oldPheroPos == null ? 1f : (transform.position - oldPheroPos).magnitude;
        if(dist >= 1f){
            oldPheroPos = transform.position;
            GameState.current.pheromonesMap.Mark(transform.position, type);
        }
    }

    // Return a rotation to avoid end of map
    Vector3 AvoidObstacles(){
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
            if(rightDist > leftDist) return right;
            else return left;
        }else {
            return Vector3.zero;
        }
    }

     private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Interactable>()){
            Targets.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other){
        if(other.gameObject.GetComponent<Interactable>()){
            Targets.Remove(other.gameObject);
        }
    }
}
