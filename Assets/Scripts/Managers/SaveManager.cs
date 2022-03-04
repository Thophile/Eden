using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SaveManager
    {
        public static readonly string fileExtension = ".edn";
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
            var savesList = new List<string>();
            var fileInfo = new DirectoryInfo(basePath).GetFiles();

            foreach (var file in fileInfo)
            {
                savesList.Add(GetVerboseName(file.FullName));
            }
            return savesList;
        }

    }
}