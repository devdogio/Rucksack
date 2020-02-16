using System;
using System.Collections.Generic;
using Devdog.Rucksack.Database;
using Devdog.Rucksack.Items;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
    [EditorPage("Items/Item", -1)]
    public class ItemEditor : InventoryEditorCrudBase<UnityItemDefinition>
    {
        public UnityEditor.Editor itemEditorInspector { get; set; }

        private UnityItemDefinitionDatabase[] _databases;
        
        public ItemEditor(EditorWindow window)
            : base("Item", "Items", "Items", window)
        {
        }

        protected override bool MatchesSearch(UnityItemDefinition item, string searchQuery)
        {
            searchQuery = searchQuery ?? "";

            string search = searchQuery.ToLower();
            return (GetDisplayName(item).ToLower().Contains(search) ||
                item.description.ToLower().Contains(search) ||
                item.ID.ToString().Contains(search) ||
                GetTypeDisplayName(item).ToLower().Contains(search));
        }

        public override UnityItemDefinition DuplicateItem(UnityItemDefinition item)
        {
            var newItem = base.DuplicateItem(item);
            if (newItem != null)
            {
                newItem.ResetID(System.Guid.NewGuid());
                newItem.ResetLocalizedStrings();
            }
            
            return newItem;
        }

        public override void EditItem(UnityItemDefinition item)
        {
            base.EditItem(item);
            if (item != null)
            {
                itemEditorInspector = UnityEditor.Editor.CreateEditor(item);
            }
        }

        protected override UnityItemDefinition CreateNewInstanceFromType(Type type)
        {
            var asset = (UnityItemDefinition)ScriptableObject.CreateInstance(type);
            asset.ResetID(System.Guid.NewGuid());

            return asset;
        }

        protected override IEnumerable<IDatabase<UnityItemDefinition>> GetProjectDatabases()
        {
            if (_databases == null)
            {
                _databases = Resources.FindObjectsOfTypeAll<UnityItemDefinitionDatabase>();
            }

            return _databases;
        }

        protected override void DrawDetailInternal(UnityItemDefinition item)
        {
            itemEditorInspector.OnInspectorGUI();
        }
    }
}