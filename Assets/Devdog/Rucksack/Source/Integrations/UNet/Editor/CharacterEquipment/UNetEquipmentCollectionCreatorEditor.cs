using Devdog.General2.Editors;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.UI;
using UnityEditor;
using UnityEngine;
using EditorUtility = UnityEditor.EditorUtility;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// Creates a local item collection on Awake and registers it in the CollectionRegistry
    /// </summary>
    [CustomEditor(typeof(UNetEquipmentCollectionCreator))]
    public sealed class UNetEquipmentCollectionCreatorEditor : UnityEditor.Editor
    {
        private EquipmentCollectionUI _collectionUI;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (UNetEquipmentCollectionCreator) target;
            var bridge = t.GetComponent<UNetActionsBridge>();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            using (new VerticalLayoutBlock("box"))
            {
                EditorGUILayout.LabelField("Editor", UnityEditor.EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Copy collection data from CollectionUI component");
                
                _collectionUI = (EquipmentCollectionUI)EditorGUILayout.ObjectField(_collectionUI, typeof(EquipmentCollectionUI), true);
                if (_collectionUI != null)
                {
                    if (GUILayout.Button("Copy (overwrites old)"))
                    {
                        var collection = new EquipmentCollection<IEquippableItemInstance>(0, t.GetComponent<IEquippableCharacter<IEquippableItemInstance>>(), null);
                        _collectionUI.collection = collection;
//                        _collectionUI.IndexSlotsAndMountPoints();

                        t.slots = new UnitySerializedEquipmentCollectionSlot[_collectionUI.collection.slotCount];
                        for (int i = 0; i < _collectionUI.collection.slotCount; i++)
                        {
                            var equipmentTypes = new UnityEquipmentType[collection.slots[i].equipmentTypes.Length];
                            for (int j = 0; j < equipmentTypes.Length; j++)
                            {
                                equipmentTypes[j] = bridge.equipmentTypeDatabase.Get(new Identifier(collection.slots[i].equipmentTypes[j].ID)).result;
                            }
                        
                            t.slots[i] = new UnitySerializedEquipmentCollectionSlot()
                            {
                                equipmentTypes = equipmentTypes,
                            };
                        }
                    
                        EditorUtility.SetDirty(t);
                        GUI.changed = true;
                    
                        Debug.Log($"Copied {_collectionUI.collection.slotCount} slots to player", t);
                        _collectionUI.collection = null;
                    }
                }
            }
        }
    }
}