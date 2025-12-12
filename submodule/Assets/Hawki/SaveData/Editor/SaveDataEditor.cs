using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hawki.SaveData.Editor
{
    public class SaveDataEditor
    {
        [MenuItem("Hawki/Save Data/Delete All")]
        public static void ShowSaveData()
        {
            PlayerPrefs.DeleteAll();

            Debug.Log($"Deleted All DataBase");
        }
    }
}