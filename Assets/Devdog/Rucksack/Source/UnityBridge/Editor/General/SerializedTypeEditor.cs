using System;
using Devdog.General2.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
	[CustomPropertyDrawer(typeof(SerializedType))]
	public class SerializedTypeEditor : PropertyDrawer
	{
		private const float LabelScale = 0.9f;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var original = position;
			
			var l = position;
			l.width = EditorGUIUtility.labelWidth;
			EditorGUI.LabelField(l, label);

			position.width -= l.width;
			position.x += l.width;
			
			var b = property.FindPropertyRelative("_assemblyQualifiedName");
			Type type = null;
			try
			{
				type = Type.GetType(b.stringValue);
			}
			catch (Exception)
			{
				// Ignored
			}
			
			GUI.enabled = false;
			EditorGUI.LabelField(position, type != null ? type.Name : "(NOT SET)");
			GUI.enabled = true;

			position.x = original.x + original.width - 60;
			position.width = 60;
			position.height = 14;
			position.y += 2;
			if (GUI.Button(position, "Set", "minibutton"))
			{
				var typePicker = ScriptPickerEditor.Get(typeof(object));
				typePicker.Show();
				typePicker.OnPickObject += pickedType =>
				{
					b.stringValue = pickedType.AssemblyQualifiedName;
					GUI.changed = true;
					b.serializedObject.ApplyModifiedProperties();
				};
			}
			
			EditorGUI.EndProperty();
		}
	}
}