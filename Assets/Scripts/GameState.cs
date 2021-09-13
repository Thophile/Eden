using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public static GameState current = null;

    public int food;
    public float gameTime;
    public int antNb;
    public List<object[]> antsInfo;
    public List<object[]> foodsInfo;
    public PheromonesMap pheromonesMap;

    public GameState() {
        food = 0;
        gameTime = 0f;
        antNb = 200;
        antsInfo = new List<object[]>();
        foodsInfo = new List<object[]>();
        pheromonesMap = new PheromonesMap();
    }
}
