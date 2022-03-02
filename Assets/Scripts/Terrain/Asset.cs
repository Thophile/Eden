using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    [System.Serializable]
    public class Asset
    {
        public string name;
        public GameObject[] prefabs;
        public float radius;
        public float density;
        public int probability;
    }

    class ProbabilityComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((Asset)x).probability - ((Asset)y).probability;
        }
    }
}