using Assets.Scripts.MonoBehaviours;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Proxies
{
    public class TransformProxy
    {
        float[] array;

        public TransformProxy(Transform ant)
        {
            Init(ant);
        }

        public void Init(Transform ant)
        {
            this.array = new float[]
            {
                ant.position.x,
                ant.position.y,
                ant.position.z,
                ant.rotation.x,
                ant.rotation.y,
                ant.rotation.z,
                ant.rotation.w
            };
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