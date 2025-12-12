using System;
using System.Collections.Generic;

namespace Hawky.Config
{
    public class ConfigManager : RuntimeSingleton<ConfigManager>, IAwakeBehaviour
    {
        private Dictionary<string, ConfigDataBase> dictData;

        private void PrepareData()
        {
            this.dictData = new Dictionary<string, ConfigDataBase>();

            var derivedTypes = TypeUtilities.FindAllDerivedTypesInDomain<IConfigDataLoader>();

            foreach (var type in derivedTypes)
            {
                var instance = (IConfigDataLoader)Activator.CreateInstance(type);

                var config = instance.LoadConfig();

                if (config == null)
                {
                    UnityEngine.Debug.LogError($"Config Error {type.Name}");
                }
                else
                {
                    this.dictData.Add(GetName(config.GetType()), config);
                }
            }
        }

        public T GetData<T>() where T : ConfigDataBase
        {
            var key = GetName(typeof(T));

            if (this.dictData.TryGetValue(key, out var data))
            {
                return data as T;
            }
            else
            {
                return null;
            }
        }

        public string GetName(Type type)
        {
            return type.Name.ToLower();
        }

        public void Awake()
        {
            PrepareData();
        }
    }
}
