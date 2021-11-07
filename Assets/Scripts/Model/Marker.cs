using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Marker
    {
        public float intensity = 1f;

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

        public Marker(Vector3 position)
        {
            this.Position = position;
        }

        public void Decay(float decayFactor)
        {
            this.intensity *= decayFactor;
        }
    }
}