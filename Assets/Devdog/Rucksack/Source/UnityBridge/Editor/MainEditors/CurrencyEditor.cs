using System;
using System.Collections.Generic;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Database;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
    [EditorPage("Currencies/Currency definitions", 20)]
    public class CurrencyEditor : InventoryEditorCrudBase<UnityCurrency>
    {
        public UnityEditor.Editor editor { get; set; }
        private UnityCurrencyDatabase[] _databases;

        public CurrencyEditor(EditorWindow window)
            : base("Currency", "Currencies", "Currencies", window)
        { }

        protected override bool MatchesSearch(UnityCurrency item, string searchQuery)
        {
            searchQuery = searchQuery ?? "";

            string search = searchQuery.ToLower();
            return (GetDisplayName(item).ToLower().Contains(search) ||
                item.ID.ToString().Contains(search) ||
                GetTypeDisplayName(item).ToLower().Contains(search));
        }

        public override void EditItem(UnityCurrency item)
        {
            base.EditItem(item);
            if (item != null)
            {
                editor = UnityEditor.Editor.CreateEditor(item);
            }
        }
        
        public override UnityCurrency DuplicateItem(UnityCurrency item)
        {
            var newItem = base.DuplicateItem(item);
            if (newItem != null)
            {
                newItem.ResetID(System.Guid.NewGuid());
            }
            
            return newItem;
        }

        protected override UnityCurrency CreateNewInstanceFromType(Type type)
        {
            var asset = (UnityCurrency)ScriptableObject.CreateInstance(type);
            asset.ResetID(System.Guid.NewGuid());

            return asset;
        }
                
        protected override IEnumerable<IDatabase<UnityCurrency>> GetProjectDatabases()
        {
            if (_databases == null)
            {
                _databases = Resources.FindObjectsOfTypeAll<UnityCurrencyDatabase>();
            }

            return _databases;
        }

        protected override void DrawDetailInternal(UnityCurrency item)
        {
            GUI.enabled = string.IsNullOrEmpty(item.name) == false;
            if (GUILayout.Button("Update asset name"))
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), item.name + " - " + item.ID.ToString().Substring(0, 8));
            }
            GUI.enabled = true;
            
            editor.OnInspectorGUI();
        }
    }
}