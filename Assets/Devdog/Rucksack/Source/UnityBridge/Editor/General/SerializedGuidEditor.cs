using System;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
	[CustomPropertyDrawer(typeof(SerializedGuid))]
	public class SerializedGuidEditor : PropertyDrawer
	{
		private const int Offset = 0;
		private const float LabelScale = 0.9f;
		private const float ButtonScale = 0.1f;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var originalPosition = position;
			
			position.width *= LabelScale;
			position.width -= Offset;

			var b = property.FindPropertyRelative("_guidBytes");
			var bytes1 = new byte[16];
			for (int i = 0; i < b.arraySize; i++)
			{
				bytes1[i] = (byte)b.GetArrayElementAtIndex(i).intValue;
			}
			
			EditorGUI.LabelField(position, label, new GUIContent(new System.Guid(bytes1).ToString()));

			if (Event.current.type == EventType.ContextClick)
			{
				if (position.Contains(Event.current.mousePosition))
				{
					var menu = new GenericMenu();
 
					menu.AddItem(new GUIContent("Copy GUID"), false, () =>
					{
						GUIUtility.systemCopyBuffer = new System.Guid(bytes1).ToString();
					});

					System.Guid parsedGuid;
					var parsed = System.Guid.TryParse(GUIUtility.systemCopyBuffer, out parsedGuid);
					if (parsed && parsedGuid != System.Guid.Empty)
					{
						menu.AddItem(new GUIContent("Paste GUID"), false, () =>
						{
							SetGuid(b, parsedGuid);
						});
					}
					else
					{
						menu.AddDisabledItem(new GUIContent("Paste GUID"));
					}
					
					menu.ShowAsContext();
 
					Event.current.Use(); 
				}
			}
			
			position.x = originalPosition.x + position.width + Offset;
			position.width = originalPosition.width * ButtonScale;

			if (GUI.Button(position, "Gen", "MiniButton"))
			{
				SetGuid(b, System.Guid.NewGuid());
			}

			EditorGUI.EndProperty();
		}

		private void SetGuid(SerializedProperty serializedProperty, Guid newGuid)
		{
			var bytes = newGuid.ToByteArray();
			serializedProperty.arraySize = bytes.Length;
			for (int i = 0; i < bytes.Length; i++)
			{
				serializedProperty.GetArrayElementAtIndex(i).intValue = bytes[i];
			}

			serializedProperty.serializedObject.ApplyModifiedProperties();
		}
	}
}