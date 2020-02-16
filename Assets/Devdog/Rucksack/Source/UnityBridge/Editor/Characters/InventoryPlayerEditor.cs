using Devdog.General2.Editors;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Characters.Editor
{
    [CustomEditor(typeof(InventoryPlayer), true)]
    public class InventoryPlayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var t = (InventoryPlayer) target;
            if (UnityEngine.Application.isPlaying)
            {
                EditorGUILayout.LabelField("Client collections", UnityEditor.EditorStyles.boldLabel);
                DrawInventoryCollections(t, t.itemCollectionGroup);
                DrawEquipmentCollections(t, t.equipmentCollectionGroup);
                DrawCurrencyCollections(t, t.currencyCollectionGroup);
            }
            
            base.OnInspectorGUI();
        }

        protected virtual void DrawInventoryCollections(InventoryPlayer t, CollectionGroup<IItemInstance> g)
        {
            using (new VerticalLayoutBlock("box"))
            {
                GUILayout.Label(new GUIContent("Inventories"), UnityEditor.EditorStyles.boldLabel);
                foreach (var collection in g.collections)
                {
                    DrawCollection(collection.collection, collection.priority.GetGeneralPriority());
                }
            }
        }

        protected virtual void DrawEquipmentCollections(InventoryPlayer t, CollectionGroup<IEquippableItemInstance, IEquipmentCollection<IEquippableItemInstance>> g)
        {
            using (new VerticalLayoutBlock("box"))
            {
                GUILayout.Label(new GUIContent("Equipment"), UnityEditor.EditorStyles.boldLabel);
                foreach (var collection in g.collections)
                {
                    DrawCollection(collection.collection, collection.priority.GetGeneralPriority());
                }
            }
        }
                
        protected virtual void DrawCurrencyCollections(InventoryPlayer t, CurrencyCollectionGroup<ICurrency> g)
        {
            using (new VerticalLayoutBlock("box"))
            {
                GUILayout.Label(new GUIContent("Currencies"), UnityEditor.EditorStyles.boldLabel);
                foreach (var collection in g.collections)
                {
                    DrawCollection(collection.collection, collection.priority.GetGeneralPriority());
                }
            }
        }

        protected virtual void DrawCollection(IReadOnlyCollection<IItemInstance> collection, int priority)
        {
            EditorGUILayout.LabelField("Name: ", collection?.ToString());
            EditorGUILayout.LabelField("Priority: ", priority + " (general)");
        }
        
        protected virtual void DrawCollection(IReadOnlyCurrencyCollection<ICurrency, double> collection, int priority)
        {
            EditorGUILayout.LabelField("Name: ", collection?.ToString());
            EditorGUILayout.LabelField("Priority: ", priority + " (general)");

            EditorGUILayout.LabelField("Currencies: ", UnityEditor.EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            foreach (var dec in collection?.ToDecorators() ?? new CurrencyDecorator<double>[0])
            {
                EditorGUILayout.LabelField(dec.currency.ToString(), dec.amount.ToString());
            }
            EditorGUI.indentLevel--;
        }
    }
}