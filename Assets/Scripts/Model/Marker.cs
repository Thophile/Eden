using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Marker
    {
        public Vector3 Values
        {
            get
            {
                return new Vector3(wander, resource, repel);
            }
            set
            {
                wander = value.x;
                resource = value.y;
                repel = value.z;
            }
        }
        public float wander;
        public float resource;
        public float repel;

        public Vector3 Position
        {
            get
            {
                return new Vector3(x, y, z);
            }
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }
        public float x;
        public float y;
        public float z;

        public Marker(Vector3 position, MarkerType type)
        {
            this.Position = position;
            switch (type)
            {
                case MarkerType.Wander:
                    wander = 1;
                    break;
                case MarkerType.Resource:
                    resource = 1;
                    break;
                case MarkerType.Repel:
                    repel = 1;
                    break;
            }
        }

        public void Decay(float decayFactor)
        {
            this.Values = this.Values * decayFactor;
        }
    }
}