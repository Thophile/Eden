using Assets.Scripts.Model;
using Assets.Scripts.MonoBehaviours;
using Assets.Scripts.Proxies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [System.Serializable]
    public class AntData
    {
        public AntData(Ant ant)
        {
            transform = new TransformProxy(ant.transform);
            previousPositions = ant.previousPositions;
            shipement = ant.Shipement ? ant.Shipement.GetComponent<Carryable>().prefabName : null;
        }
        public AntData(Vector3 position, Quaternion rotation)
        {
            var tmp = new GameObject().transform;
                tmp.SetPositionAndRotation(position, rotation);
            transform = new TransformProxy(tmp);
            previousPositions = null;
            shipement = null;
        }

        public static string prefab = "Ant";
        public TransformProxy transform;
        public List<TimedPosition> previousPositions;
        public string shipement;
    }
}