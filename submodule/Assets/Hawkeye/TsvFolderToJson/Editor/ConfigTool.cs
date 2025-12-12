using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hawkeye.TSVtoJSON.Editor
{
    public class ConfigTool
    {
        public static string ALL_CONFIG_PATH = "Resources/AllConfig.json";

        [MenuItem("Hawkeye/TsvFolderToJson/Export Tsv Folder to Json File")]
        public static void Init()
        {
            var allExportable = TypeUtilities.FindAllDerivedTypesInDomain<IExportable>();

            if (allExportable == null || allExportable.Count() == 0)
            {
                Debug.Log("No classes found that implement IExportable.");
                return;
            }

            foreach (var exportable in allExportable)
            {
                var instance = (IExportable)Activator.CreateInstance(exportable);

                var tsvFolderPath = Path.Combine(instance.FromPath(), instance.FolderName());
                var jsonFilePath = $"{Path.Combine(instance.ToPath(), instance.FileName())}.json";

                try
                {
                    BuildResourceManager.ExportConfig(tsvFolderPath, jsonFilePath);

                    Debug.Log($"Successfully exported TSV folder '{tsvFolderPath}' to JSON file '{jsonFilePath}'.");
                } catch (Exception e)
                {
                    Debug.LogError($"Failed exported TSV folder '{tsvFolderPath}' to JSON file '{jsonFilePath}' with error = {e.Message}");
                    Debug.LogError(e.StackTrace);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
