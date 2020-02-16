using System;
using System.Collections.Generic;
using Devdog.General2.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Items.Editor
{
    [CustomEditor(typeof(UnityEquippableItemDefinition), true)]
    public class EquippableItemDefinitionEditor : ItemDefinitionEditor
    {
        private SerializedProperty _equipmentType;
        private SerializedProperty _mountPoint;
        private SerializedProperty _equipmentModel;
        
        protected static UnityEditor.Editor equipmentModelPreviewEditor;

        protected override void FindProperties()
        {
            base.FindProperties();
            
            FindProperty(nameof(_equipmentType), out _equipmentType);
            FindProperty(nameof(_mountPoint), out _mountPoint);
            FindProperty(nameof(_equipmentModel), out _equipmentModel);
        }

        protected override void OnInspecstorGUI(Dictionary<string, Action<SerializedProperty>> overrides)
        {
            DrawWithOrder(10, () =>
            {
                var t = (UnityEquippableItemDefinition) target;
                EditorGUILayout.LabelField(new GUIContent("Equippable item info"), General2.Editors.EditorStyles.titleStyle);
                using (new VerticalLayoutBlock(General2.Editors.EditorStyles.boxStyle))
                {
                    DrawPropertyOrOverride(_equipmentType, t.equipmentType, overrides);
                    DrawPropertyOrOverride(_mountPoint, t.mountPoint, overrides);
                    DrawPropertyOrOverride(_equipmentModel, t.equipmentModel, overrides);
                
                    UnityEditor.Editor.CreateCachedEditor(t.equipmentModel, null, ref equipmentModelPreviewEditor);
                    equipmentModelPreviewEditor?.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(500, 250), UnityEditor.EditorStyles.helpBox);
                }
            });
            
            base.OnInspecstorGUI(overrides);
        }
    }
}