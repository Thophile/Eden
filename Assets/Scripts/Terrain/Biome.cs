using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    [System.Serializable]
    public class Biome
    {
        public string name;
        public float steepness;
        public float minHeight;
        public float maxHeight;
        public Color color;
    }
}