using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public Rigidbody rb;
    public LayerMask obstacleLayer;
    public bool isWandering;
    public float downForce;
    public float climbDist;
    public int detectionRadius;
    public float speed;
    public float reactionTime;
    public int turnAngle;
    public float lerpSpeed;

    float counter = 0;
    Vector3 surfaceNormal;
    Vector3 normal;
    bool isLerping = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(7,7);
    }

    void FixedUpdate()
    {
        if(!UserInterface.isGamePaused){
            normal = transform.up;
        
        
            // Getting surface normal
            RaycastHit hit = new RaycastHit();
            // if wall in reach choose it as new normal references
            if (Physics.Raycast (transform.position + (transform.up * 0.15f), transform.forward, out hit, climbDist)) {
                surfaceNormal = hit.normal;
                
            }else if(Physics.Raycast (transform.position + transform.up*0.15f + transform.forward*0.1f, Quaternion.Euler(15, 0, 0) * -transform.up, out hit, Mathf.Infinity)){
                surfaceNormal = hit.normal;
            }
            
            
            Quaternion targetRot = Quaternion.LookRotation(Vector3.Cross(transform.right,surfaceNormal), surfaceNormal);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed*Time.deltaTime);
            transform.Rotate(0, PickDirection(), 0);

            //Debug.DrawRay(transform.position + (transform.up * 0.15f), transform.forward, Color.green);
            //Debug.DrawRay(transform.position, normal, Color.red);
            //Debug.DrawRay(transform.position + transform.up*0.15f + transform.forward*0.1f, Quaternion.Euler(15, 0, 0) * -transform.up, Color.blue);

            //fake gravity
            rb.AddForce(-normal * downForce);
            
            // Walking to forward direction
            rb.velocity = transform.forward * speed * Time.fixedDeltaTime;
        }else{
            rb.velocity = Vector3.zero;
        }
        
    }

    // Return a rotation to avoid end of map
    float PickDirection(){
        float dChange = 0;

        // Wandering
        counter += Time.deltaTime;
        if(counter >= reactionTime && isWandering){
            counter -= reactionTime;
            dChange = Random.Range(-turnAngle,turnAngle);
        }

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
}
