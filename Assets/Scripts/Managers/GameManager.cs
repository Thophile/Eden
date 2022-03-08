using Assets.Scripts.Managers;
using Assets.Scripts.Model;
using Assets.Scripts.MonoBehaviours;
using Assets.Scripts.Terrain;
using Assets.Scripts.Ui;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool isPaused;
    public static GameState gameState = null;
    public static List<Ant> activeAnts = new List<Ant>();
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
        Stopwatch watch = new Stopwatch();
        int MAX_MILLIS = 3;
        watch.Start();
        for(int i = 0;; i++){
            if (!isPaused) {
                if (watch.ElapsedMilliseconds > MAX_MILLIS) {
                    watch.Reset();
                    yield return null;
                    watch.Start();
                }
                if(i > activeAnts.Count - 1) {
                    i = -1;
                } else if(activeAnts[i] != null) {
                    activeAnts[i].UpdateSelf();
                    //UnityEngine.Debug.Log("Updating ant n° " + i);

                }
            } else {
                yield return null;
            }
        }       

    }

    public static void Save() {
        if(gameState != null){
            if (gameState.antsInfo.Count > 0 ) gameState.antsInfo.Clear();
            if(AntSpawner.antsInfo.Count >0 ){
                foreach (var item in AntSpawner.antsInfo)
                {
                    if(item != null)
                    {
                        List<object[]> previousPositions = new List<object[]>();
                        foreach(var pos in item.GetComponent<Ant>().previousPositions)
                        {
                            previousPositions.Add(new object[] {
                                pos.x,
                                pos.y,
                                pos.z,
                                pos.time
                            });
                        }


                        gameState.antsInfo.Add(new object[] {
                            item.transform.position.x,
                            item.transform.position.y,
                            item.transform.position.z,
                            item.transform.rotation.eulerAngles.x,
                            item.transform.rotation.eulerAngles.y,
                            item.transform.rotation.eulerAngles.z,
                            item.GetComponent<Ant>().prefabName,
                            item.GetComponent<Ant>().Load == null ? null : item.GetComponent<Ant>().Load.GetComponent<Carryable>().prefabName,
                            previousPositions
                            });
                    }
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
                camera.GetComponent<CameraController>().zoomLevel
            };

            SaveManager.SaveGame();
        }
    }

    public static void Load() {
        // BuildWorld
        AntSpawner.antsInfo.Clear();
        foreach (var ar in gameState.antsInfo)
        {
            List<TimedPosition> previousPositions = new List<TimedPosition>();
            foreach(var e in ar[8] as List<object[]>)
            {
                previousPositions.Add(
                    new TimedPosition(
                        new Vector3((float)e[0], (float)e[1], (float)e[2]),
                        (float)e[3])
                    );
            }
            AntSpawner.SpawnAnt(
                new Vector3((float)ar[0],(float)ar[1],(float)ar[2]),
                Quaternion.Euler((float)ar[3], (float)ar[4], (float)ar[5]),
                Resources.Load((string)ar[6]) as GameObject,
                Resources.Load((string)ar[7]) as GameObject,
                previousPositions

            );
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
            camera.GetComponent<CameraController>().zoomLevel = (float)gameState.cameraInfo[7];
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
