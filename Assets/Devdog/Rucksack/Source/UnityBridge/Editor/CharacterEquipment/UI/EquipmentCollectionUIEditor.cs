using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.UI.Editor
{
    [CustomEditor(typeof(EquipmentCollectionUI))]
    public class EquipmentCollectionUIEditor : UnityEditor.Editor
    {

//        private void OnEnable()
//        {
//            var t = (EquipmentCollectionUI) target;
//            t.IndexSlotsAndMountPoints();
//        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                var t = (EquipmentCollectionUI) target;
                EditorGUILayout.LabelField("Indexed slots", t.collection?.slotCount.ToString());
            }
        }
    }
}