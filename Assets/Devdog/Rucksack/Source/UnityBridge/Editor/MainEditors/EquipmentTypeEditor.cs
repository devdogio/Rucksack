using System;
using System.Collections.Generic;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.Database;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
    [EditorPage("Equipment/Equipment types", 10)]
    public class EquipmentTypeEditor : InventoryEditorCrudBase<UnityEquipmentType>
    {
        public UnityEditor.Editor editor { get; set; }
        private UnityEquipmentTypeDatabase[] _databases;

        public EquipmentTypeEditor(EditorWindow window)
            : base("Equipment Type", "Equipment Types", "EquipmentTypes", window)
        { }

        protected override bool MatchesSearch(UnityEquipmentType item, string searchQuery)
        {
            searchQuery = searchQuery ?? "";

            string search = searchQuery.ToLower();
            return (GetDisplayName(item).ToLower().Contains(search) ||
                item.ID.ToString().Contains(search) ||
                GetTypeDisplayName(item).ToLower().Contains(search));
        }

        public override void EditItem(UnityEquipmentType item)
        {
            base.EditItem(item);
            if (item != null)
            {
                editor = UnityEditor.Editor.CreateEditor(item);
            }
        }
        
        public override UnityEquipmentType DuplicateItem(UnityEquipmentType item)
        {
            var newItem = base.DuplicateItem(item);
            newItem?.ResetID(System.Guid.NewGuid());
            return newItem;
        }
        
        protected override UnityEquipmentType CreateNewInstanceFromType(Type type)
        {
            var asset = (UnityEquipmentType)ScriptableObject.CreateInstance(type);
            asset.ResetID(System.Guid.NewGuid());

            return asset;
        }
        
        protected override IEnumerable<IDatabase<UnityEquipmentType>> GetProjectDatabases()
        {
            if (_databases == null)
            {
                _databases = Resources.FindObjectsOfTypeAll<UnityEquipmentTypeDatabase>();
            }

            return _databases;
        }

        protected override void DrawDetailInternal(UnityEquipmentType item)
        {
            editor.OnInspectorGUI();
        }
    }
}