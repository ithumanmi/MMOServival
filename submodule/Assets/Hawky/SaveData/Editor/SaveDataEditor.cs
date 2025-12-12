using UnityEditor;
using UnityEngine;

namespace Hawky.SaveData.Editor
{
    public class SaveDataEditor
    {
        [MenuItem("Hawky/Save Data/Delete All")]
        public static void ShowSaveData()
        {
            PlayerPrefs.DeleteAll();

            Debug.Log($"Deleted All DataBase");
        }
    }
}