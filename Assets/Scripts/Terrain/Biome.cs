using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    [System.Serializable]
    public class Biome
    {
        public string name;
        public Asset[] assets;
        public float radius;
        public float density;
        public int probability;
    }

    [System.Serializable]
    public class Asset
    {
        public GameObject prefab;
        public int probability;
    }
}