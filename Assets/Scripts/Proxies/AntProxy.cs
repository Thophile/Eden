using Assets.Scripts.MonoBehaviours;
using UnityEngine;

namespace Assets.Scripts.Proxies
{
    public class AntProxy
    {
        float[] array;

        public AntProxy(Ant ant)
        {
            Init(ant);
        }

        public void Init(Ant ant)
        {
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
