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
                switch (ant.Load.GetComponent<Carryable>().type){
                    case CarryableType.Food:
                        GameState.current.food += ant.Load.GetComponent<Carryable>().value;
                        break;

                }
            }
        } 
        colony.DespawnAnt(ant.gameObject);
            
    }
}