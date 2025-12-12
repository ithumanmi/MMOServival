using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace Hawkeye.SaveData
{
    public class Saver<T> where T : SaveDataBase, new()
    {
        public string userId;
        public string path;

        public Saver(string userId, string path)
        {
            this.userId = userId;
            this.path = path;
        }

        public void SaveData(T data)
        {
            var key = SaveDataUtil.BuildKey(userId, path);
            var value = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(key, value);
        }
    }

    public class SaveDataManager : MonoSingleton<SaveDataManager>
    {
        private string userId = "demo";

        private List<SaveDataLoader> data = new List<SaveDataLoader>();

        private Dictionary<string, SaveDataBase> dictData;

        public void LoadData(string userId)
        {
            this.userId = userId;

            PrepareListData();

            PrepareDictData();
        }

        private void PrepareListData()
        {
            var derivedTypes = TypeUtilities.FindAllDerivedTypesInDomain<SaveDataBase>();
            foreach (var type in derivedTypes)
            {
                Type loaderType = typeof(SaveDataLoader<>).MakeGenericType(type);

                SaveDataLoader loader = (SaveDataLoader)Activator.CreateInstance(loaderType, userId, type.Name.ToLower());
                data.Add(loader);
            }
        }

        private void PrepareDictData()
        {
            this.dictData = new Dictionary<string, SaveDataBase>();

            foreach (var data in this.data)
            {
                var saveData = data.LoadData();

                this.dictData.Add(saveData.Path, saveData);
            }
        }

        public T GetData<T>() where T : SaveDataBase
        {
            var key = typeof(T).Name.ToLower();

            if (this.dictData.ContainsKey(key) == false)
            {
                return default(T);
            }

            return this.dictData[key] as T;
        }

    }
}
