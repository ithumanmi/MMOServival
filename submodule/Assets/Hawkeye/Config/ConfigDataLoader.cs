using Newtonsoft.Json;
using UnityEngine;
using System;

namespace Hawkeye.Config
{
    public interface IConfigDataLoader
    {
        ConfigDataBase LoadConfig();
    }

    public abstract class ConfigDataLoader<T> : IConfigDataLoader where T : ConfigDataBase
    {
        protected abstract string ResourcesPath();

        public ConfigDataBase LoadConfig()
        {
            var textAssets = Resources.Load<TextAsset>(ResourcesPath());

            return JsonConvert.DeserializeObject<T>(textAssets.text);
        }
    }
}
