﻿using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    [System.Serializable]
    public class Biome
    {
        public string name;
        public float maxSteepness;
        public float minHeight;
        public float maxHeight;
        public Color colorOne;
        public Color colorTwo;
    }
}