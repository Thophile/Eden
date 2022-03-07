using Assets.Scripts.Model;
using Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class Ant : MonoBehaviour
    {
        // Parameters
        public string prefabName;
        public int pheroDetectionRange;
        public float activationRadius;
        public float downForce;
        public float climbDist;
        public int damage;
        public LayerMask waterLayer;
        public LayerMask antLayer;
        public Rigidbody rb;
        public AntState state;
        public float maxVelocity;
        public float wanderStrenght;
        public float pheroStrenght;
        public float markingDistance;
        public float groundDistance;
        public float previousPosDistance;
        private Vector3 previousMark;
        private Vector3 velocity;
        private AntProxy proxy;


        public GameObject Load
        {
            get { return _load; }
            set
            {
                if (value != null)
                {
                    state = AntState.GoingHome;
                    _load = value;
                }
            }
        }
        GameObject _load = null;
        public Transform loadPos;
        public List<GameObject> Targets = new List<GameObject>();
        public List<TimedPosition> previousPositions;

        Animator animator;

        Vector3 surfaceNormal;
        Vector3 desiredDirection;
        Quaternion targetRot;

        void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            rb = gameObject.GetComponent<Rigidbody>();

            rb.isKinematic = GameManager.isPaused;
            desiredDirection = transform.forward;
            surfaceNormal = Vector3.up;
            targetRot = transform.rotation;
            proxy = new AntProxy(this);

            Physics.IgnoreLayerCollision(7, 7);
        }

        void Update()
        {
            if (!GameManager.isPaused)
            {
                proxy.Init(this);
                //Apply stickingForce if grounded else apply gravity
                if (Physics.Raycast(transform.position, -transform.up, out _, groundDistance, ~antLayer))
                {
                    rb.AddForce(-surfaceNormal * downForce);
                    // Setting rotation and velocity
                    float turnRatio = 1 - (Vector3.Angle(desiredDirection, rb.velocity) / 180);
                    rb.MoveRotation(Quaternion.Lerp(transform.rotation, targetRot, 3f * Time.deltaTime));
                    rb.velocity = velocity = ((maxVelocity * turnRatio * turnRatio * transform.forward + velocity * 2) / 3f);
                }
                else
                {
                    rb.AddForce(10 * downForce * -Vector3.up);
                    targetRot = Quaternion.LookRotation(transform.forward, Vector3.up);
                }
            }
            if (animator)
            {
                animator.SetFloat("velocity", 6 * (velocity.sqrMagnitude / (maxVelocity * maxVelocity)));
            }
        }

        public void UpdateSelf()
        {
            if (!GameManager.isPaused)
            {
                // Surface alignement
                Vector3 newNormal = GetTargetSurfaceNormal(proxy);

                // New direction choice
                Vector3 newDirection = GetDesiredDir(proxy);

                // Recalculate rotation
                if (surfaceNormal != newNormal || newDirection != desiredDirection)
                {
                    surfaceNormal = newNormal;
                    desiredDirection = newDirection;
                    targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(desiredDirection, surfaceNormal), surfaceNormal);
                }
            }
        }

        void LateUpdate()
        {

            var head = transform.Find("Body/Armature/Main/Head").transform;
            var angle = Vector3.SignedAngle(transform.forward, desiredDirection, transform.up);
            var targetRot = Quaternion.Euler(head.localEulerAngles.x, head.localEulerAngles.y, angle);
            head.localRotation = Quaternion.Lerp(head.localRotation, targetRot, 2f * Time.deltaTime);
        }

        Vector3 GetTargetSurfaceNormal(AntProxy proxy)
        {
            if (proxy.climbCheckL) return proxy.climbNormalL;
            if (proxy.climbCheckR) return proxy.climbNormalR;
            if (proxy.surfaceCheck) return proxy.surfaceNormal;
            return surfaceNormal;
        }

        Vector3 GetDesiredDir(AntProxy proxy)
        {
            Vector3 dryPathDir = GetDryPathDir(proxy);
            if (dryPathDir == Vector3.zero)
            {
                MarkPath(proxy);
                var randomDir = GetRandomDir(proxy);
                var targetDir = GetTargetDir(proxy);

                return (wanderStrenght * randomDir) + targetDir * pheroStrenght;
            }
            else
            {
                return dryPathDir;
            }
        }

        Vector3 GetRandomDir(AntProxy proxy)
        {
            return Vector3.ProjectOnPlane(new Vector3(proxy.random.x, 0, proxy.random.y), surfaceNormal);
        }

        Vector3 GetTargetDir(AntProxy proxy)
        {

            if(proxy.target != null && TryInteract(proxy))
            {

                return (proxy.target.transform.position - proxy.position).normalized;
            }
            /*if (Targets.Count > 0)
            {
                //TODO proxy target calc
                var target = PickTarget();

                if (target != null && !TryInteract(target))
                {
                    return (target.transform.position - proxy.position).normalized;
                }
            }*/

            Vector3 targetDir = proxy.forward;

            switch (state)
            {
                case AntState.Wandering:
                    const float magnitude = 0.5f;
                    Vector3 center = proxy.forward * magnitude;
                    Vector3 left = Quaternion.AngleAxis(-30, proxy.up) * proxy.forward * magnitude;
                    Vector3 right = Quaternion.AngleAxis(30, proxy.up) * proxy.forward * magnitude;

                    var centerPhero = GameManager.gameState.pheromonesMap.ComputeZone(proxy.position + center, pheroDetectionRange);
                    var leftPhero = GameManager.gameState.pheromonesMap.ComputeZone(proxy.position + left, pheroDetectionRange);
                    var rightPhero = GameManager.gameState.pheromonesMap.ComputeZone(proxy.position + right, pheroDetectionRange);

                    //Debug.DrawLine(proxy.position + center, proxy.position + center + Vector3.up * centerPhero, Color.white, 1f);
                    //Debug.DrawLine(proxy.position + left, proxy.position + left + Vector3.up * leftPhero, Color.white, 1f);
                    //Debug.DrawLine(proxy.position + right, proxy.position + right + Vector3.up * rightPhero, Color.white, 1f);

                    if (centerPhero >= leftPhero && centerPhero >= rightPhero) targetDir = center;
                    else if (leftPhero >= centerPhero && leftPhero >= rightPhero) targetDir = left;
                    else targetDir = right;
                    break;
                case AntState.GoingHome:
                    TimedPosition oldestPos = null;

                    foreach (TimedPosition pos in previousPositions)
                    {
                        if ((pos.Position - proxy.position).sqrMagnitude < previousPosDistance * previousPosDistance)
                        {
                            oldestPos = pos;
                            break;
                        }
                    }
                    targetDir = (oldestPos.Position - proxy.position);
                    break;
            }
            return targetDir;
        }

        Vector3 GetDryPathDir(AntProxy proxy)
        {
            // Obstacle Avoidance
            if (proxy.DryPathCheckL && proxy.DryPathLayerL == 4)
            {
                return -proxy.DryPathVectorL;
            }
            else if (proxy.DryPathCheckR && proxy.DryPathLayerR == 4)
            {
                return -proxy.DryPathVectorR;
            }
            else
            {
                return Vector3.zero;
            }
        }

        void MarkPath(AntProxy proxy)
        {
            if (previousMark == null || (previousMark - proxy.position).sqrMagnitude > markingDistance * markingDistance)
            {
                previousMark = proxy.position;

                switch (state)
                {
                    case AntState.Wandering:
                        this.previousPositions.Add(new TimedPosition(proxy.position));
                        break;
                    case AntState.GoingHome:
                        GameManager.gameState.pheromonesMap.Mark(proxy.position);
                        break;
                }
            }
        }

        bool TryInteract(AntProxy proxy)
        {
            if (proxy.target != null && Vector3.Magnitude(proxy.targetPosition - proxy.position) < activationRadius)
            {
                proxy.targetInteractable.Interact(this);
                return true;
            }
            return false;
        }

        public GameObject PickTarget()
        {
            float? minDist = null;
            GameObject closest = null;
            foreach (var item in Targets)
            {
                if (item == null) continue;
                if (item.GetComponent<Exit>() && state == AntState.Wandering) continue;
                if (item.GetComponent<Resource>() && Load != null) continue;
                if (minDist == null || (transform.position - item.transform.position).sqrMagnitude < minDist)
                {
                    minDist = (transform.position - item.transform.position).sqrMagnitude;
                    closest = item;
                }
            }
            return closest;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Interactable>())
            {
                Targets.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Interactable>())
            {
                Targets.Remove(other.gameObject);
            }
        }
    }
}
