using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    static string savePath = "/GameState.cln";
    public int autoSaveTime;
    float time = 0;


    void Update(){
        if(!UserInterface.isGamePaused){
            time += Time.deltaTime;
            if (Options.autoSave && time > autoSaveTime){
                time -= autoSaveTime;
                Save();
            }
        }
    }

    public static void Save() {
        if (GameState.current.antsPos.Count > 0 ) GameState.current.antsPos.Clear();
        foreach (var item in GameObject.Find("Colony").GetComponent<Colony>().antsOut)
            {
                GameState.current.antsPos.Add(new float[] {item.transform.position.x, item.transform.position.y, item.transform.position.z, item.transform.rotation.eulerAngles.x, item.transform.rotation.eulerAngles.y, item.transform.rotation.eulerAngles.z});
            }


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create (Application.persistentDataPath + savePath);
        bf.Serialize(file, GameState.current);
        file.Close();
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
        foreach (var ar in GameState.current.antsPos)
        {
            GameObject.Find("Colony").GetComponent<Colony>().SpawnAnt(new Vector3(ar[0],ar[1],ar[2]), Quaternion.Euler(ar[3], ar[4], ar[5]));
        }        

    }

    public static void Reset(){
        File.Delete(Application.persistentDataPath + savePath);
        GameState.current = null;
        foreach (var item in GameObject.Find("Colony").GetComponent<Colony>().antsOut)
        {
            Destroy(item);
        } 
        GameObject.Find("Colony").GetComponent<Colony>().antsOut.Clear();

    }

    void OnApplicationQuit()
    {
        Save();
    }
}
