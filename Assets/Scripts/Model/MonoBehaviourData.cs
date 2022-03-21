using Assets.Scripts.Proxies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class MonoBehaviourData
    {
        public MonoBehaviourData(string prefab, TransformProxy transform, Dictionary<string, object> properties = null)
        {
            this.prefab = prefab;
            this.transform = transform;
            this.properties = properties != null ? properties : new Dictionary<string, object>();
        }

        public string prefab;
        public TransformProxy transform;
        public Dictionary<string, object> properties;
    }
}