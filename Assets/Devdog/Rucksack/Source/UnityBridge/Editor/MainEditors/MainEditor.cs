using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.General2;
using Devdog.General2.Editors;
using Devdog.General2.Editors.GameRules;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
    public class MainEditor : EditorWindow
    {
        private static int _toolbarIndex;
        private static List<IEditorCrud> _editors = new List<IEditorCrud>(8);
        
        private static MainEditor _window;
        public static MainEditor window
        {
            get
            {
                if (_window == null)
                {
                    _window = GetThisWindow();
                }

                return _window;
            }
        }

//        protected string[] editorNames
//        {
//            get
//            {
//                string[] items = new string[_editors.Count];
//                for (int i = 0; i < _editors.Count; i++)
//                {
//                    items[i] = _editors[i].ToString();
//                }
//
//                return items;
//            }
//        }

        [MenuItem(RucksackConstants.ToolsPath + "Main editor", false, -99)] // Always at the top
        public static void ShowWindow()
        {
            _window = GetThisWindow();
        }

        private static MainEditor GetThisWindow()
        {
            return GetWindow<MainEditor>(false, RucksackConstants.ProductName + " Manager", true);
        }

        private void OnEnable()
        {
            minSize = new Vector2(600.0f, 400.0f);
            _toolbarIndex = 0;

            GameRulesWindow.CheckForIssues();
            GameRulesWindow.OnIssuesUpdated += UpdateMiniToolbar;

            CreateEditors();
        }

        private void OnDisable()
        {
            GameRulesWindow.OnIssuesUpdated -= UpdateMiniToolbar;
        }

        internal static void UpdateMiniToolbar(List<IGameRule> issues)
        {
            window.Repaint();
        }

//        public static void SelectPage(string path)
//        {
//            throw new NotImplementedException();
//        }
        
        public virtual void CreateEditors()
        {
            _editors.Clear();

            var dict = new Dictionary<string, EmptyEditor>();
            
            var foundEditors = ReflectionUtility.GetAllClassesWithAttribute(typeof(EditorPageAttribute), true).ToList();
            foundEditors.Sort((a, b) =>
            {
                var first = (EditorPageAttribute)a.GetCustomAttributes(typeof(EditorPageAttribute), true).First();
                var second = (EditorPageAttribute)b.GetCustomAttributes(typeof(EditorPageAttribute), true).First();

                if (first.order > second.order)
                {
                    return 1;
                }
                else if (first.order < second.order)
                {
                    return -1;
                }

                return 0;
            });
            
            foreach (var foundEditor in foundEditors)
            {
                var attribute = (EditorPageAttribute)foundEditor.GetCustomAttributes(typeof(EditorPageAttribute), true).First();
                var pathSplit = attribute.path.Split('/');
                if (pathSplit.Length == 1 || pathSplit.Length == 2)
                {
                    if (dict.ContainsKey(pathSplit[0]) == false)
                    {
                        dict[pathSplit[0]] = new EmptyEditor(pathSplit[0], this);
                    }

                    var constructor = foundEditor.GetConstructor(new[] {typeof(EditorWindow)});
                    if (constructor != null)
                    {
                        var inst = (IEditorCrud)constructor.Invoke(new object[] {this});
                        dict[pathSplit[0]].childEditors.Add(inst);
                    }
                    else
                    {
                        Debug.Log("[Editor] Couldn't initialize editor page, constructor needs to take 1 argument (EditorWindow): parent", this);
                    }
                }
                else
                {
                    Debug.Log("[Editor] Couldn't initialize editor with attribute path: " + attribute.path, this);
                }
            }

            _editors.AddRange(dict.Select(o => o.Value));
            
            _toolbarIndex = 0;
            if (_editors.Count > _toolbarIndex)
            {
                _editors[_toolbarIndex].Focus();
            }
        }

        protected virtual void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            int before = _toolbarIndex;
            var editorNames = _editors.Select(o => o.ToString()).ToArray();
            _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, editorNames, Devdog.General2.Editors.EditorStyles.toolbarStyle);
            if (before != _toolbarIndex)
            {
                _editors[_toolbarIndex].Focus();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        internal static void DrawMiniToolbar(List<IGameRule> issues)
        {
            GUILayout.BeginVertical("Toolbar", GUILayout.ExpandWidth(true));
            
            var issueCount = issues.Sum(o => o.ignore == false ? o.issues.Count : 0);
            if (issueCount > 0)
            {
                GUI.color = Color.red;
            }
            else
            {
                GUI.color = Color.green;
            }
            
            if (GUILayout.Button(issueCount + " issues found in scene.", "toolbarbutton", GUILayout.Width(300)))
            {
                GameRulesWindow.ShowWindow();
            }
            
            GUI.color = Color.white;
            GUILayout.EndVertical();
        }

        public void OnGUI()
        {
            DrawToolbar();

            if (InventoryScriptableObjectUtility.isPrefabsSaveFolderSet == false || InventoryScriptableObjectUtility.isPrefabsSaveFolderValid == false)
            {
                SettingsEditor.DrawSaveFolderPicker();
                return;
            }
            
            if (_toolbarIndex < 0 || _toolbarIndex >= _editors.Count || _editors.Count == 0)
            {
                _toolbarIndex = 0;
                CreateEditors();
            }

            if (_editors.Count == 0)
            {
                EditorGUILayout.LabelField("No editor pages found");
                if (GUILayout.Button("Force refresh"))
                {
                    _toolbarIndex = 0;
                    CreateEditors();
                }
                    
                return;
            }

            // Draw the editor
            _editors[_toolbarIndex].Draw();

            DrawMiniToolbar(GameRulesWindow.GetAllActiveRules().ToList());
        }
    }
}