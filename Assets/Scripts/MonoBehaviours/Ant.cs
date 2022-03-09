using Assets.Scripts.Model;
using Assets.Scripts.Proxies;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class Ant : MonoBehaviour
    {
        // Parameters
        [Header("General")]
        public string prefabName;
        [Header("References")]
        public Rigidbody rb;
        public Transform head;
        public Transform loadPos;
        public LayerMask waterLayer;
        public LayerMask antLayer;

        [Header("Movement")]
        public float updateDelay;
        public float updateWindow;
        public float groundDist;
        public float climbDist;
        public float maxVelocity;
        public float downForce;
        public float wanderStrenght;
        public float pheroStrenght;
        public int pheroRange;
        public float markingRange;
        public float memoryRange;

        [Header("Interactions")]
        public float activationRadius;
        public int damage;

        [HideInInspector] public bool grounded = false;
        [HideInInspector] public AntProxy proxy;
        [HideInInspector] public AntState state;
        [HideInInspector] public List<GameObject> Targets = new List<GameObject>();
        [HideInInspector] public List<TimedPosition> previousPositions;
        private float timestamp;
        private Vector3 velocity;
        private Vector3 previousMark;
        private Vector3 surfaceNormal;
        private Vector3 desiredDirection;
        private Quaternion targetRot;
        Animator animator;


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

        void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            rb = gameObject.GetComponent<Rigidbody>();

            rb.isKinematic = GameManager.isPaused;
            desiredDirection = transform.forward;
            surfaceNormal = Vector3.up;
            targetRot = transform.rotation;
            timestamp = GameManager.gameState.gameTime;
            proxy = new AntProxy(this);
            Physics.IgnoreLayerCollision(7, 7);
        }

        void Update()
        {
            if (!GameManager.isPaused)
            {
                //Apply down force
                rb.AddForce(downForce * -surfaceNormal);

                if (grounded)
                {
                    // Setting rotation and velocity
                    float turnRatio = 1 - (Vector3.Angle(desiredDirection, rb.velocity) / 180);
                    rb.MoveRotation(Quaternion.Lerp(transform.rotation, targetRot, 3f * Time.deltaTime));
                    rb.velocity = velocity = ((maxVelocity * turnRatio * turnRatio * transform.forward + velocity * 2) / 3f);
                }

                //Register for update
                if (GameManager.gameState.gameTime - timestamp > Random.Range(updateDelay - updateWindow, updateDelay + updateWindow))
                {
                    timestamp = GameManager.gameState.gameTime;
                    GameManager.antsToUpdate.Add(this);
                }

            }
            if (animator)
            {
                animator.SetFloat("velocity", 4 * (velocity.sqrMagnitude / (maxVelocity * maxVelocity)));
            }
        }

        public void UpdateSelf()
        {
            if (!GameManager.isPaused)
            {
                proxy.Init(this);
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
            var angle = Vector3.SignedAngle(proxy.forward, desiredDirection, proxy.up);
            var targetRot = Quaternion.Euler(head.localEulerAngles.x, head.localEulerAngles.y, angle);
            head.localRotation = Quaternion.Lerp(head.localRotation, targetRot, 2 * Time.deltaTime);
        }

        Vector3 GetTargetSurfaceNormal(AntProxy proxy)
        {
            RaycastHit hit;
            if (Physics.Raycast(proxy.position - proxy.up * 0.02f, proxy.forward, out hit, climbDist, ~antLayer))
            {
                return hit.normal;
            }
            else if (Physics.Raycast(proxy.position - proxy.up * 0.01f + proxy.forward * 0.02f, Quaternion.Euler(30, 0, 0) * -proxy.up, out hit, 0.06f, ~antLayer))
            {
                grounded = true;
                return hit.normal;
            }
            else
            {
                grounded = false;
                return Vector3.up;
            }
        }

        Vector3 GetDesiredDir(AntProxy proxy)
        {
            Vector3 dryPathDir = GetDryPathDir(proxy);
            if (dryPathDir == Vector3.zero)
            {
                MarkPath(proxy);
                var randomDir = GetRandomDir();
                var targetDir = GetTargetDir(proxy);

                return (wanderStrenght * randomDir) + targetDir * pheroStrenght;
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

        Vector3 GetTargetDir(AntProxy proxy)
        {
            var target = PickTarget(proxy);
            if (target != null && TryInteract(target))
            {

                return (target.transform.position - proxy.position).normalized;
            }

            Vector3 targetDir = proxy.forward;

            switch (state)
            {
                case AntState.Wandering:
                    const float magnitude = 0.5f;
                    Vector3 center = proxy.forward * magnitude;
                    Vector3 left = Quaternion.AngleAxis(-30, proxy.up) * proxy.forward * magnitude;
                    Vector3 right = Quaternion.AngleAxis(30, proxy.up) * proxy.forward * magnitude;

                    var centerPhero = GameManager.gameState.pheromonesMap.ComputeZone(proxy.position + center, pheroRange);
                    var leftPhero = GameManager.gameState.pheromonesMap.ComputeZone(proxy.position + left, pheroRange);
                    var rightPhero = GameManager.gameState.pheromonesMap.ComputeZone(proxy.position + right, pheroRange);

                    Debug.DrawLine(proxy.position + center, proxy.position + center + Vector3.up * centerPhero, Color.white, 1f);
                    Debug.DrawLine(proxy.position + left, proxy.position + left + Vector3.up * leftPhero, Color.white, 1f);
                    Debug.DrawLine(proxy.position + right, proxy.position + right + Vector3.up * rightPhero, Color.white, 1f);

                    if (centerPhero >= leftPhero && centerPhero >= rightPhero) targetDir = center;
                    else if (leftPhero >= centerPhero && leftPhero >= rightPhero) targetDir = left;
                    else targetDir = right;
                    break;
                case AntState.GoingHome:
                    TimedPosition oldestPos = null;

                    foreach (TimedPosition pos in previousPositions)
                    {
                        if ((pos.Position - proxy.position).sqrMagnitude < memoryRange * memoryRange)
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
            var right = Quaternion.Euler(0, 30, 0) * proxy.forward * 0.3f;
            var left = Quaternion.Euler(0, -30, 0) * proxy.forward * 0.3f;

            RaycastHit hit;
            if (Physics.Raycast(proxy.position + left + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, ~antLayer) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                return -left;
            }
            else if (Physics.Raycast(proxy.position + right + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, ~antLayer) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                return -right;
            }
            else
            {
                return Vector3.zero;
            }
        }

        void MarkPath(AntProxy proxy)
        {
            if (previousMark == null || (previousMark - proxy.position).sqrMagnitude > markingRange * markingRange)
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

        bool TryInteract(GameObject target)
        {

            if (target != null && Vector3.Magnitude(target.transform.position - transform.position) < activationRadius)
            {
                target.GetComponent<Interactable>().Interact(this);
                return true;
            }
            return false;
        }

        GameObject PickTarget(AntProxy proxy)
        {
            float? minDist = null;
            GameObject closest = null;
            foreach (var item in Targets)
            {
                if (item == null) continue;
                if (item.GetComponent<Exit>() && state == AntState.Wandering) continue;
                if (item.GetComponent<Resource>() && Load != null) continue;
                if (minDist == null || (proxy.position - item.transform.position).sqrMagnitude < minDist)
                {
                    minDist = (proxy.position - item.transform.position).sqrMagnitude;
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
