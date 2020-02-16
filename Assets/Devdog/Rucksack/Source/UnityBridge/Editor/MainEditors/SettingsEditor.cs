using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devdog.General2;
using Devdog.General2.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
    [EditorPage("Settings/General Settings", 999)]
    public class SettingsEditor : EditorCrudBase<SettingsEditor.CategoryLookup>
    {
        public class CategoryLookup
        {
            public string name { get; }
            public readonly List<SerializedProperty> serializedProperties = new List<SerializedProperty>(8);

            public CategoryLookup(string name)
            {
                this.name = name;
            }

            public override bool Equals(object obj)
            {
                var o = obj as CategoryLookup;
                return o != null && o.name == name;
            }

            public override int GetHashCode()
            {
                return name.GetHashCode();
            }
        }

        private SerializedObject _serializedObject;
        public SerializedObject serializedObject
        {
            get
            {
                if (_serializedObject == null)
                    _serializedObject = new SerializedObject(settings);

                return _serializedObject;
            }
        }

        private GeneralSettings _settings;
        public GeneralSettings settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Resources.FindObjectsOfTypeAll<GeneralSettings>().FirstOrDefault();
//                    var manager = Resources.FindObjectsOfTypeAll<GeneralSettings>().FirstOrDefault();
//                    if (manager != null)
//                    {
//                        _settings = manager.settingsDatabase;
//                    }
                }

                return _settings;
            }
            protected set { _settings = value; }
        }

        protected override ICollection<CategoryLookup> crudList
        {
            get
            {
                var list = new List<CategoryLookup>(8);
                if (settings != null)
                {
                    var fields = settings.GetType().GetFields();

                    CategoryLookup currentCategory = null;
                    foreach (var field in fields)
                    {
                        var cat = (CategoryAttribute)field.GetCustomAttributes(typeof (CategoryAttribute), true).FirstOrDefault();
                        if (cat != null)
                        {
                            // Got a category marker
                            currentCategory = new CategoryLookup(cat.category);
                            list.Add(currentCategory);
                        }

                        if (currentCategory != null)
                        {
                            var prop = serializedObject.FindProperty(field.Name);
                            if (prop != null)
                            {
                                currentCategory.serializedProperties.Add(prop);
                            }
                        }
                    }
                }

                return list;
            }
        }

        public SettingsEditor(EditorWindow window)
            : base("Settings", "Settings", window)
        {
            this.canCreateItems = false;
            this.canDeleteItems = false;
            this.canReOrderItems = false;
            this.canDuplicateItems = false;
            this.hideCreateItem = true;
        }

        protected override void CreateNewItem()
        {
            
        }

        public override CategoryLookup DuplicateItem(CategoryLookup item)
        {
            return item;
        }

        protected override bool MatchesSearch(CategoryLookup category, string query)
        {
            query = query.ToLower();
            return category.name.ToLower().Contains(query) || category.serializedProperties.Any(o => o.displayName.ToLower().Contains(query));
        }

        public override void Draw()
        {
            DrawSaveFolderPicker();

            base.Draw();
        }

        public static void DrawSaveFolderPicker()
        {
            if (InventoryScriptableObjectUtility.isPrefabsSaveFolderSet == false || InventoryScriptableObjectUtility.isPrefabsSaveFolderValid == false)
            {   
                GUI.color = Color.red;
            }
            
            EditorGUILayout.BeginHorizontal(Devdog.General2.Editors.EditorStyles.boxStyle);
            EditorGUILayout.LabelField(RucksackConstants.ProductName + " save folder: " + InventoryScriptableObjectUtility.prefabsSaveFolder);
            if (GUILayout.Button("Set path", GUILayout.Width(100)))
            {
                InventoryScriptableObjectUtility.SetPrefabSaveFolder();
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
        }

        protected override void DrawSidebarRow(CategoryLookup category, int i)
        {
            BeginSidebarRow(category, i);

            DrawSidebarRowElement(category.name, 400);

            EndSidebarRow(category, i);
        }

        protected override void DrawDetail(CategoryLookup category)
        {
            EditorGUILayout.BeginVertical(Devdog.General2.Editors.EditorStyles.boxStyle);
            EditorGUIUtility.labelWidth = Devdog.General2.Editors.EditorStyles.labelWidth;


            serializedObject.Update();
            foreach (var setting in category.serializedProperties)
            {
                EditorGUILayout.PropertyField(setting, true);
            }
            serializedObject.ApplyModifiedProperties();


            EditorGUIUtility.labelWidth = 0; // Resets it to the default
            EditorGUILayout.EndVertical();
        }
    }
}