using Assets.Scripts.Managers;
using Assets.Scripts.Model;
using Assets.Scripts.Model.Data;
using Assets.Scripts.MonoBehaviours;
using Assets.Scripts.Terrain;
using Assets.Scripts.Ui;
using System.Collections;
using System.Collections.Generic;
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
        if(gameState != null){
            if (gameState.ants.Count > 0 ) gameState.ants.Clear();
            if(GameManager.antInstances.Count >0 ){
                foreach (Ant ant in GameManager.antInstances)
                {
                    gameState.ants.Add(new AntData(ant));
                }
            }

            if (gameState.resourceInfo.Count > 0 ) gameState.resourceInfo.Clear();
            if(ResourceSpawner.resourceInfo.Count > 0 ){
                foreach (var item in ResourceSpawner.resourceInfo)
                {
                    gameState.resourceInfo.Add(new object[] {
                        item.transform.position.x,
                        item.transform.position.y,
                        item.transform.position.z,
                        item.transform.rotation.eulerAngles.x,
                        item.transform.rotation.eulerAngles.y,
                        item.transform.rotation.eulerAngles.z,
                        item.GetComponent<Resource>().prefabName,
                        item.GetComponent<Resource>().health
                        });
                }
            }

            var camera = GameObject.Find("Camera");
            gameState.cameraInfo = new object[]
            {
                camera.transform.position.x,
                camera.transform.position.y,
                camera.transform.position.z,
                camera.transform.rotation.x,
                camera.transform.rotation.y,
                camera.transform.rotation.z,
                camera.transform.rotation.w,
                camera.GetComponent<Controller>().zoomLevel
            };

            SaveManager.SaveGame();
        }
    }

    public static void Load() {
        // BuildWorld
        GameManager.antInstances.Clear();
        foreach (AntData data in gameState.ants)
        {
            Ant.Spawn(data);
        }

        ResourceSpawner.resourceInfo.Clear();
        foreach (var ar in gameState.resourceInfo)
        {
            ResourceSpawner.SpawnResource(
                new Vector3((float)ar[0],(float)ar[1],(float)ar[2]),
                Quaternion.Euler((float)ar[3], (float)ar[4], (float)ar[5]),
                Resources.Load((string)ar[6]) as GameObject,
                (int) ar[7]
            );
        }

        if(gameState.cameraInfo != default(object[]))
        {
            var camera = GameObject.Find("Camera");
            camera.transform.position = new Vector3(
                (float)gameState.cameraInfo[0], 
                (float)gameState.cameraInfo[1], 
                (float)gameState.cameraInfo[2]);
            camera.transform.rotation = new Quaternion(
                (float)gameState.cameraInfo[3], 
                (float)gameState.cameraInfo[4], 
                (float)gameState.cameraInfo[5], 
                (float)gameState.cameraInfo[6]);
            camera.GetComponent<Controller>().zoomLevel = (float)gameState.cameraInfo[7];
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
