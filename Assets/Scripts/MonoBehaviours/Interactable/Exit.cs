using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Interactable
{
    public Colony colony;
    public override void Interact(Ant ant){
        base.Interact(ant);
        if(ant.Load != null){
            if(ant.Load.GetComponent<Carryable>()){
                GameState.current.resourceValue += ant.Load.GetComponent<Carryable>().resourceValue;
            }
        } 
        colony.DespawnAnt(ant.gameObject);
            
    }
}