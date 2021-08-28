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
            GameState.current = new GameState();
            Save();

        }
        BuildWorld();        

    }

    public static void Reset(){
        File.Delete(Application.persistentDataPath + savePath);
        GameState.current = null;
    }

    public static void BuildWorld(){
        Debug.Log("Building with" + GameState.current);
    }


    void OnApplicationQuit()
    {
        Save();
    }

}
