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

public class GameManager : MonoBehaviour
{
    static readonly string savePath = "/GameState.cln";
    public static bool isPaused = true;
    public static GameState gameState = null;
    public static List<Ant> activeAnts = new List<Ant>();
    public int autoSaveTime;

    float lastSaveTime = 0f;

    protected float pheroDecayTimer = 0f;
    protected readonly float pheroDecayDelay = 0.5f;



    void Start(){
        StartCoroutine(nameof(UpdateAnts));
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
                    UnityEngine.Debug.Log("Updating ant n° " + i);

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


            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create (Application.persistentDataPath + savePath);
            bf.Serialize(file, gameState);
            file.Close();

        }
    }

    public static void Load() {
        if(File.Exists(Application.persistentDataPath + savePath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + savePath, FileMode.Open);
            gameState = (GameState)bf.Deserialize(file);
            file.Close();
        }else{
            gameState = new GameState();
            Save();

        }

        // BuildWorld
        foreach (var ar in gameState.antsInfo)
        {
            List<PreviousPosition> previousPositions = new List<PreviousPosition>();
            foreach(var e in ar[8] as List<object[]>)
            {
                previousPositions.Add(
                    new PreviousPosition(
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
        foreach (var ar in gameState.resourceInfo)
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
        gameState = null;
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
