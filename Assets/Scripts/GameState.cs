using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public static GameState current;

    public int food;

    public GameState() {
        food = 0;
    }
}
