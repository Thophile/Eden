using Assets.Scripts.Model;
using Assets.Scripts.Model.Interfaces;
using Assets.Scripts.Proxies;
using Assets.Scripts.Ui;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class Ant : MonoBehaviour, ISaveable
    {
        // Parameters
        [Header("References")]
        public Rigidbody rigidbodyRef;
        public Transform head;
        public Transform loadPos;
        public LayerMask waterLayer;
        public LayerMask antLayer;

        [Header("Movement")]
        public float updateDelay;
        public float updateWindow;
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
        [HideInInspector] public TransformProxy transformProxy;
        [HideInInspector] public AntState state;
        [HideInInspector] public List<GameObject> targets = new List<GameObject>();
        [HideInInspector] public List<TimedPosition> previousPositions = new List<TimedPosition>();
        private float timestamp;
        private int instantiationFrame;
        private Vector3 velocity;
        private Vector3 previousMark;
        private Vector3 newNormal;
        private Vector3 newDirection;
        private Quaternion newRotation;
        Animator animatorRef;
        Renderer rendererRef;

        public GameObject Shipement
        {
            get { return _shipement; }
            set
            {
                if (value != null)
                {
                    state = AntState.GoingHome;
                    _shipement = value;
                }
            }
        }
        GameObject _shipement = null;

        void Start()
        {
            animatorRef = gameObject.GetComponent<Animator>();
            rendererRef = gameObject.GetComponentInChildren<Renderer>();
            rigidbodyRef = gameObject.GetComponent<Rigidbody>();

            rigidbodyRef.isKinematic = GameManager.isPaused;

            newDirection = transform.forward;
            newNormal = Vector3.up;
            newRotation = transform.rotation;

            timestamp = GameManager.gameState.gameTime;
            instantiationFrame = Time.frameCount;
            transformProxy = new TransformProxy(transform);
        }

        void Update()
        {
            if (!GameManager.isPaused)
            {
                rigidbodyRef.MoveRotation(Quaternion.Lerp(transform.rotation, newRotation, 12 * Time.deltaTime));

                //Apply down force
                rigidbodyRef.AddForce((-downForce * newNormal) - Vector3.Project(rigidbodyRef.velocity, newNormal), ForceMode.VelocityChange);

                if (grounded)
                {
                    // Setting rotation and velocity
                    float turnRatio = 1 - (Vector3.Angle(newDirection, rigidbodyRef.velocity) / 180);
                    rigidbodyRef.AddForce(
                        ((maxVelocity * turnRatio * turnRatio * transform.forward + rigidbodyRef.velocity * 2) / 3f)
                        - Vector3.ProjectOnPlane(rigidbodyRef.velocity, newNormal),
                        ForceMode.VelocityChange);
                }

                //Register for update
                if (GameManager.gameState.gameTime - timestamp > Random.Range(updateDelay - updateWindow, updateDelay + updateWindow))
                {
                    timestamp = GameManager.gameState.gameTime;
                    GameManager.antsToUpdate.Add(this);
                }
            }
            if (animatorRef)
            {
                animatorRef.SetFloat("velocity", 6 * (velocity.sqrMagnitude / (maxVelocity * maxVelocity)));
            }

        }
        void LateUpdate()
        {
            if ((Time.frameCount - instantiationFrame) % 3 == 0 && rendererRef.isVisible) {
                var angle = Vector3.SignedAngle(transformProxy.forward, newDirection, transformProxy.up);
                var localEuler = head.localEulerAngles;
                head.localRotation = Quaternion.Lerp(head.localRotation,
                                                     Quaternion.Euler(localEuler.x, localEuler.y, angle),
                                                     5 * Time.deltaTime);
            }
        }

        public void UpdateSelf()
        {
            if (!GameManager.isPaused)
            {
                transformProxy.Init(transform);
                // Surface alignement
                Vector3 surfaceNormal = GetTargetSurfaceNormal(transformProxy);

                // New direction choice
                Vector3 desiredDirection = GetDesiredDir(transformProxy);

                // Recalculate rotation
                if (surfaceNormal != newNormal || desiredDirection != newDirection)
                {
                    newNormal = surfaceNormal;
                    newDirection = desiredDirection;
                    newRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(newDirection, newNormal), newNormal);
                }
            }
        }


        Vector3 GetTargetSurfaceNormal(TransformProxy transformProxy)
        {
            RaycastHit hit;
            if (Physics.Raycast(transformProxy.position - transformProxy.up * 0.02f, transformProxy.forward, out hit, 0.06f, ~antLayer))
            {
                return hit.normal;
            }
            else if (Physics.Raycast(transformProxy.position + transformProxy.forward * 0.02f, Quaternion.Euler(10, 0, 0) * -transformProxy.up, out hit, 0.06f, ~antLayer))
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

        Vector3 GetDesiredDir(TransformProxy transformProxy)
        {
            Vector3 dryPathDir = GetDryPathDir(transformProxy);
            if (dryPathDir == Vector3.zero)
            {
                MarkPath(transformProxy);
                var randomDir = GetRandomDir();
                var targetDir = GetTargetDir(transformProxy);

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
            return Vector3.ProjectOnPlane(new Vector3(random.x, 0, random.y), newNormal);
        }

        Vector3 GetTargetDir(TransformProxy transformProxy)
        {
            var target = PickTarget(transformProxy);
            if (target != null && TryInteract(target))
            {

                return (target.transform.position - transformProxy.position).normalized;
            }

            Vector3 targetDir = transformProxy.forward;

            switch (state)
            {
                case AntState.Wandering:
                    const float magnitude = 0.5f;
                    Vector3 center = transformProxy.forward * magnitude;
                    Vector3 left = Quaternion.AngleAxis(-30, transformProxy.up) * transformProxy.forward * magnitude;
                    Vector3 right = Quaternion.AngleAxis(30, transformProxy.up) * transformProxy.forward * magnitude;

                    var centerPhero = GameManager.gameState.pheromonesMap.ComputeZone(transformProxy.position + center, pheroRange);
                    var leftPhero = GameManager.gameState.pheromonesMap.ComputeZone(transformProxy.position + left, pheroRange);
                    var rightPhero = GameManager.gameState.pheromonesMap.ComputeZone(transformProxy.position + right, pheroRange);

                    if (centerPhero >= leftPhero && centerPhero >= rightPhero) targetDir = center;
                    else if (leftPhero >= centerPhero && leftPhero >= rightPhero) targetDir = left;
                    else targetDir = right;
                    break;
                case AntState.GoingHome:
                    TimedPosition oldestPos = null;

                    foreach (TimedPosition pos in previousPositions)
                    {
                        if ((pos.Position - transformProxy.position).sqrMagnitude < memoryRange * memoryRange)
                        {
                            oldestPos = pos;
                            break;
                        }
                    }
                    targetDir = (oldestPos.Position - transformProxy.position);
                    break;
            }
            return targetDir;
        }

        Vector3 GetDryPathDir(TransformProxy transformProxy)
        {
            // Obstacle Avoidance
            var right = Quaternion.Euler(0, 30, 0) * transformProxy.forward * 0.3f;
            var left = Quaternion.Euler(0, -30, 0) * transformProxy.forward * 0.3f;

            RaycastHit hit;
            if (Physics.Raycast(transformProxy.position + left + Vector3.up, -Vector3.up, out hit) && hit.collider.gameObject.layer == 4)
            {
                return -left;
            }
            else if (Physics.Raycast(transformProxy.position + right + Vector3.up, -Vector3.up, out hit) && hit.collider.gameObject.layer == 4)
            {
                return -right;
            }
            else
            {
                return Vector3.zero;
            }
        }

        void MarkPath(TransformProxy transformProxy)
        {
            if (previousMark == null || (previousMark - transformProxy.position).sqrMagnitude > markingRange * markingRange)
            {
                previousMark = transformProxy.position;

                switch (state)
                {
                    case AntState.Wandering:
                        this.previousPositions.Add(new TimedPosition(transformProxy.position));
                        break;
                    case AntState.GoingHome:
                        GameManager.gameState.pheromonesMap.Mark(transformProxy.position);
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

        public GameObject PickTarget(TransformProxy transformProxy)
        {
            float? minDist = null;
            GameObject closest = null;
            foreach (var item in targets)
            {
                if (item == null) continue;
                if (item.GetComponent<Exit>() && state == AntState.Wandering) continue;
                if (item.GetComponent<Resource>() && Shipement != null) continue;
                if (minDist == null || (transformProxy.position - item.transform.position).sqrMagnitude < minDist)
                {
                    minDist = (transformProxy.position - item.transform.position).sqrMagnitude;
                    closest = item;
                }
            }
            return closest;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Interactable>())
            {
                targets.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Interactable>())
            {
                targets.Remove(other.gameObject);
            }
        }

        public MonoBehaviourData Save()
        {
            MonoBehaviourData data = new MonoBehaviourData("Ant", new Proxies.TransformProxy(transform));
            data.properties.Add("PreviousPosition", previousPositions);
            if (Shipement)
            {
                data.properties.Add("Shipement", Shipement.GetComponent<Carryable>().prefabName);
            }
            return data;
        }

        public void Load(MonoBehaviourData data)
        {
            if (data.properties.ContainsKey("PreviousPosition"))
            {
                previousPositions = data.properties["PreviousPosition"] as List<TimedPosition>;
            }
            if (data.properties.ContainsKey("Shipement"))
            {
                Shipement = Instantiate(
                    Resources.Load(data.properties["Shipement"] as string) as GameObject,
                    loadPos.position,
                    loadPos.rotation);
            }
            GameManager.antInstances.Add(this);
        }

        public void Expunge()
        {
            GameManager.antInstances.Remove(this);
        }
    }
}
