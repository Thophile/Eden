using Assets.Scripts.MonoBehaviours;
using Assets.Scripts.Utils;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class AntProxy
    {
        float[] array;
        public RaycastHit climbCheckL;
        public RaycastHit climbCheckR;

        public RaycastHit surfaceNormalCheck;

        public RaycastHit DryPathCheckL;
        public RaycastHit DryPathCheckR;
        public AntProxy(Ant ant)
        {
            Init(ant);

        }

        public void Init(Ant ant)
        {
            //ClimbCheck
            Physics.Raycast(ant.transform.position - ant.transform.up * 0.01f, Quaternion.AngleAxis(-5, ant.transform.up) * ant.transform.forward, out this.climbCheckL, ant.climbDist, ~7);
            Physics.Raycast(ant.transform.position - ant.transform.up * 0.01f, Quaternion.AngleAxis(5, ant.transform.up) * ant.transform.forward, out this.climbCheckR, ant.climbDist, ~7);

            //SurfaceNormalCheck
            Physics.Raycast(ant.transform.position - ant.transform.up * 0.02f + ant.transform.forward * 0.01f, Quaternion.Euler(15, 0, 0) * -ant.transform.up, out this.surfaceNormalCheck, Mathf.Infinity, ~7);

            //DryPathCheck
            var right = Quaternion.Euler(0, 30, 0) * ant.transform.forward * 0.3f;
            var left = Quaternion.Euler(0, -30, 0) * ant.transform.forward * 0.3f;
            Physics.Raycast(ant.transform.position + left + Vector3.up, -Vector3.up, out DryPathCheckL, Mathf.Infinity, ~7);
            Physics.Raycast(ant.transform.position + right + Vector3.up, -Vector3.up, out DryPathCheckR, Mathf.Infinity, ~7);
            

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