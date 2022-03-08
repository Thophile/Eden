using Assets.Scripts.MonoBehaviours;
using Assets.Scripts.Utils;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class AntProxy
    {
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
            Init(ant);
        }

        public void Init(Ant ant)
        {
            RaycastHit hit;
            //ClimbCheck
            climbCheckL = Physics.Raycast(ant.transform.position - ant.transform.up * 0.01f, Quaternion.AngleAxis(-5, ant.transform.up) * ant.transform.forward, out hit, ant.climbDist, ~7);
            climbNormalL = hit.normal;
            if (!climbCheckL)
            {
                climbCheckR = Physics.Raycast(ant.transform.position - ant.transform.up * 0.01f, Quaternion.AngleAxis(5, ant.transform.up) * ant.transform.forward, out hit, ant.climbDist, ~7);
                climbNormalR = hit.normal;
            }

            //SurfaceNormalCheck
            surfaceCheck = Physics.Raycast(ant.transform.position - ant.transform.up * 0.02f + ant.transform.forward * 0.01f, Quaternion.Euler(15, 0, 0) * -ant.transform.up, out hit, Mathf.Infinity, ~7);
            surfaceNormal = hit.normal;

            //DryPathCheck
            DryPathVectorL = Quaternion.Euler(0, -30, 0) * ant.transform.forward * 0.3f;
            DryPathCheckL= Physics.Raycast(ant.transform.position + DryPathVectorL + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, ~7);
            DryPathLayerL = hit.collider.gameObject.layer;

            if (!DryPathCheckL)
            {
                DryPathVectorR = Quaternion.Euler(0, 30, 0) * ant.transform.forward * 0.3f;
                DryPathCheckR = Physics.Raycast(ant.transform.position + DryPathVectorR + Vector3.up, -Vector3.up, out hit, Mathf.Infinity, ~7);
                DryPathLayerR = hit.collider.gameObject.layer;
            }

            //Targeting
            target = ant.PickTarget();
            targetPosition = target ? target.transform.position : default(Vector3);

            //Random
            random = Random.insideUnitCircle;

            this.array = new float[]
            {
                ant.transform.position.x,
                ant.transform.position.y,
                ant.transform.position.z,
                ant.transform.rotation.x,
                ant.transform.rotation.y,
                ant.transform.rotation.z,
                ant.transform.rotation.w
            };
        }

        public float[] GetArray()
        {
            return array;
        }

        public Vector3 position
        {
            get
            {
                return new Vector3(array[0], array[1], array[2]);
            }
        }

        public Quaternion rotation
        {
            get
            {
                return new Quaternion(array[3], array[4], array[5], array[6]);
            }
        }

        public Vector3 up 
        { 
            get
            {
                return rotation * Vector3.up;
            } 
        }

        public Vector3 forward
        {
            get
            {
                return rotation * Vector3.forward;
            }
        }
    }
}