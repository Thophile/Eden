using Assets.Scripts.Model;

namespace Assets.Scripts.MonoBehaviours
{
    public class Exit : Interactable
    {
        public AntSpawner antSpawner;
        public override void Interact(Ant ant){
            base.Interact(ant);
            if(ant.Load != null){
                if(ant.Load.GetComponent<Carryable>()){
                    WorldManager.gameState.resources += ant.Load.GetComponent<Carryable>().resourceValue;
                }
            } 
            antSpawner.DespawnAnt(ant.gameObject);
            
        }
    }
}