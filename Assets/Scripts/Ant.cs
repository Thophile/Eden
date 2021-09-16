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
    public LayerMask waterLayer;
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
    Vector3 oldPheroPos;

    // Serialized
    public float maxVelocity = 20f;
    public float wanderStrenght = 0.5f;
    Animator animator;

    Vector3 surfaceNormal;
    Vector3 desiredDirection;
    Quaternion targetRot;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();

        rb.isKinematic = UserInterface.isGamePaused;
        desiredDirection = transform.forward;
        surfaceNormal = Vector3.up;
        targetRot = transform.rotation;

        Physics.IgnoreLayerCollision(7,7);
    }

    void Update(){
        if(!UserInterface.isGamePaused){   
            // Setting rotation and velocity
            float turnRatio = 1 - (Vector3.Angle(desiredDirection, rb.velocity)/180);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 3f * Time.deltaTime);   
            //transform.rotation = targetRot;
            //rb.velocity = Vector3.ClampMagnitude(rb.velocity + (desiredDirection-rb.velocity) * 2f * Time.deltaTime, maxVelocity) ;
            rb.velocity = (transform.forward * maxVelocity * turnRatio + rb.velocity)/2f;
        }
        if(animator){
            animator.SetFloat("velocity", 4*rb.velocity.magnitude );
        }
    }
    public void UpdateSelf(){
        if(!UserInterface.isGamePaused){                      
            // Debug direction
            //Debug.DrawRay(transform.position, desiredDirection, Color.red);
            //Debug.DrawRay(transform.position, targetVelocity, Color.blue);
            //Debug.DrawRay(transform.position, rb.velocity, Color.magenta);


            // Surface alignement
            Vector3 newNormal = GetTargetSurfaceNormal();

            // New direction choice
            Vector3 newDirection = GetDesiredDir();

            // Recalculate rotation
            if(surfaceNormal != newNormal || newDirection != desiredDirection){
                surfaceNormal = newNormal;
                desiredDirection = newDirection;
                targetRot =  Quaternion.LookRotation(Vector3.ProjectOnPlane(desiredDirection, surfaceNormal),surfaceNormal);


                //Apply stickingForce if grounded else apply gravity
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast (transform.position, -transform.up, out hit, 0.4f, ~antLayer)) {
                    rb.AddForce(-surfaceNormal * downForce);
                }else{
                    rb.AddForce(-Vector3.up * 10*downForce);
                }           
            }

            

        }
        
        
    }

    void LateUpdate(){
        
        var head = transform.Find("Body/Armature/Main/Head").transform;
        var angle = Vector3.SignedAngle(transform.forward, desiredDirection, transform.up);
        var targetRot = Quaternion.Euler(head.localEulerAngles.x,head.localEulerAngles.y,angle);
        head.localRotation = Quaternion.Lerp(head.localRotation, targetRot, 2f*Time.deltaTime);
    }

    Vector3 GetTargetSurfaceNormal(){
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast (transform.position - transform.up*0.01f, Quaternion.AngleAxis(-5,transform.up)*transform.forward, out hit, climbDist, ~antLayer) || Physics.Raycast (transform.position - transform.up*0.06f, Quaternion.AngleAxis(5,transform.up)*transform.forward, out hit, climbDist, ~antLayer)) {
            return hit.normal;
        }else if(Physics.Raycast (transform.position - transform.up*0.02f+ transform.forward*0.01f, Quaternion.Euler(15, 0, 0) * -transform.up, out hit, Mathf.Infinity,~antLayer)){
            return hit.normal;
        }else{
            return surfaceNormal;
        }
    }
    Vector3 GetDesiredDir(){
        Vector3 dryPathDir = GetDryPathDir();
        if(dryPathDir == Vector3.zero){ 
            MarkPath();
            return GetRandomDir() * wanderStrenght * WorldManager.activeAnts.Count/200f + GetTargetDir();
        }else{
            return dryPathDir;
        }
    }
    Vector3 GetRandomDir(){
        Vector2 random = Random.insideUnitCircle;
        return Vector3.ProjectOnPlane(new Vector3(random.x, 0, random.y), surfaceNormal);
    }
    Vector3 GetTargetDir(){
        if(Targets.Count > 0){
            var target = PickTarget();
            if (target!=null){
                TryInteract(target);
                return (target.transform.position - transform.position).normalized;
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
                    centerValue = centerPhero.y-centerPhero.z;
                    leftValue = leftPhero.y-leftPhero.z;
                    rightValue = rightPhero.y-rightPhero.z;
                    break;
                case AntState.GoingHome:
                    centerValue = centerPhero.x;
                    leftValue = leftPhero.x;
                    rightValue = rightPhero.x;
                    break;

            }

            if(centerValue >= leftValue && centerValue >= rightValue) return center.normalized;
            else if(leftValue >= centerValue && leftValue >= rightValue) return left.normalized;
            else return right.normalized;
        }
    }
    Vector3 GetDryPathDir(){
        // Obstacle Avoidance
        var right = Quaternion.Euler(0, 30, 0) * transform.forward * 0.3f;
        var left = Quaternion.Euler(0, -30, 0) * transform.forward * 0.3f;
        
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast (transform.position + left + Vector3.up, - Vector3.up, out hit, Mathf.Infinity, ~antLayer) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Water")){
            //Debug.DrawRay(transform.position + left + Vector3.up, - Vector3.up,Color.red);
            return -left;
        }else if(Physics.Raycast (transform.position + right + Vector3.up, - Vector3.up, out hit, Mathf.Infinity, ~antLayer) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Water")){
            //Debug.DrawRay(transform.position + right + Vector3.up, - Vector3.up,Color.red);
            return -right;
        }else {
            //Debug.DrawRay(transform.position + left + Vector3.up, - Vector3.up,Color.green);
            //Debug.DrawRay(transform.position + right + Vector3.up, - Vector3.up,Color.green);
            return Vector3.zero;
        }
    }
    void MarkPath(){
        MarkerType type = MarkerType.Wander;
        if(state == AntState.GoingHome) type = MarkerType.Resource;

        GameState.current.pheromonesMap.Mark(transform.position, type);
    }
    void TryInteract(GameObject target){
        
        if(target!= null && Vector3.Magnitude(target.transform.position - transform.position) < activationRadius){
            target.GetComponent<Interactable>().Interact(this);
        }
    }
    GameObject PickTarget(){
        float? minDist = null;
        GameObject closest = null;
        foreach (var item in Targets)
        {
            if (item.GetComponent<Exit>() && state == AntState.Wandering) continue;
            if (item == null) continue;
            if(minDist == null || (transform.position - item.transform.position).sqrMagnitude < minDist ){
                minDist = (transform.position - item.transform.position).sqrMagnitude;
                closest = item;
            }
        }
        return closest;
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
