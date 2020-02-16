using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Devdog.General2.Editors;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
    public class IntegrationsEditor : EditorWindow
    {
        private const string MenuItemPath = RucksackConstants.ToolsPath + "Integrations";
        private static IntegrationsEditor _window;
        
        [MenuItem(MenuItemPath, false, 10)] // Always at bottom
        protected static void ShowWindowInternal()
        {
            _window = GetWindow<IntegrationsEditor>();
            _window.Show();
        }
        
        protected void OnGUI()
        {
            GUILayout.Label("Integrations can be downloaded at our website devdog.io");

            GUI.color = Color.green;
            if (GUILayout.Button("Download integrations"))
            {
                Application.OpenURL("https://devdog.io/unity-assets/rucksack-multiplayer-inventory-system/community-bonus?utm_source=unity");
            }
            GUI.color = Color.white;
        }
    }
}