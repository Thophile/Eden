using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class PreviousPosition
    {

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
        public float time;

        public PreviousPosition(Vector3 position, float time)
        {
            this.Position = position;
            this.time = time;
        }

        public PreviousPosition(Vector3 position)
        {
            this.Position = position;
            this.time = GameManager.gameState.gameTime;
        }
    }
}