using Assets.Scripts.Model;
using Assets.Scripts.MonoBehaviours;
using Assets.Scripts.Ui;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class WorldManager : MonoBehaviour
{
    static string savePath = "/GameState.cln";
    public static bool isPaused = true;
    public static GameState gameState = null;
    public static List<Ant> activeAnts = new List<Ant>();
    public int autoSaveTime;

    float lastSaveTime = 0f;

    float pheroDecayTimer = 0f;
    float pheroDecayDelay = 1f;



    void Start(){
        StartCoroutine("UpdateAnts");
    }

    void Update(){
        if(!UserInterface.WorldManagerisPaused){
            WorldManager.gameState.gameTime += Time.deltaTime;
            if (Options.autoSave && WorldManager.gameState.gameTime - lastSaveTime > autoSaveTime){
                lastSaveTime = WorldManager.gameState.gameTime;
                Save();
            }

            pheroDecayTimer += Time.deltaTime;
            if(pheroDecayTimer > pheroDecayDelay){
                pheroDecayTimer -= pheroDecayDelay;
                WorldManager.gameState.pheromonesMap.decayMarkers();
            }
        }

    }

    public IEnumerator UpdateAnts(){
        Stopwatch watch = new Stopwatch();
        int MAX_MILLIS = 3;
        watch.Start();
        for(int i = 0;; i++){
            if (!UserInterface.WorldManagerisPaused) {
                if (watch.ElapsedMilliseconds > MAX_MILLIS) {
                    watch.Reset();
                    yield return null;
                    watch.Start();
                }
                if(i > activeAnts.Count - 1) {
                    i = -1;
                } else if(activeAnts[i] != null) {
                    activeAnts[i].UpdateSelf();
                    UnityEngine.Debug.Log("Updating ant n° " + i);

                }
            } else {
                yield return null;
            }
        }       

    }

    public static void Save() {
        if(WorldManager.gameState != null){
            if (WorldManager.gameState.antsInfo.Count > 0 ) WorldManager.gameState.antsInfo.Clear();
            if(AntSpawner.antsInfo.Count >0 ){
                foreach (var item in AntSpawner.antsInfo)
                {
                    WorldManager.gameState.antsInfo.Add(new object[] {
                        item.transform.position.x,
                        item.transform.position.y,
                        item.transform.position.z,
                        item.transform.rotation.eulerAngles.x,
                        item.transform.rotation.eulerAngles.y,
                        item.transform.rotation.eulerAngles.z,
                        item.GetComponent<Ant>().prefabName,
                        item.GetComponent<Ant>().Load == null ? null : item.GetComponent<Ant>().Load.GetComponent<Carryable>().prefabName
                        });
                }
            }

            if (WorldManager.gameState.resourceInfo.Count > 0 ) WorldManager.gameState.resourceInfo.Clear();
            if(ResourceSpawner.resourceInfo.Count > 0 ){
                foreach (var item in ResourceSpawner.resourceInfo)
                {
                    WorldManager.gameState.resourceInfo.Add(new object[] {
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


            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create (Application.persistentDataPath + savePath);
            bf.Serialize(file, WorldManager.gameState);
            file.Close();

        }
    }

    public static void Load() {
        if(File.Exists(Application.persistentDataPath + savePath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + savePath, FileMode.Open);
            WorldManager.gameState = (GameState)bf.Deserialize(file);
            file.Close();
        }else{
            WorldManager.gameState = new GameState();
            Save();

        }

        // BuildWorld
        foreach (var ar in WorldManager.gameState.antsInfo)
        {
            AntSpawner.SpawnAnt(
                new Vector3((float)ar[0],(float)ar[1],(float)ar[2]),
                Quaternion.Euler((float)ar[3], (float)ar[4], (float)ar[5]),
                Resources.Load((string)ar[6]) as GameObject,
                Resources.Load((string)ar[7]) as GameObject
            );
        }      
        foreach (var ar in WorldManager.gameState.resourceInfo)
        {
            ResourceSpawner.SpawnResource(
                new Vector3((float)ar[0],(float)ar[1],(float)ar[2]),
                Quaternion.Euler((float)ar[3], (float)ar[4], (float)ar[5]),
                Resources.Load((string)ar[6]) as GameObject,
                (int) ar[7]
            );
        }   
        

    }

    public static void Reset(){
        File.Delete(Application.persistentDataPath + savePath);
        WorldManager.gameState = null;
        AntSpawner.antsInfo.Clear();
        activeAnts.Clear();
        ResourceSpawner.resourceInfo.Clear();
        var keepAlives = GameObject.FindObjectsOfType<KeepAlive>();
        foreach (var item in keepAlives)
        {
            if(item.baseSceneName == SceneManager.GetActiveScene().name) Destroy(item.gameObject);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
