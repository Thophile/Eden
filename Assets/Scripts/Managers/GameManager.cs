using Assets.Scripts.Managers;
using Assets.Scripts.Model;
using Assets.Scripts.Model.Interfaces;
using Assets.Scripts.MonoBehaviours;
using Assets.Scripts.Terrain;
using Assets.Scripts.Ui;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isPaused;
    public static GameState gameState = null;
    public static List<Ant> antInstances = new List<Ant>();
    public static List<Ant> antsToUpdate = new List<Ant>();
    public int autoSaveTime;
    public World world;

    float lastSaveTime = 0f;

    protected float pheroDecayTimer = 0f;
    protected readonly float pheroDecayDelay = 0.5f;

    void Start(){
        world.BuildWorld();
        Load();
        StartCoroutine(nameof(UpdateAnts));
        isPaused = false;
    }

    void Update(){
        if(!isPaused){
            gameState.gameTime += Time.deltaTime;
            if (Options.autoSave && gameState.gameTime - lastSaveTime > autoSaveTime){
                lastSaveTime = gameState.gameTime;
                Save();
            }

            pheroDecayTimer += Time.deltaTime;
            if(pheroDecayTimer > pheroDecayDelay){
                pheroDecayTimer -= pheroDecayDelay;
                gameState.pheromonesMap.DecayMarkers();
            }
        }

    }

    public IEnumerator UpdateAnts(){
        while (true)
        {
            Debug.Log(antsToUpdate.Count);
            foreach (Ant ant in antsToUpdate)
            {
                if (ant)
                {
                    ant.UpdateSelf();
                }
            }
            antsToUpdate.Clear();
            yield return null;
        }

    }

    public static void Save() {

        gameState.monobehaviours.Clear();
        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        foreach (ISaveable saveable in saveables)
        {
            gameState.monobehaviours.Add(saveable.Save());
        }
        Debug.Log(gameState.monobehaviours.Count);
        SaveManager.SaveGame();
    }

    public static void Load() {
        Debug.Log(gameState.monobehaviours.Count);
        foreach (MonoBehaviourData data in gameState.monobehaviours)
        {
            ObjectManager.Spawn(data);
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
