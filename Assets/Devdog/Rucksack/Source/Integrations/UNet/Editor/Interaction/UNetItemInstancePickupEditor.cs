using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Items.Editor
{
    [CustomEditor(typeof(UNetItemInstancePickup))]
    public class UNetItemInstancePickupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
            {
                GUILayout.BeginVertical("box");

                var t = (UNetItemInstancePickup) target;
                GUILayout.Label(new GUIContent("InstanceID: "));
                GUILayout.Label(t.itemInstance?.ID.ToString() ?? "<NOT FOUND>");
                
                GUILayout.EndVertical();
            }
        }
    }
}