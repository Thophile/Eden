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
    public LayerMask obstacleLayer;
    public LayerMask antLayer;

    //[System.NonSerialized]
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

    // Serialized
    public float maxVelocity = 20f;
    public float steerStrength = 1f;
    public float wanderStrenght = 0.5f;
    public float updateDelay = 1f;

    Vector3 desiredDirection;
    float updateCounter = 0;
    Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        desiredDirection = transform.forward;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = UserInterface.isGamePaused;
        Physics.IgnoreLayerCollision(7,7);
    }
    void Update(){
        if(!UserInterface.isGamePaused){

            // Surface Adaptation
            surfaceNormal = GetTargetSurfaceNormal();

            desiredDirection += AvoidObstacles();

            updateCounter+= Time.deltaTime;
            if (updateCounter >= updateDelay){
                updateCounter -= updateDelay;
                MarkPath();

                Vector2 random = Random.insideUnitCircle;
                Vector3 randomDir = Vector3.ProjectOnPlane(new Vector3(random.x, 0, random.y), surfaceNormal);
                
                Vector3 targetDir = GetDir(surfaceNormal).normalized;

                desiredDirection = (randomDir * wanderStrenght + targetDir).normalized;             

            }

            Vector3 desiredVelocity = desiredDirection * maxVelocity;
            Vector3 acceleration = Vector3.ClampMagnitude(Vector3.ProjectOnPlane(desiredVelocity - rb.velocity, surfaceNormal) * steerStrength, steerStrength);

            rb.velocity = Vector3.ClampMagnitude(rb.velocity + acceleration, maxVelocity);
            Quaternion targetRot =  Quaternion.LookRotation(Vector3.ProjectOnPlane(rb.velocity, surfaceNormal),surfaceNormal);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 1f*Time.deltaTime);   


            //Ant down force
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast (transform.position, -transform.up, out hit, 0.8f, ~antLayer)) {
                rb.AddForce(-surfaceNormal * downForce);
            }else{
                rb.AddForce(-Vector3.up * downForce);
            }           
            
        }
        if(animator){
            animator.SetFloat("velocity", 2*rb.velocity.magnitude * rb.velocity.magnitude );
        }
        
    }

    void LateUpdate(){
        
        var head = transform.Find("AntBody/Armature/Main/Head").transform;
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(transform.position, desiredDirection, Color.green);

        var angle = Vector3.SignedAngle(transform.forward, desiredDirection, transform.up);
        
        var z = (angle - head.localEulerAngles.z) / 3 ;
        if (Mathf.Abs( z ) > 0.1f ) head.localEulerAngles = new Vector3(head.localEulerAngles.x,head.localEulerAngles.y,angle);
    }
    GameObject PickTarget(){
        float? minDist = null;
        GameObject closest = null;
        foreach (var item in Targets)
        {
            if (item == null) continue;
            if(minDist == null || (transform.position - item.transform.position).magnitude < minDist ){
                minDist = (transform.position - item.transform.position).magnitude;
                closest = item;
            }
        }
        return closest;
    }

    Vector3 GetTargetSurfaceNormal(){
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast (transform.position - transform.up*0.03f, Quaternion.AngleAxis(-5,transform.up)*transform.forward, out hit, climbDist, ~antLayer) || Physics.Raycast (transform.position - transform.up*0.06f, Quaternion.AngleAxis(5,transform.up)*transform.forward, out hit, climbDist, ~antLayer)) {
            return hit.normal;
        }else if(Physics.Raycast (transform.position - transform.up*0.02f+ transform.forward*0.01f, Quaternion.Euler(15, 0, 0) * -transform.up, out hit, Mathf.Infinity,~antLayer)){
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

    Vector3 GetDir(Vector3 surfaceNormal){
        if(Targets.Count > 0){
            var target = PickTarget();
            if (target!=null){
                TryInteract(target);
                return Vector3.ProjectOnPlane(target.transform.position - transform.position, surfaceNormal).normalized;
            } 
            else return transform.forward;
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

    // Spitting phero to indicates route every 1 s
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
        var right = Quaternion.Euler(0, 45, 0) * front;
        var left = Quaternion.Euler(0, -45, 0) * front;
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast (transform.position + left, - Vector3.up, out hit, Mathf.Infinity, obstacleLayer)){
            return right;
        }else if(Physics.Raycast (transform.position + right, - Vector3.up, out hit, Mathf.Infinity, obstacleLayer)){
            return left;
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
