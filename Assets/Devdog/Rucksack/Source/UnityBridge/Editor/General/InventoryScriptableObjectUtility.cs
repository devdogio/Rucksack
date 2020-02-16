using System;
using UnityEditor;

namespace Devdog.Rucksack.Editor
{
    public static class InventoryScriptableObjectUtility
    {
        public const string PrefabsSaveFolderSaveKey = RucksackConstants.ProductName + "_PrefabSavePath";
        private static ILogger _logger;
        
        public static bool isPrefabsSaveFolderSet
        {
            get { return EditorPrefs.HasKey(PrefabsSaveFolderSaveKey); }
        }

        public static bool isPrefabsSaveFolderValid
        {
            get { return AssetDatabase.IsValidFolder(prefabsSaveFolder); }
        }

        public static string prefabsSaveFolder
        {
            get { return EditorPrefs.GetString(PrefabsSaveFolderSaveKey, "Assets/" + RucksackConstants.ProductName + " Data"); }
            private set { EditorPrefs.SetString(PrefabsSaveFolderSaveKey, value); }
        }

        static InventoryScriptableObjectUtility()
        {
            _logger = new UnityLogger("[Editor] ");
        }
        
        public static string GetSaveFolderForFolderName(string name)
        {
            if (isPrefabsSaveFolderSet == false)
            {
                _logger.Warning("Trying to grab folder for: " + name + " but no prefab folder is set.");
                return string.Empty;
            }

            var saveFolder = prefabsSaveFolder + "/" + name;
            if (AssetDatabase.IsValidFolder(saveFolder) == false)
            {
                CreateFolderIfDoesNotExist(prefabsSaveFolder, name);
                _logger.LogVerbose("Trying to grab folder for " + name + " but could not be found. Creating one...");
            }

            return saveFolder;
        }

//        public static string GetSaveFolderForType(Type type)
//        {
//            return GetSaveFolderForFolderName(type.Name);
//        }

        public static void SetPrefabSaveFolderIfNotSet()
        {
            if (isPrefabsSaveFolderSet == false || isPrefabsSaveFolderValid == false)
            {
                SetPrefabSaveFolder();
            }
        }

        public static void SetPrefabSaveFolder()
        {
            string absolutePath = EditorUtility.SaveFolderPanel("Choose a folder to save your item prefabs", "", "");
            prefabsSaveFolder = "Assets" + absolutePath.Replace(UnityEngine.Application.dataPath, "");

            if (isPrefabsSaveFolderValid)
            {
                CreateFolderIfDoesNotExist(prefabsSaveFolder, "Items");
                CreateFolderIfDoesNotExist(prefabsSaveFolder, "Equipment");
                CreateFolderIfDoesNotExist(prefabsSaveFolder, "Currencies");
                CreateFolderIfDoesNotExist(prefabsSaveFolder, "Settings");
            }
        }

        private static void CreateFolderIfDoesNotExist(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder(path + "/" + folderName) == false)
            {
                AssetDatabase.CreateFolder(path, folderName);
            }
        }
    }
}