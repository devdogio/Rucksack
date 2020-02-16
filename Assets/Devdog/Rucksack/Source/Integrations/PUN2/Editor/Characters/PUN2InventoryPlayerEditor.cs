using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Collections.Editor;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using UnityEditor;
using UnityEngine;

using Photon.Pun;

namespace Devdog.Rucksack.Characters.Editor
{
    [CustomEditor(typeof(PUN2InventoryPlayer), true)]
    public class PUN2InventoryPlayerEditor : InventoryPlayerEditor
    {
        public override void OnInspectorGUI()
        {
            //            var t = (PUN2InventoryPlayer) target;
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

            var PUN2Col = collection as IPUN2Collection;
            if (PUN2Col != null)
            {
                var t = (InventoryPlayer)target;
                var identity = t.GetComponent<PhotonView>();
                if (identity != null)
                {
                    var permission = PUN2PermissionsRegistry.collections.GetPermission(PUN2Col, identity);
                    EditorGUILayout.LabelField("Permission: ", permission.ToString());
                }

                if (GUILayout.Button("Inspect collection", "minibutton"))
                {
                    CollectionInspectorEditor.collectionNameOrGuid = PUN2Col.ID.ToString();
                    CollectionInspectorEditor.ShowWindow();
                }
            }
        }

        protected override void DrawCollection(IReadOnlyCurrencyCollection<ICurrency, double> collection, int priority)
        {
            base.DrawCollection(collection, priority);

            var PUN2Col = collection as IPUN2Collection;
            if (PUN2Col != null)
            {
                var t = (InventoryPlayer)target;
                var identity = t.GetComponent<PhotonView>();
                if (identity != null)
                {
                    var permission = PUN2PermissionsRegistry.collections.GetPermission(PUN2Col, identity);
                    EditorGUILayout.LabelField("Permission: ", permission.ToString());
                }

                //                if (GUILayout.Button("Inspect collection", "minibutton"))
                //                {
                //                    CollectionInspectorEditor.collectionNameOrGuid = PUN2Col.collectionGuid.ToString();
                //                    CollectionInspectorEditor.ShowWindow();
                //                }
            }
        }
    }
}