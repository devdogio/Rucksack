using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Collections.Editor;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Characters.Editor
{
    [CustomEditor(typeof(UNetInventoryPlayer), true)]
    public class UNetInventoryPlayerEditor : InventoryPlayerEditor
    {
        public override void OnInspectorGUI()
        {
//            var t = (UNetInventoryPlayer) target;
//            if (UnityEngine.Application.isPlaying)
//            {
//                EditorGUILayout.LabelField("Server collections", UnityEditor.EditorStyles.boldLabel);
//
//                DrawInventoryCollections(t, t.serverItemCollectionGroup);
//                DrawEquipmentCollections(t, t.serverEquipmentCollectionGroup);
//                DrawCurrencyCollections(t, t.serverCurrencyCollectionGroup);
//            }
            
            base.OnInspectorGUI();
        }

        protected override void DrawCollection(IReadOnlyCollection<IItemInstance> collection, int priority)
        {
            base.DrawCollection(collection, priority);
            
            var unetCol = collection as IUNetCollection;
            if (unetCol != null)
            {
                var t = (InventoryPlayer) target;
                var identity = t.GetComponent<NetworkIdentity>();
                if (identity != null)
                {
                    var permission = UNetPermissionsRegistry.collections.GetPermission(unetCol, identity);
                    EditorGUILayout.LabelField("Permission: ", permission.ToString());
                }
                
                if (GUILayout.Button("Inspect collection", "minibutton"))
                {
                    CollectionInspectorEditor.collectionNameOrGuid = unetCol.ID.ToString();
                    CollectionInspectorEditor.ShowWindow();
                }
            }
        }

        protected override void DrawCollection(IReadOnlyCurrencyCollection<ICurrency, double> collection, int priority)
        {
            base.DrawCollection(collection, priority);
            
            var unetCol = collection as IUNetCollection;
            if (unetCol != null)
            {
                var t = (InventoryPlayer) target;
                var identity = t.GetComponent<NetworkIdentity>();
                if (identity != null)
                {
                    var permission = UNetPermissionsRegistry.collections.GetPermission(unetCol, identity);
                    EditorGUILayout.LabelField("Permission: ", permission.ToString());
                }
    
//                if (GUILayout.Button("Inspect collection", "minibutton"))
//                {
//                    CollectionInspectorEditor.collectionNameOrGuid = unetCol.collectionGuid.ToString();
//                    CollectionInspectorEditor.ShowWindow();
//                }
            }
        }
    }
}