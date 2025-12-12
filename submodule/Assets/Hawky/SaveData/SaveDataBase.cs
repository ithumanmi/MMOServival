using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace Hawky.SaveData
{
    public abstract class SaveDataBase
    {
        [NonSerialized]
        public string UserId;
        [NonSerialized]
        public string Path;
        public abstract void Save();
        public virtual void Default()
        {
            Debug.Log($"Load Data: {Path}");
        }

        public virtual void FixData()
        {
            Debug.Log($"FixData Data: {Path}");
        }

        public virtual void OnLoad()
        {
            Debug.Log($"Load Data: {Path}");
        }

        public virtual void OnSave()
        {
            Debug.Log($"Save Data: {Path}");
        }
    }

    public abstract class SaveDataBase<T> : SaveDataBase where T : SaveDataBase
    {
        public override void Save()
        {
            OnSave();

            var key = SaveDataUtil.BuildKey(UserId, Path);

            var value = JsonConvert.SerializeObject(this as T);
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();

#if UNITY_EDITOR
            // Ghi log vào file
            LogToFile($"{value}", Path);

            void LogToFile(string message, string path)
            {
                // Tạo tên file dựa trên path
                string sanitizedPath = path.Replace('/', '_').Replace('\\', '_'); // Đảm bảo rằng path không chứa ký tự không hợp lệ cho tên file
                string logFileName = $"log_{sanitizedPath}.txt";

                // Xây dựng đường dẫn hoàn chỉnh
                string relativeLogDir = System.IO.Path.Combine(Application.dataPath, "../../LogSaveData/");
                string absoluteLogDir = System.IO.Path.GetFullPath(relativeLogDir); // Đường dẫn tuyệt đối

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(absoluteLogDir))
                {
                    Directory.CreateDirectory(absoluteLogDir);
                }

                string logFilePath = System.IO.Path.Combine(absoluteLogDir, logFileName);

                using (StreamWriter writer = new StreamWriter(logFilePath, false)) // true để thêm vào file, không ghi đè
                {
                    writer.WriteLine(message);
                }
            }
#endif
        }
    }
}
