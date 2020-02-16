using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devdog.General2.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Items.Editor
{
    public abstract class ObjectEditorBase<T> : UnityEditor.Editor
    {
        protected readonly ILogger logger;
        protected readonly List<SerializedProperty> properties = new List<SerializedProperty>();
        protected readonly Dictionary<int, Action> queuedDrawingDict = new Dictionary<int, Action>();
        
        public ObjectEditorBase()
        {
            logger = new UnityLogger("[Editor] ");
        }
        
        protected virtual void OnEnable()
        {
            FindProperties();
        }

        protected virtual void FindProperties()
        {
            
        }

        protected void FindProperty(string findName, out SerializedProperty prop)
        {
            prop = serializedObject.FindProperty(findName);
            if (prop != null)
            {
                properties.Add(prop);
            }
            else
            {
                logger.Warning("Couldn't find property with name: " + findName, this);
            }
        }
        
        protected SerializedProperty GetPropertyWithoutAdding(string findName, out SerializedProperty prop)
        {
            prop = serializedObject.FindProperty(findName);
            if (prop == null)
            {
                logger.Warning("Couldn't find property with name: " + findName, this);
            }

            return prop;
        }
        
        protected void DrawWithOrder(int order, Action drawAction)
        {
            queuedDrawingDict[order] = drawAction;
        }

        protected void DoDrawWithOrder(int fromOrder, int toOrder)
        {
            for (int i = fromOrder; i < toOrder; i++)
            {
                Action action;
                if (queuedDrawingDict.TryGetValue(i, out action))
                {
                    action.Invoke();
                }
            }
        }
        
        protected void DrawSprite(Rect position, Sprite sprite)
        {
            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);
 
            Rect coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;
 
            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);
 
            Vector2 center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;
 
            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
        
        public sealed override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnInspecstorGUI(new Dictionary<string, Action<SerializedProperty>>());
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnInspecstorGUI(Dictionary<string, Action<SerializedProperty>> overrides)
        {
            queuedDrawingDict.Clear();
            foreach (var action in overrides)
            {
                var prop = serializedObject.FindProperty(action.Key);
                if (prop != null)
                {
                    action.Value?.Invoke(prop);
                }
            }
            
            DrawPropertiesExcluding(serializedObject, overrides.Keys.Concat(properties.Select(o => o.name)).ToArray());
        }
        
        protected void DrawReadOnlyPropertyOrOverride<TValueType>(SerializedProperty property, TValueType currentChainValue, Dictionary<string, Action<SerializedProperty>> overrides, bool drawChildren = true)
        {
            using (new GUIDisabledBlock())
            {
                DrawPropertyOrOverride(property, currentChainValue, overrides, drawChildren);
            }
        }

        protected void DrawPropertyOrOverride<TValueType>(SerializedProperty property, TValueType currentChainValue, Dictionary<string, Action<SerializedProperty>> overrides, bool drawChildren = true)
        {
            if (overrides.ContainsKey(property.name))
            {
                overrides[property.name].Invoke(property);
            }
            else
            {
                var inherits = DoesInheritFromParent(property.name, currentChainValue, property.serializedObject.targetObject as UnityItemDefinition);
                if (inherits)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                }
                
                EditorGUILayout.PropertyField(property, new GUIContent(property.displayName), drawChildren);

                if (inherits)
                {
                    var rect = GUILayoutUtility.GetLastRect();
                    rect.x += rect.width;
                    rect.width = EditorGUIUtility.singleLineHeight;
                    rect.height = rect.width;
                    rect.x -= rect.width;
                    rect.x -= 20f;
                    rect.y += 2f;

                    using (new ColorBlock(Color.white))
                    {
                        GUI.Label(rect, new GUIContent("", "Value in inherited from parent"), "Icon.ExtrapolationLoop");
                    }
                }
                
                GUI.color = Color.white;
            }
        }

        /// <summary>
        /// Get the currentChainValue of a parent object. If none is found null will be returned.
        /// </summary>
        /// <returns></returns>
        protected bool DoesInheritFromParent<TValueType>(string propertyName, TValueType currentChainValue, IItemDefinition startObj)
        {
            IItemDefinition parent = startObj.parent;
            while (parent != null)
            {
                var type = parent.GetType();
                var field = type.GetField(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var startValue = field?.GetValue(parent);
                if (Equals(startValue, currentChainValue))
                {
                    return true;
                }

                var startValueValue = Equals(startValue, GetDefault(type));
                var currentChainValueValue = Equals(currentChainValue, GetDefault(type));
                if (startValueValue && currentChainValueValue == false)
                {
                    return true;
                }
                
                parent = parent.parent;
            }

            return false;
        }
        
        protected static object GetDefault(Type type)
        {
            if(type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            
            return null;
        }
    }
}