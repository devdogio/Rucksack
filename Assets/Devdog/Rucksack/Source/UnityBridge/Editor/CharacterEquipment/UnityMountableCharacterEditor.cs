using Devdog.General2.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.CharacterEquipment.Editor
{
    [CustomEditor(typeof(UnityEquippableCharacter), true)]
    public class UnityMountableCharacterEditor : UnityEditor.Editor
    {

        private void OnEnable()
        {
            var r = (UnityEquippableCharacter) target;
            if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            
            r.UpdateMountPoints();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var r = (UnityEquippableCharacter) target;
            using (new VerticalLayoutBlock())
            {
                EditorGUILayout.LabelField("Found: ", r.mountPoints.Length + " mount points");
                EditorGUI.indentLevel++;

                foreach (var point in r.mountPoints)
                {
                    if (point == null)
                    {
                        GUI.color = Color.red;
                        EditorGUILayout.LabelField("<NULL>");
                        GUI.color = Color.white;
                        continue;
                    }

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(point.name, point.GetType().Name);
                    var obj = point as UnityEngine.Object;
                    if (obj != null)
                    {
                        if (GUILayout.Button("Select", "minibutton", GUILayout.Width(80)))
                        {
                            Selection.activeObject = obj;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUI.indentLevel--;
            }
        }
    }
}