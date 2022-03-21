using Assets.Scripts.Model;
using Assets.Scripts.Model.Interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class ObjectManager : MonoBehaviour
    {

        public static void Spawn(MonoBehaviourData data)
        {
            var gameObject = Instantiate(
                Resources.Load(data.prefab) as GameObject,
                data.transform.position,
                data.transform.rotation);


            var component = gameObject.GetComponent<ISaveable>();
                component.Load(data);
        }

        public static void Despawn(GameObject gameObject)
        {
            gameObject.GetComponent<ISaveable>().Expunge();
            Destroy(gameObject);
        }
    }
}