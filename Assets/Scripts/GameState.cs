using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public static GameState current = null;

    public int food;
    public int antNb;
    public List<float[]> antsPos;

    public GameState() {
        food = 0;
        antNb = 100;
        antsPos = new List<float[]>();
    }
}
