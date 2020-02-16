using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.General2;
using Devdog.General2.Editors;
using Devdog.General2.Editors.ReflectionDrawers;
using Devdog.Rucksack.Database;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
    public abstract class InventoryEditorCrudBase<T> : EditorCrudBase<T>
        where T : UnityEngine.Object, ICloneable, IIdentifiable
    {
        protected class TypeFilter
        {
            public System.Type type;
            public bool enabled;

            public TypeFilter(System.Type type, bool enabled)
            {
                this.type = type;
                this.enabled = enabled;
            }
        }
        
        protected class ErrorLookup
        {
            public string message;
            public List<DrawerBase> drawers = new List<DrawerBase>();
        }

        private string databaseSaveKey
        {
            get { return $"{RucksackConstants.ProductName}_DevdogCurrentDatabase_{typeof(T)}"; }
        }

        private DatabaseCollection<T> _databaseCrud;
        private DatabaseCollection<T> databaseCrud
        {
            get { return _databaseCrud; }
            set
            {
                _databaseCrud = value;
                if (_databaseCrud != null)
                {
                    var t = _databaseCrud.database as UnityEngine.Object;
                    if (t != null)
                    {
                        var path = AssetDatabase.GetAssetPath(t);
                        EditorPrefs.SetString(databaseSaveKey, path);
                        
                        new UnityLogger("[Editor] ").Log($"Set {path} as current database for type {typeof(T).Name}", t);
                    }
                    else
                    {
                        EditorPrefs.DeleteKey(databaseSaveKey);
                    }
                }
                else
                {
                    EditorPrefs.DeleteKey(databaseSaveKey);
                }
            }
        }
        
        protected override ICollection<T> crudList
        {
            get { return databaseCrud; }
        }
        
        protected static T previousItem;
        protected string previouslySelectedGUIItemName;

        protected TypeFilter[] allTypes;

        protected Dictionary<T, ErrorLookup> itemsErrorLookup = new Dictionary<T, ErrorLookup>();
        protected readonly string SaveFolderName;
        
        protected InventoryEditorCrudBase(string singleName, string pluralName, string saveFolderName, EditorWindow window)
            : base(singleName, pluralName, window)
        {
            SaveFolderName = saveFolderName;
            allTypes = GetAllItemTypes();
            
            window.autoRepaintOnSceneChange = false;
        }
        
        protected TypeFilter[] GetAllItemTypes()
        {
            return ReflectionUtility.GetAllTypesThatImplement(typeof (T), true)
                .Select(o => new TypeFilter(o, false)).ToArray();
        }

        public override void Focus()
        {
            base.Focus();

            if (databaseCrud?.database == null)
            {
                var dbPath = EditorPrefs.GetString(databaseSaveKey);
                var db = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(dbPath) as IDatabase<T>;
                if (db != null)
                {
                    databaseCrud = new DatabaseCollection<T>(db);
                }
            }
            
            ValidateItems();
        }

        protected void SetDatabaseDirty()
        {
            var db = databaseCrud?.database as UnityDatabase<T>;
            if (db != null)
            {
                UnityEditor.EditorUtility.SetDirty(db);
            }
        }

        public override void AddItem(T item, bool editOnceAdded = true)
        {
            base.AddItem(item, editOnceAdded);
            SetDatabaseDirty();
        }
        
        public override void EditItem(T item)
        {
            base.EditItem(item);

            Undo.ClearUndo(previousItem);
            Undo.RecordObject(item, RucksackConstants.ProductName + "_item");

            previousItem = item;
        }
        
        public override T DuplicateItem(T item)
        {
            InventoryScriptableObjectUtility.SetPrefabSaveFolderIfNotSet();

            var newItem = CreateNewInstanceFromType(item.GetType());
            string assetPath = InventoryScriptableObjectUtility.GetSaveFolderForFolderName(SaveFolderName);
            var fileName = GetNewFileName(newItem.GetType());
            
            UnityEditor.EditorUtility.CopySerialized(item, newItem);
            
            AssetDatabase.CreateAsset(newItem, assetPath + "/" + fileName);
            AssetDatabase.SetLabels(newItem, new string[] { RucksackConstants.ProductName });
            
            DeferAction(() =>
            {
                AddItem(newItem);
                SetDatabaseDirty();
            });
            
            return newItem;
        }
        
        public override void RemoveItem(T item)
        {
            base.RemoveItem(item);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
            SetDatabaseDirty();
        }
        
        public virtual string GetDisplayName(T item)
        {
            return item?.name ?? "";
        }
                
        public virtual string GetTypeDisplayName(T item)
        {
            string name = item?.GetType().Name ?? "";
            name = name.Replace("Unity", "");
            name = name.Replace("Definition", "");

            return name;
        }

        public virtual string GetNewFileName(Type type)
        {
            return type.Name.Replace(' ', '_').ToLower() + "_" + System.DateTime.Now.ToFileTimeUtc() + ".asset";
        }
        
//        public virtual string GetFileName(T item, string suffix = "")
//        {
//            var name = item?.name?.Replace(' ', '_').ToLower() + suffix ?? "";
//            if (name.EndsWith(".asset"))
//            {
//                name += ".asset";
//            }
//            
//            return name;
//        }
        
//        protected virtual void UpdateAssetName(T item)
//        {
//            var newName = GetFileName(item);
//            if (AssetDatabase.GetAssetPath(item).EndsWith(newName) == false)
//            {
//                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), newName);
//            }
//        }
        
        protected override void CreateNewItem()
        {
            var picker = ScriptPickerEditor.Get(typeof(T));
            picker.OnPickObject += type =>
            {
                InventoryScriptableObjectUtility.SetPrefabSaveFolderIfNotSet();
                string assetPath = InventoryScriptableObjectUtility.GetSaveFolderForFolderName(SaveFolderName);

                var asset = CreateNewInstanceFromType(type);

                AssetDatabase.CreateAsset(asset, assetPath + "/" + GetNewFileName(type));
                AssetDatabase.SetLabels(asset, new string[] { RucksackConstants.ProductName });

                AddItem(asset, true);
            };
            
            picker.Show();
        }

        protected abstract T CreateNewInstanceFromType(Type type);

        /// <summary>
        /// Validate all items and make sure they're set up correctly. If not render a warning icon in the crud list.
        /// </summary>
        private void ValidateItems()
        {
            if (databaseCrud?.database == null)
            {
                return;
            }
            
//            foreach (var item in crudList)
//            {
//                if (item == null)
//                {
//                    continue;
//                }
//
//                var drawers = ReflectionDrawerUtility.BuildEditorHierarchy(item.GetType(), item);
//                foreach (var drawer in drawers)
//                {
//                    var isValid = ValidateItem(item, drawer);
//                    if (isValid == false)
//                    {
//                        break;
//                    }
//                }
//            }
        }

        protected void ValidateItemFromCache(T item)
        {
            if (itemsErrorLookup.ContainsKey(item))
            {
                var lookup = itemsErrorLookup[item];
                for (int i = 0; i < lookup.drawers.Count; i++)
                {
                    var isValid = ValidateItem(item, lookup.drawers[i], true);
                    if (isValid == false)
                    {
                        break;
                    }
                }
            }
        }

        protected virtual void ValidateItem(T item)
        {
            var drawers = ReflectionDrawerUtility.BuildEditorHierarchy(item.GetType(), item);
            foreach (var drawer in drawers)
            {
                ValidateItem(item, drawer, false, false);
            }
        }

        protected virtual bool ValidateItem(T item, DrawerBase drawer, bool refreshValues = false, bool isRoot = true)
        {
            if (itemsErrorLookup.ContainsKey(item) == false)
            {
                itemsErrorLookup[item] = new ErrorLookup();
            }
            else
            {
                if (isRoot)
                {
                    itemsErrorLookup[item].message = "";
                    itemsErrorLookup[item].drawers.Clear();
                }
            }

            if (refreshValues)
            {
                drawer.RefreshValue();
            }

            var childDrawer = drawer as IChildrenDrawer;
            if (childDrawer != null)
            {
                bool allValid = true;
                foreach (var c in childDrawer.children)
                {
                    var valid = ValidateItem(item, c, refreshValues, false);
                    if (valid == false)
                    {
                        allValid = false;
                    }
                }

                if (allValid == false)
                {
                    return false;
                }
            }
            else
            {
                if (drawer.isEmpty && drawer.required && drawer.isInArray == false)
                {
                    itemsErrorLookup[item].message += drawer.fieldName.text + " is empty" + '\n';
                    itemsErrorLookup[item].drawers.Add(drawer);
                    return false;
                }
            }

            return true;
        }

        public override void Draw() 
        { 
            if (databaseCrud?.database == null) 
            { 
                DrawDatabasePicker();
            }
            else 
            {
                if (GUI.Button(new Rect(0, 40, 100, 20), "< Database"))
                {
                    databaseCrud = null;
                    return;
                }
                
                base.Draw(); 
            }
        }

        protected virtual IEnumerable<IDatabase<T>> GetProjectDatabases()
        {
            return new IDatabase<T>[0];
        }

        protected virtual void DrawDatabasePicker()
        {
            GUILayout.Space(40);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginVertical(GUILayout.Width(600));
            EditorGUILayout.LabelField("Select a database to edit", General2.Editors.EditorStyles.titleStyle);
            GUILayout.Space(20);

            EditorGUILayout.BeginVertical(Devdog.General2.Editors.EditorStyles.boxStyle);
            EditorGUILayout.LabelField("Found (local) databases");
            foreach (var db in GetProjectDatabases())
            {
                var asset = db as UnityEngine.Object;
                var dbName = db.ToString();
                if (asset != null)
                {
                    dbName = AssetDatabase.GetAssetPath(asset);
                }
                
                if (GUILayout.Button("Select: " + dbName))
                {
                    databaseCrud = new DatabaseCollection<T>(db);
                }
            }
            
            EditorGUILayout.EndVertical();

            
            EditorGUILayout.BeginVertical(Devdog.General2.Editors.EditorStyles.boxStyle);
            EditorGUILayout.LabelField("Manual selection");

            databaseCrud = databaseCrud ?? new DatabaseCollection<T>();
            databaseCrud.database = (IDatabase<T>) UnityEditor.EditorGUILayout.ObjectField(databaseCrud.database as UnityEngine.Object, typeof(IDatabase<T>), false);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();    
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(40);
        }
        
        protected virtual void DrawSidebarValidation(T item, int i)
        {
            if (itemsErrorLookup.ContainsKey(item) && itemsErrorLookup[item].drawers.Count > 0)
            {
                sidebarRowElementOffset.width = 20;
                Vector2 offset = new Vector2(15, 8);
                if (canReOrderItems)
                {
                    offset.x += 90;
                }

                sidebarRowElementOffset.position -= offset;

                EditorGUI.LabelField(sidebarRowElementOffset, new GUIContent("", itemsErrorLookup[item].message), (GUIStyle)"CN EntryError");
                sidebarRowElementOffset.position += offset;
            }
        }

//        private void DrawTooltip(Vector2 position, string message)
//        {
//            if (string.IsNullOrEmpty(message))
//            {
//                return;
//            }
//
//            var style = (GUIStyle)"SelectionRect";
//            var height = 100f;
//
//            GUI.color = new Color(1, 1, 1, 0.5f);
//            using (new GroupBlock(new Rect(position.x, position.y, 240, height), GUIContent.none, style))
//            {
//                GUI.color = Color.white;
//                EditorGUI.LabelField(new Rect(10, 10, 220, height - 20), message, UnityEditor.EditorStyles.wordWrappedLabel);
//            }
//
//            GUI.color = Color.white;
//        }
        
        protected override void DrawSidebar()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            int i = 0;
            foreach (var type in allTypes)
            {
                if (i % 3 == 0)
                {
                    GUILayout.BeginHorizontal();
                }

                type.enabled = GUILayout.Toggle(type.enabled, type.type.Name, "OL Toggle");

                if (i % 3 == 2 || i == allTypes.Length - 1)
                {
                    GUILayout.EndHorizontal();
                }

                i++;
            }
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            base.DrawSidebar();
        }
        
        protected override void DrawSidebarRow(T item, int i)
        {
            int checkedCount = 0;
            foreach (var type in allTypes)
            {
                if (type.enabled)
                    checkedCount++;
            }

            if (checkedCount > 0)
            {
                if (allTypes.FirstOrDefault(o => o.type == item.GetType() && o.enabled) == null)
                {
                    return;
                }
            }

            BeginSidebarRow(item, i);

            DrawSidebarRowElement(item.ID.ToString(), 40);
            DrawSidebarRowElement(GetDisplayName(item), 130);
            DrawSidebarRowElement(GetTypeDisplayName(item), 125);
            DrawSidebarValidation(item, i);

            sidebarRowElementOffset.x -= 20; // To compensate for visibility toggle
            bool t = DrawSidebarRowElementButton("V", UnityEditor.EditorStyles.miniButton, 20, 18);
            
            // User clicked view icon
            if (t)
            {
                AssetDatabase.OpenAsset(selectedItem);
            }

            EndSidebarRow(item, i);
        }

        protected sealed override void DrawDetail(T item)
        {
            if (InventoryScriptableObjectUtility.isPrefabsSaveFolderSet == false)
            {
                EditorGUILayout.HelpBox("Prefab save folder is not set.", MessageType.Error);
                if (GUILayout.Button("Set prefab save folder"))
                {
                    InventoryScriptableObjectUtility.SetPrefabSaveFolder();
                }

                EditorGUIUtility.labelWidth = 0;
                return;
            }
            
            EditorGUI.BeginChangeCheck();
            DrawDetailInternal(item);
            if (EditorGUI.EndChangeCheck() && selectedItem != null)
            {
                UnityEditor.EditorUtility.SetDirty(selectedItem);
            }

            previouslySelectedGUIItemName = GUI.GetNameOfFocusedControl();
            EditorGUIUtility.labelWidth = 0;
        }

        protected virtual void DrawDetailInternal(T item)
        {
            
        }
    }
}