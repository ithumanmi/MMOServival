using System;
using System.Collections.Generic;

namespace Hawky.SaveData
{
    public class SaveDataManager : RuntimeSingleton<SaveDataManager>, IAwakeBehaviour, IAllSingletonAwakeComplete
    {
        private string userId = "Default";

        private Dictionary<string, SaveDataBase> dictData;

        public void Load(string userId)
        {
            this.userId = userId;

            InitData();
        }

        private void InitData()
        {
            var derivedTypes = TypeUtilities.FindAllDerivedTypesInDomain<SaveDataBase>();
            var loaders = new List<SaveDataLoader>();
            foreach (var type in derivedTypes)
            {
                Type loaderType = typeof(SaveDataLoader<>).MakeGenericType(type);

                SaveDataLoader loader = (SaveDataLoader)Activator.CreateInstance(loaderType, userId, type.Name.ToLower());
                loaders.Add(loader);
            }

            this.dictData = new Dictionary<string, SaveDataBase>();

            foreach (var data in loaders)
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
                return default;
            }

            return this.dictData[key] as T;
        }

        public void Awake()
        {
            Load("Default");
        }

        public void OnAllSingletonInitComplete()
        {
            foreach (var data in dictData.Values)
            {
                data.FixData();
            }
        }
    }
}
