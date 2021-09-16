using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public static GameState current = null;

    public float gameTime;
    public int antNb;
    public List<object[]> antsInfo;
    public List<object[]> resourceInfo;
    public ResourceValue resourceValue;
    public PheromonesMap pheromonesMap;

    public GameState() {
        gameTime = 0f;
        antNb = 200;
        antsInfo = new List<object[]>();
        resourceInfo = new List<object[]>();
        resourceValue = new ResourceValue();
        pheromonesMap = new PheromonesMap();
    }
}

[System.Serializable]
public class ResourceValue{
    public ResourceValue(float protein, float sugar, float fat, float fiber, float dirt){
        this.protein = protein;
        this.sugar = sugar;
        this.fat = fat;
        this.fiber = fiber;
        this.dirt = dirt;
    }
    public ResourceValue(){}
    
    public float protein = 0f;
    public float sugar = 0f;
    public float fat = 0f;
    public float fiber = 0f;
    public float dirt = 0f;

    public static ResourceValue operator *(ResourceValue a, float b) => new ResourceValue(a.protein*b, a.sugar*b, a.fat*b, a.fiber*b, a.dirt*b);
    public static ResourceValue operator *(float b, ResourceValue a) => new ResourceValue(a.protein*b, a.sugar*b, a.fat*b, a.fiber*b, a.dirt*b);

    public static ResourceValue operator *(ResourceValue a, int b) => new ResourceValue(a.protein*b, a.sugar*b, a.fat*b, a.fiber*b, a.dirt*b);
    public static ResourceValue operator *(int b, ResourceValue a) => new ResourceValue(a.protein*b, a.sugar*b, a.fat*b, a.fiber*b, a.dirt*b);

    public override string ToString() => $"{protein}Protein {sugar}Sugar {fat}Fat {fiber}Fiber {dirt}Dirt";

    public static ResourceValue operator +(ResourceValue a,ResourceValue b) => new ResourceValue(a.protein + b.protein, a.sugar + b.sugar, a.fat + b.fat, a.fiber + b.fiber, a.dirt + b.dirt); 

}
