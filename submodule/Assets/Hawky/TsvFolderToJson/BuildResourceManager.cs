using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hawky.TSVtoJSON
{
    public class BuildResourceManager
    {
        public static void ExportConfig(string tsvFolderPath, string jsonFilePath)
        {
            var listOfData = new Dictionary<string, List<Dictionary<string, string>>>();

            var directoryInfo = new DirectoryInfo(tsvFolderPath);
            var listFolder = directoryInfo.GetDirectories();
            foreach (var info in listFolder)
            {
                var fileTsv = info.GetFiles();
                foreach (var fileInfo in fileTsv)
                {
                    if (fileInfo.Extension != ".tsv")
                    {
                        continue;
                    }

                    var fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
                    var lines = File.ReadAllLines(fileInfo.FullName);
                    BuildObject(listOfData, fileName, lines);
                }
            }

            var rootFile = directoryInfo.GetFiles();
            foreach (var fileInfo in rootFile)
            {
                if (fileInfo.Extension != ".tsv")
                {
                    continue;
                }

                var fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
                var lines = File.ReadAllLines(fileInfo.FullName);
                BuildObject(listOfData, fileName, lines);
            }

            var directoryPath = Path.GetDirectoryName(jsonFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var jsonString = JsonConvert.SerializeObject(listOfData);

            using (var sw = File.CreateText(jsonFilePath))
            {
                sw.Write(jsonString);
            }
        }

        public static void BuildObject(Dictionary<string, List<Dictionary<string, string>>> listData, string name, string[] tsvLines)
        {
            var properties = tsvLines[0].Split('\t');

            var listObjResult = new List<Dictionary<string, string>>();

            for (var i = 1; i < tsvLines.Length; i++)
            {

                var lineData = tsvLines[i].Split('\t');
                var objResult = new Dictionary<string, string>();
                for (var j = 0; j < properties.Length; j++)
                {
                    if (objResult.ContainsKey(properties[j]))
                    {
                        Debug.LogError("con tainkey " + properties[j]);
                    }
                    if (lineData.Length > j)
                    {
                        objResult.Add(properties[j], lineData[j]);
                    }
                    else
                    {
                        objResult.Add(properties[j], lineData[0]);
                    }
                }
                listObjResult.Add(objResult);
            }

            listData.Add(name, listObjResult);
        }
    }
}
