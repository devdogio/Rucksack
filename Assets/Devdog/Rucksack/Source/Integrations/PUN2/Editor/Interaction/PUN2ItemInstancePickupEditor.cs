using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Items.Editor
{
    [CustomEditor(typeof(PUN2ItemInstancePickup))]
    public class PUN2ItemInstancePickupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
            {
                GUILayout.BeginVertical("box");

                var t = (PUN2ItemInstancePickup)target;
                GUILayout.Label(new GUIContent("InstanceID: "));
                GUILayout.Label(t.itemInstance?.ID.ToString() ?? "<NOT FOUND>");

                GUILayout.EndVertical();
            }
        }
    }
}