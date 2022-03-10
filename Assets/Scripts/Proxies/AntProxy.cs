using Assets.Scripts.Model;
using Assets.Scripts.MonoBehaviours;
using UnityEngine;

namespace Assets.Scripts.Proxies
{
    public class AntProxy
    {
        TransformProxy transform;
        float[] array;
        public bool climbCheckL;
        public Vector3 climbNormalL;
        public bool climbCheckR;
        public Vector3 climbNormalR;


        public bool surfaceCheck;
        public Vector3 surfaceNormal;

        public bool DryPathCheckL;
        public int DryPathLayerL;
        public Vector3 DryPathVectorL;

        public bool DryPathCheckR;
        public int DryPathLayerR;
        public Vector3 DryPathVectorR;

        public GameObject target = null;
        public Interactable targetInteractable;
        public Vector3 targetPosition;

        public Vector2 random;

        public AntProxy(Ant ant)
        {
            this.transform = new TransformProxy(ant.transform);
            Init(ant);
        }

        public void Init(Ant ant)
        {
            transform.Init(ant.transform);

            RaycastHit hit;
            //ClimbCheck
            climbCheckL = Physics.Raycast(transform.position - transform.up * 0.01f, Quaternion.AngleAxis(-5, transform.up) * transform.forward, out hit, ant.climbDist, ~7);
            climbNormalL = hit.normal;
            if (!climbCheckL)
            {
                climbCheckR = Physics.Raycast(transform.position - transform.up * 0.01f, Quaternion.AngleAxis(5, transform.up) * transform.forward, out hit, ant.climbDist, ~7);
                climbNormalR = hit.normal;
            }

            //SurfaceNormalCheck
            surfaceCheck = Physics.Raycast(transform.position - transform.up * 0.02f + transform.forward * 0.01f, Quaternion.Euler(15, 0, 0) * -transform.up, out hit, Mathf.Infinity, ~7);
            surfaceNormal = hit.normal;

            //DryPathCheck
            DryPathVectorL = Quaternion.Euler(0, -30, 0) * transform.forward * 0.3f;
            DryPathCheckL = Physics.Raycast(transform.position + DryPathVectorL + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, ~7);
            DryPathLayerL = hit.collider.gameObject.layer;

            if (!DryPathCheckL)
            {
                DryPathVectorR = Quaternion.Euler(0, 30, 0) * transform.forward * 0.3f;
                DryPathCheckR = Physics.Raycast(transform.position + DryPathVectorR + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, ~7);
                DryPathLayerR = hit.collider.gameObject.layer;
            }

            //Targeting
            target = ant.PickTarget(transform);
            targetPosition = target ? target.transform.position : default(Vector3);

            //Random
            random = Random.insideUnitCircle;
        }

        public Vector3 position
        {
            get
            {
                return transform.position;
            }
        }

        public Quaternion rotation
        {
            get
            {
                return transform.rotation;
            }
        }

        public Vector3 up
        {
            get
            {
                return transform.up;
            }
        }

        public Vector3 forward
        {
            get
            {
                return transform.forward;
            }
        }
    }
}
