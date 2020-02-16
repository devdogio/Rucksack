using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Currencies.Editor
{
    [CustomPropertyDrawer(typeof(UnityCurrencyDecorator))]
    public class UnityCurrencyDecoratorEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var currency = property.FindPropertyRelative("currency");
            var amount = property.FindPropertyRelative("amount");

            var r = position;
            r.width *= 0.8f;
            
            EditorGUI.PropertyField(r, currency, true);

            r.x += r.width;
            r.width = position.width * 0.2f;
            EditorGUI.PropertyField(r, amount, new GUIContent(""));
            if (amount.floatValue < 0f)
            {
                amount.floatValue = 0f;
            }
            
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}