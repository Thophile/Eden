using Assets.Scripts.Model;
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
        public List<PreviousPosition> previousPositions;

        Animator animator;

        Vector3 surfaceNormal;
        Vector3 desiredDirection;
        Quaternion targetRot;

        void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            rb = gameObject.GetComponent<Rigidbody>();

            rb.isKinematic = WorldManager.isPaused;
            desiredDirection = transform.forward;
            surfaceNormal = Vector3.up;
            targetRot = transform.rotation;

            Physics.IgnoreLayerCollision(7, 7);
        }

        void Update()
        {
            if (!WorldManager.isPaused)
            {
                // Setting rotation and velocity
                float turnRatio = 1 - (Vector3.Angle(desiredDirection, rb.velocity) / 180);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 3f * Time.deltaTime);
                rb.velocity= velocity = ((maxVelocity * turnRatio * turnRatio * transform.forward + velocity * 2) / 3f);
            }
            if (animator)
            {
                animator.SetFloat("velocity", 6 * (velocity.sqrMagnitude/(maxVelocity*maxVelocity)));
            }
        }

        public void UpdateSelf()
        {
            if (!WorldManager.isPaused)
            {
                // Surface alignement
                Vector3 newNormal = GetTargetSurfaceNormal();

                // New direction choice
                Vector3 newDirection = GetDesiredDir();

                // Recalculate rotation
                if (surfaceNormal != newNormal || newDirection != desiredDirection)
                {
                    surfaceNormal = newNormal;
                    desiredDirection = newDirection;
                    targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(desiredDirection, surfaceNormal), surfaceNormal);


                }
                //Apply stickingForce if grounded else apply gravity
                if (Physics.Raycast(transform.position, -transform.up, out _, groundDistance, ~antLayer))
                {
                    rb.AddForce(-surfaceNormal * downForce);
                }
                else
                {
                    rb.AddForce(10 * downForce * -Vector3.up);
                    targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(desiredDirection, Vector3.up), Vector3.up);
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

        Vector3 GetTargetSurfaceNormal()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position - transform.up * 0.01f, Quaternion.AngleAxis(-5, transform.up) * transform.forward, out hit, climbDist, ~antLayer) || Physics.Raycast(transform.position - transform.up * 0.06f, Quaternion.AngleAxis(5, transform.up) * transform.forward, out hit, climbDist, ~antLayer))
            {
                return hit.normal;
            }
            else if (Physics.Raycast(transform.position - transform.up * 0.02f + transform.forward * 0.01f, Quaternion.Euler(15, 0, 0) * -transform.up, out hit, Mathf.Infinity, ~antLayer))
            {
                return hit.normal;
            }
            else
            {
                return surfaceNormal;
            }
        }

        Vector3 GetDesiredDir()
        {
            Vector3 dryPathDir = GetDryPathDir();
            if (dryPathDir == Vector3.zero)
            {
                MarkPath();
                var randomDir = GetRandomDir();
                var targetDir = GetTargetDir();

                return (wanderStrenght * WorldManager.activeAnts.Count * randomDir / 200f) + targetDir * pheroStrenght;
            }
            else
            {
                return dryPathDir;
            }
        }

        Vector3 GetRandomDir()
        {
            Vector2 random = Random.insideUnitCircle;
            return Vector3.ProjectOnPlane(new Vector3(random.x, 0, random.y), surfaceNormal);
        }

        Vector3 GetTargetDir()
        {
            

            if (Targets.Count > 0)
            {
                var target = PickTarget();

                if (target != null && !TryInteract(target))
                {
                    return (target.transform.position - transform.position).normalized;
                }
            }

            Vector3 targetDir = transform.forward;

            switch (state)
            {
                case AntState.Wandering:
                    const float magnitude = 0.5f;
                    Vector3 center = transform.forward * magnitude;
                    Vector3 left = Quaternion.AngleAxis(-30, transform.up) * transform.forward * magnitude;
                    Vector3 right = Quaternion.AngleAxis(30, transform.up) * transform.forward * magnitude;

                    var centerPhero = WorldManager.gameState.pheromonesMap.ComputeZone(transform.position + center, pheroDetectionRange);
                    var leftPhero = WorldManager.gameState.pheromonesMap.ComputeZone(transform.position + left, pheroDetectionRange);
                    var rightPhero = WorldManager.gameState.pheromonesMap.ComputeZone(transform.position + right, pheroDetectionRange);

                    Debug.DrawLine(transform.position + center, transform.position + center + Vector3.up * centerPhero, Color.white, 1f);
                    Debug.DrawLine(transform.position + left, transform.position + left + Vector3.up * leftPhero, Color.white, 1f);
                    Debug.DrawLine(transform.position + right, transform.position + right + Vector3.up * rightPhero, Color.white, 1f);

                    if (centerPhero >= leftPhero && centerPhero >= rightPhero) targetDir = center;
                    else if (leftPhero >= centerPhero && leftPhero >= rightPhero) targetDir = left;
                    else targetDir = right;
                    break;
                case AntState.GoingHome:
                    PreviousPosition oldestPos = null;

                    List<PreviousPosition> inRanges = new List<PreviousPosition>();
                    previousPositions.ForEach((PreviousPosition pos) =>
                    {
                        if((pos.Position - transform.position).sqrMagnitude < previousPosDistance * previousPosDistance)
                        {
                            inRanges.Add(pos);
                        }
                    });
                    inRanges.ForEach((PreviousPosition pos) =>
                    {
                        if(oldestPos == null || pos.time < oldestPos.time)
                        {
                            oldestPos = pos;
                        }
                    });
                    targetDir = (oldestPos.Position - transform.position);
                    break;
            }
            return targetDir;
        }

        Vector3 GetDryPathDir()
        {
            // Obstacle Avoidance
            var right = Quaternion.Euler(0, 30, 0) * transform.forward * 0.3f;
            var left = Quaternion.Euler(0, -30, 0) * transform.forward * 0.3f;

            RaycastHit hit;
            if (Physics.Raycast(transform.position + left + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, ~antLayer) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                return -left;
            }
            else if (Physics.Raycast(transform.position + right + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, ~antLayer) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                return -right;
            }
            else
            {
                return Vector3.zero;
            }
        }

        void MarkPath()
        {
            if (previousMark == null || (previousMark - transform.position).sqrMagnitude > markingDistance * markingDistance)
            {
                previousMark = transform.position;

                switch (state)
                {
                    case AntState.Wandering:
                        this.previousPositions.Add(new PreviousPosition(transform.position));
                        break;
                    case AntState.GoingHome:
                    WorldManager.gameState.pheromonesMap.Mark(transform.position);
                        break;
                }
            }
        }

        bool TryInteract(GameObject target)
        {

            if (target != null && Vector3.Magnitude(target.transform.position - transform.position) < activationRadius)
            {
                target.GetComponent<Interactable>().Interact(this);
                return true;
            }
            return false;
        }

        GameObject PickTarget()
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
