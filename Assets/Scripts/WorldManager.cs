using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Threading;
using System.Diagnostics;


public class WorldManager : MonoBehaviour
{
    static string savePath = "/GameState.cln";
    public static List<Ant> activeAnts = new List<Ant>();
    public int autoSaveTime;

    float lastSaveTime = 0f;

    float pheroDecayTimer = 0f;
    float pheroDecayDelay = 1f;

    void Start(){
        StartCoroutine("UpdateAnts");
    }

    void Update(){
        if(!UserInterface.isGamePaused){
            GameState.current.gameTime += Time.deltaTime;
            if (Options.autoSave && GameState.current.gameTime - lastSaveTime > autoSaveTime){
                lastSaveTime = GameState.current.gameTime;
                Save();
            }

            pheroDecayTimer += Time.deltaTime;
            if(pheroDecayTimer > pheroDecayDelay){
                pheroDecayTimer -= pheroDecayDelay;
                GameState.current.pheromonesMap.decayMarkers();
            }
        }

    }

    public IEnumerator UpdateAnts(){
        Stopwatch watch = new System.Diagnostics.Stopwatch();
        int MAX_MILLIS = 3;
        watch.Start();
        for(int i = 0;; i++){
            UnityEngine.Debug.Log(i);
            
            if (watch.ElapsedMilliseconds > MAX_MILLIS)
            {
                watch.Reset();
                yield return null;
                watch.Start();
            }
            if(i > activeAnts.Count - 1){
                i = -1;
            }else if(activeAnts[i] != null){
                activeAnts[i].UpdateSelf();
                UnityEngine.Debug.Log("Updating ant n° "+i);

            }
        }       

    }

    public static void Save() {
        if(GameState.current != null){
            if (GameState.current.antsInfo.Count > 0 ) GameState.current.antsInfo.Clear();
            if(Colony.antsInfo.Count >0 ){
                foreach (var item in Colony.antsInfo)
                {
                    GameState.current.antsInfo.Add(new object[] {
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

            if (GameState.current.foodsInfo.Count > 0 ) GameState.current.foodsInfo.Clear();
            if(FoodSpawner.foodsInfo.Count > 0 ){
                foreach (var item in FoodSpawner.foodsInfo)
                {
                    GameState.current.foodsInfo.Add(new object[] {
                        item.transform.position.x,
                        item.transform.position.y,
                        item.transform.position.z,
                        item.transform.rotation.eulerAngles.x,
                        item.transform.rotation.eulerAngles.y,
                        item.transform.rotation.eulerAngles.z,
                        item.GetComponent<Food>().prefabName,
                        item.GetComponent<Food>().health
                        });
                }
            }


            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create (Application.persistentDataPath + savePath);
            bf.Serialize(file, GameState.current);
            file.Close();

        }
    }

    public static void Load() {
        if(File.Exists(Application.persistentDataPath + savePath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + savePath, FileMode.Open);
            GameState.current = (GameState)bf.Deserialize(file);
            file.Close();
        }else{
            GameState.current = new GameState();
            Save();

        }

        // BuildWorld
        foreach (var ar in GameState.current.antsInfo)
        {
            Colony.SpawnAnt(
                new Vector3((float)ar[0],(float)ar[1],(float)ar[2]),
                Quaternion.Euler((float)ar[3], (float)ar[4], (float)ar[5]),
                Resources.Load((string)ar[6]) as GameObject,
                Resources.Load((string)ar[7]) as GameObject
            );
        }      
        foreach (var ar in GameState.current.foodsInfo)
        {
            FoodSpawner.SpawnFood(
                new Vector3((float)ar[0],(float)ar[1],(float)ar[2]),
                Quaternion.Euler((float)ar[3], (float)ar[4], (float)ar[5]),
                Resources.Load((string)ar[6]) as GameObject,
                (int) ar[7]
            );
        }   
        

    }

    public static void Reset(){
        File.Delete(Application.persistentDataPath + savePath);
        GameState.current = null;
        Colony.antsInfo.Clear();
        activeAnts.Clear();
        FoodSpawner.foodsInfo.Clear();
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
