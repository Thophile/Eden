using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    static string savePath = "/GameState.cln";

    public static void Save() {
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
            New();
        }
        RunGame();        

    }

    public static void New() {
        GameState.current = new GameState();
        Save();
        RunGame();
    }

    public static void RunGame(){
        SceneManager.LoadSceneAsync(1);
    }


    void OnApplicationQuit()
    {
        Save();
    }

}
