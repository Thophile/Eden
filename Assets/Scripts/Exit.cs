using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Interactable
{
    public Colony colony;
    public override void Interact(Ant ant){
        GameState.current.food += ant.Load.GetComponent<FoodPiece>().foodValue;
        colony.DespawnAnt(ant.gameObject);
    }
}
