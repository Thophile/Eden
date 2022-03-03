using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    [System.Serializable]
    public class Zone
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

    class ProbabilityComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((Zone)x).probability - ((Zone)y).probability;
        }
    }
}