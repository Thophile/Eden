using Assets.Scripts.MonoBehaviours;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Proxies
{
    [System.Serializable]
    public class TransformProxy
    {
        float[] array;

        public TransformProxy(Transform transform)
        {
            Init(transform);
        }

        public void Init(Transform transform)
        {
            this.array = new float[]
            {
                transform.position.x,
                transform.position.y,
                transform.position.z,
                transform.rotation.x,
                transform.rotation.y,
                transform.rotation.z,
                transform.rotation.w
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