using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class StaticRb : MonoBehaviour
    {
        // Make static rb after being to sleep once
        public bool isStatic = false;

        void Update(){
            if(gameObject.GetComponent<Rigidbody>().IsSleeping()){
                isStatic = true;
                gameObject.GetComponent<Rigidbody>().isKinematic=true;
            }
        }
    }
}
