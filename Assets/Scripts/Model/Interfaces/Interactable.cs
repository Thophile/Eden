using Assets.Scripts.MonoBehaviours;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class Interactable : MonoBehaviour
    {
        public virtual void Interact(Ant ant){
            ant.state = AntState.Still;
            return;
        }
    }
}
