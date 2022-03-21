using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Interfaces
{
    public interface ISaveable
    {
        public MonoBehaviourData Save();
        public void Load(MonoBehaviourData data);

        public void Expunge();
    }
}