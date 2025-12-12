using Newtonsoft.Json;
using UnityEngine;

namespace Hawky.SaveData
{
    public abstract class SaveDataLoader
    {
        public abstract SaveDataBase LoadData();
    }

    public class SaveDataLoader<T> : SaveDataLoader where T : SaveDataBase, new()
    {
        public string userId;
        public string path;
        public SaveDataLoader(string userId, string path)
        {
            this.userId = userId;
            this.path = path;
        }

        public override SaveDataBase LoadData()
        {
            var key = SaveDataUtil.BuildKey(userId, path);

            if (PlayerPrefs.HasKey(key))
            {
                var value = PlayerPrefs.GetString(key);
                var rs = JsonConvert.DeserializeObject<T>(value);
                rs.Path = path;
                rs.UserId = userId;

                rs.OnLoad();

                return rs;
            }
            else
            {
                var rs = new T();

                rs.Path = path;
                rs.UserId = userId;

                rs.Default();
                rs.OnLoad();
                rs.Save();
                return rs;
            }
        }
    }
}
