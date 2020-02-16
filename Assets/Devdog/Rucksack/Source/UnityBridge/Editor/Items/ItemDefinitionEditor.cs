using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.General2;
using Devdog.General2.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Items.Editor
{
    [CustomEditor(typeof(UnityItemDefinition), true)]
    public class ItemDefinitionEditor : ObjectEditorBase<UnityItemDefinition>
    {
        private SerializedProperty _parent;
        private SerializedProperty _guid;
        private SerializedProperty _name;
        private SerializedProperty _description;
        private SerializedProperty _maxStackSize;
        private SerializedProperty _layoutShape;
        private SerializedProperty _buyPrice;
        private SerializedProperty _sellPrice;
        private SerializedProperty _weight;
        private SerializedProperty _icon;
        private SerializedProperty _worldModel;

        protected static UnityEditor.Editor modelPreviewEditor;
        
        protected override void FindProperties()
        {
            properties.Clear();
            FindProperty(nameof(_parent), out _parent);
            FindProperty(nameof(_guid), out _guid);
            FindProperty(nameof(_name), out _name);
            FindProperty(nameof(_description), out _description);
            FindProperty(nameof(_icon), out _icon);
            FindProperty(nameof(_worldModel), out _worldModel);
            
            FindProperty(nameof(_layoutShape), out _layoutShape);
            FindProperty(nameof(_maxStackSize), out _maxStackSize);
            FindProperty(nameof(_weight), out _weight);
            
            FindProperty(nameof(_buyPrice), out _buyPrice);
            FindProperty(nameof(_sellPrice), out _sellPrice);
        }

        protected override void OnInspecstorGUI(Dictionary<string, Action<SerializedProperty>> overrides)
        {
            if (overrides.ContainsKey("m_Script") == false)
            {
                // Hides the m_Script
                overrides["m_Script"] = (obj) => { };
            }
            
            var t = (UnityItemDefinition) target;
            GUI.enabled = string.IsNullOrEmpty(t.name) == false;
            if (GUILayout.Button("Update asset name"))
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(t), t.name + " - " + t.ID.ToString().Substring(0, 8));
            }
            GUI.enabled = true;
            
            EditorGUILayout.LabelField(new GUIContent("Type: " + target.GetType().Name));


            DoDrawWithOrder(0, 9);
            
            EditorGUILayout.LabelField(new GUIContent("General item info"), General2.Editors.EditorStyles.titleStyle);
            using (new VerticalLayoutBlock(General2.Editors.EditorStyles.boxStyle))
            {
                DrawPropertyOrOverride(_parent, t.parent, overrides);
                if (Equals(_parent.objectReferenceValue, t) || ReferenceEquals(_parent.objectReferenceValue, t) || t.Equals(_parent.objectReferenceValue))
                {
                    _parent.objectReferenceValue = null;
                }
                
                DrawReadOnlyPropertyOrOverride(_guid, t.ID, overrides);
                DrawPropertyOrOverride(_name, t.name, overrides);
                DrawPropertyOrOverride(_description, t.description, overrides);
                DrawPropertyOrOverride(_icon, t.icon, overrides);
                if (t.icon != null)
                {
                    DrawSprite(GUILayoutUtility.GetRect(32, 32), t.icon);
                }
                
                DrawPropertyOrOverride(_worldModel, t.worldModel, overrides);
                
                UnityEditor.Editor.CreateCachedEditor(t.worldModel, null, ref modelPreviewEditor);
                if (modelPreviewEditor != null)
                {
                    modelPreviewEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(500, 250), UnityEditor.EditorStyles.helpBox);
                }
            }
            
            DoDrawWithOrder(10, 19);

            EditorGUILayout.LabelField(new GUIContent("Item specific"), General2.Editors.EditorStyles.titleStyle);
            using (new VerticalLayoutBlock(General2.Editors.EditorStyles.boxStyle))
            {
//                var keys = overrides.Keys.Concat(properties.Select(o => o.name));
//                foreach (var key in keys)
//                {
//                    SerializedProperty prop;
//                    GetPropertyWithoutAdding(key, out prop);
//                    
//                    if (prop != null)
//                    {
//                        var r = ReflectionUtility.GetFieldInherited()
//                        DrawPropertyOrOverride(prop, , overrides);
//                    }
//                }

                var propertyToExclude = overrides.Keys.Concat(properties.Select(o => o.name)).ToArray();
                var iterator = serializedObject.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;
                    if (propertyToExclude.Contains<string>(iterator.name) == false)
                    {
                        var n = iterator.name.Replace("_", "");
                        var val = t.GetValue(o =>
                        {
                            var prop = ReflectionUtility.GetPropertyInherited(t.GetType(), n);
                            if (prop != null)
                            {
                                return prop.GetValue(t);
                            }
                            
                            return null;
                        });
                        
                        DrawPropertyOrOverride(iterator, val, overrides);
                    }
                }
                
//                overrides.Keys.Concat(properties.Select(o => o.name).Where(o => properties.Contains(o) == false))
//                DrawPropertiesExcluding(serializedObject, );
            }

            DoDrawWithOrder(20, 29);

            EditorGUILayout.LabelField(new GUIContent("Logistics"), General2.Editors.EditorStyles.titleStyle);
            using (new VerticalLayoutBlock(General2.Editors.EditorStyles.boxStyle))
            {
                DrawPropertyOrOverride(_layoutShape, t.layoutShape, overrides);
                DrawPropertyOrOverride(_maxStackSize, t.maxStackSize, overrides);
                _maxStackSize.intValue = Mathf.Clamp(_maxStackSize.intValue, 1, 9999);
                
                DrawPropertyOrOverride(_weight, t.weight, overrides);
                _weight.floatValue = Mathf.Clamp(_weight.floatValue, 0f, 9999f);
            }
            
            DoDrawWithOrder(30, 39);
            
            EditorGUILayout.LabelField(new GUIContent("Financial"), General2.Editors.EditorStyles.titleStyle);
            using (new VerticalLayoutBlock(General2.Editors.EditorStyles.boxStyle))
            {
                if (_buyPrice.arraySize == 0)
                {
                    GUI.color = new Color(1f, 0.6f, 0f, GUI.color.a);
                }
                DrawPropertyOrOverride(_buyPrice, t.buyPrice, overrides);
                GUI.color = Color.white;
                
                
                if (_sellPrice.arraySize == 0)
                {
                    GUI.color = new Color(1f, 0.6f, 0f, GUI.color.a);
                }
                DrawPropertyOrOverride(_sellPrice, t.sellPrice, overrides);
                GUI.color = Color.white;

            }
            
            DoDrawWithOrder(40, 49);
        }
    }
}