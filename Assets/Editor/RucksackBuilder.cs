using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;

namespace Devdog.Editors.Internal
{
	public class InventoryPlusBuilder : EditorWindow 
	{
		[XmlRoot(ElementName="dependency")]
		public class Dependency {
			[XmlAttribute(AttributeName="name")]
			public string Name { get; set; }
			[XmlAttribute(AttributeName="minVersion")]
			public string MinVersion { get; set; }
		}

		[XmlRoot(ElementName="plugin")]
		public class Plugin {
			[XmlElement(ElementName="name")]
			public string Name { get; set; }
			[XmlElement(ElementName="description")]
			public string Description { get; set; }
			[XmlElement(ElementName="version")]
			public string Version { get; set; }

			private string _separateBuild;
			[XmlElement(ElementName="separateBuild")]
			public bool SeparateBuild {
				get { return bool.Parse(_separateBuild); }
				set { _separateBuild = value.ToString(); }
			}
			
			[XmlElement(ElementName="dependencies")]
			public List<Dependency> Dependencies { get; set; }
		}
		
		
		private static InventoryPlusBuilder _window;
		private static Dictionary<string, Plugin> _plugins = new Dictionary<string, Plugin>();
		private static HashSet<string> _allSubFolders = new HashSet<string>();
		private static HashSet<string> _mainPackageFiles = new HashSet<string>();
		private static Vector2 _scrollPos;
		
        [MenuItem("Tools/Build Rucksack", false, -99)]
        public static void ShowWindow()
        {
            _window = GetWindow<InventoryPlusBuilder>(false, "Rucksack Builder", true);
	        _window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 600, 400);
        }

        public void OnGUI()
        {
	        if (GUILayout.Button("Scan"))
	        {
		        ScanProject();
	        }

	        foreach (var plugin in _plugins)
	        {
		        EditorGUILayout.BeginHorizontal();
				
		        EditorGUILayout.LabelField(plugin.Value.Name);
//		        EditorGUILayout.LabelField("V" + plugin.Value.Version, GUILayout.Width(60));
		        EditorGUI.BeginChangeCheck(); 
		        plugin.Value.Version = EditorGUILayout.DelayedTextField(plugin.Value.Version, GUILayout.Width(60));
		        if (EditorGUI.EndChangeCheck())
		        {
			        var serializer = new XmlSerializer(typeof(Plugin));
			        var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(plugin.Key);
			        var assetPath = AssetDatabase.GetAssetPath(asset);
			        using (var writer = new StreamWriter(assetPath))
			        {
				        serializer.Serialize(writer, plugin.Value);
			        }
			        
			        EditorUtility.SetDirty(asset);
			        Debug.Log("Updated version number in config to: " + plugin.Value.Version);
		        }
		        
		        EditorGUILayout.Toggle(plugin.Value.SeparateBuild, GUILayout.Width(40));
		        if (GUILayout.Button("Export"))
		        {
			        var saveFolder = EditorUtility.SaveFolderPanel("Choose a save folder", "PackageExports", "");
			        if (string.IsNullOrEmpty(saveFolder) == false)
			        {
				        ExportPackage(plugin, saveFolder);
			        }
		        }
		        
		        EditorGUILayout.EndHorizontal();
	        }

	        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
	        foreach (var file in _mainPackageFiles)
	        {
		        EditorGUILayout.LabelField(file);
	        }
	        EditorGUILayout.EndScrollView();
	        
	        
        	if(GUILayout.Button("Export all"))
	        {
		        var saveFolder = EditorUtility.SaveFolderPanel("Choose a save folder", "PackageExports", "");
		        if (string.IsNullOrEmpty(saveFolder) == false)
		        {
			        ScanProject();
			        Debug.Log($"Saving all files in {saveFolder}");
			        
			        foreach (var plugin in _plugins.Where(o => o.Value.SeparateBuild))
			        {
				        var pluginFolder = GetFolder(plugin.Key);
				        var pluginSubFolders = new HashSet<string>();
				        GetSubFoldersRecursive(pluginFolder, pluginSubFolders);

				        var pluginFiles = new HashSet<string>(AssetDatabase.FindAssets("", pluginSubFolders.ToArray()).Select(AssetDatabase.GUIDToAssetPath));
				        foreach (var pluginFile in pluginFiles)
				        {
					        _mainPackageFiles.Remove(pluginFile);
				        }
				        
				        ExportPackage(plugin, saveFolder);
			        }
			        
			        Debug.Log("Assets found: " + _mainPackageFiles.Count);
			        AssetDatabase.ExportPackage(_mainPackageFiles.ToArray(), $"{saveFolder}/Rucksack_V1.unitypackage");
		        }
	        }
        }

		private static void ScanProject()
		{
			_plugins = GetAllPluginsInProject();
			_allSubFolders = new HashSet<string>();
			GetSubFoldersRecursive("Assets/Devdog", _allSubFolders);
			_mainPackageFiles = new HashSet<string>(AssetDatabase.FindAssets("", _allSubFolders.ToArray()).Select(AssetDatabase.GUIDToAssetPath));
			
			foreach (var plugin in _plugins.Where(o => o.Value.SeparateBuild))
			{
				var pluginFolder = GetFolder(plugin.Key);
				var pluginSubFolders = new HashSet<string>();
				GetSubFoldersRecursive(pluginFolder, pluginSubFolders);

				var pluginFiles = new HashSet<string>(AssetDatabase.FindAssets("", pluginSubFolders.ToArray()).Select(AssetDatabase.GUIDToAssetPath));
				foreach (var pluginFile in pluginFiles)
				{
					_mainPackageFiles.Remove(pluginFile);
				}
			}
		}

		private static void GetSubFoldersRecursive(string startFolder, HashSet<string> appendTo)
		{
			appendTo.Add(startFolder.TrimEnd('/'));

			var subFolders = AssetDatabase.GetSubFolders(startFolder);
			foreach (var subFolder in subFolders)
			{
				appendTo.Add(subFolder.TrimEnd('/'));
				GetSubFoldersRecursive(subFolder, appendTo);
			}
		}

		private static string GetFolder(string filePath)
		{
			var lastIndex = filePath.LastIndexOf('/');
			if (lastIndex == -1)
			{
				return filePath;
			}
			
			return filePath.Substring(0, lastIndex).TrimEnd('/');
		}

		private static void ExportPackage(KeyValuePair<string, Plugin> plugin, string saveFolder)
		{
			AssetDatabase.ExportPackage(GetFolder(plugin.Key), $"{saveFolder}/{plugin.Value.Name}_V{plugin.Value.Version}.unitypackage", ExportPackageOptions.Recurse);
			Debug.Log($"Exported package path: {GetFolder(plugin.Key)} with name: {plugin.Value.Name} - V{plugin.Value.Version} to {saveFolder}");
		}

		private static Dictionary<string, Plugin> GetAllPluginsInProject()
		{
			AssetDatabase.Refresh();
			
			var plugins = new Dictionary<string, Plugin>();
			var pluginGuids = AssetDatabase.FindAssets("plugin t:TextAsset");
			var serializer = new XmlSerializer(typeof(Plugin));
			foreach (var pluginAssetFileGuid in pluginGuids)
			{
				var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(pluginAssetFileGuid));
				if (asset != null)
				{
					try
					{
						var p = serializer.Deserialize(new StringReader(asset.text));
						var plugin = p as Plugin;
						if (plugin != null)
						{
							plugins.Add(AssetDatabase.GUIDToAssetPath(pluginAssetFileGuid), plugin);
						}
					}
					catch (Exception)
					{
						// Ignored
					}
				}
			}

			return plugins;
		}
	}
}