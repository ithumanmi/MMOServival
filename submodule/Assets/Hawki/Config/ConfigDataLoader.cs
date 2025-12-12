using Newtonsoft.Json;
using UnityEngine;

namespace Hawki.Config
{
    public interface IConfigDataLoader
    {
        ConfigDataBase LoadConfig();
    }

    public abstract class ConfigDataLoader<T> : IConfigDataLoader where T : ConfigDataBase, new ()  
    {
        protected abstract string ResourcesPath();

        public ConfigDataBase LoadConfig()
        {
            var path = ResourcesPath();

            var textAssets = Resources.Load<TextAsset>(path);

            if (textAssets == null)
            {
                Debug.Log($"Không có Config tại {path} => return default");
                return new T();
            }
            return JsonConvert.DeserializeObject<T>(textAssets.text);
        }
    }
}
