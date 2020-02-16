using Devdog.Rucksack.Collections;
using UnityEditor;

namespace Devdog.Rucksack.UI.Editor
{
    [CustomEditor(typeof(ItemCollectionSlotUI))]
    public class ItemCollectionSlotUIEditor : UnityEditor.Editor
    {
        
        public override void OnInspectorGUI()
        {
            if (EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var t = (ItemCollectionSlotUI)target;

                EditorGUILayout.BeginVertical("box");
                
                // CollectionUI might get destroyed when exiting game mode...
                if (t.collectionUI != null)
                {
                    EditorGUILayout.LabelField("Runtime Info:", UnityEditor.EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Item: ", t.current?.ToString());
                    EditorGUILayout.LabelField("Amount: ", (t.current as ICollectionSlotEntry)?.collectionEntry?.amount + "/" + t.current?.maxStackSize);
                    EditorGUILayout.LabelField("Type: ", t.current?.GetType().Name);
                }
                
                EditorGUILayout.EndVertical();
            }
            
            base.OnInspectorGUI();
        }
    }
}