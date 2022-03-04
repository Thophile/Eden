using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    public class SaveManager
    {
        public static readonly string fileExtension = ".eden";
        public static readonly string basePath = Application.persistentDataPath + "/Saves/";
        public static string currentSavePath = basePath + GetFileName("New Game");

        public static string GetFileName(string verboseName)
        {
            return verboseName.Replace(" ", "_") + fileExtension;
        }

        public static string GetVerboseName(string filename)
        {
            return filename.Replace("_", " ").Replace(fileExtension, "");
        }

        public static List<string> GetSavesList()
        {
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            var savesList = new List<string>();
            var filesInfo = new DirectoryInfo(basePath).GetFiles();

            foreach (var file in filesInfo)
            {
                savesList.Add(file.Name);
            }
            return savesList;
        }

        public static void SaveGame()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(currentSavePath);
            bf.Serialize(file, GameManager.gameState);
            file.Close();
        }

        public static void LoadGame(string name)
        {
            currentSavePath = basePath + name;
            BinaryFormatter bf = new BinaryFormatter();
            
            if (!File.Exists(currentSavePath))
            {
                GameManager.gameState = new GameState();
                SaveGame();
            } else {
                FileStream file = File.Open(currentSavePath, FileMode.Open);
                GameManager.gameState = (GameState)bf.Deserialize(file);
                file.Close();
            }
            Debug.Log(name);
            SceneManager.LoadScene(1);
        }

    }
}