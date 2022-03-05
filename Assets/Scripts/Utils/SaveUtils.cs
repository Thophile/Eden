using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public abstract class SaveUtils : MonoBehaviour
    {
        public static readonly string savePath = Application.persistentDataPath + "/Saves/";

        public static string GetFileName(string name)
        {

            return name.Replace(" ", "") + ".cln";
        }
        public List<string> GetSaveList()
        {
            return new List<string>();
        }
    }
}