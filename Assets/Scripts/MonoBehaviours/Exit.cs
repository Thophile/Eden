using Assets.Scripts.Model;

namespace Assets.Scripts.MonoBehaviours
{
    public class Exit : Interactable
    {
        public AntSpawner antSpawner;
        public override void Interact(Ant ant){
            base.Interact(ant);
            if(ant.Shipement != null){
                if(ant.Shipement.GetComponent<Carryable>()){
                    GameManager.gameState.resources += ant.Shipement.GetComponent<Carryable>().resourceValue;
                }
            }
            Ant.Despawn(ant);
            
        }
    }
}