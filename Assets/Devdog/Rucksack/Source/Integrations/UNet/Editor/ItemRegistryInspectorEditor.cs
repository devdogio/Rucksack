using Devdog.Rucksack.Items;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Collections.Editor
{
    public class ItemRegistryInspectorEditor : EditorWindow
    {
        private enum Source
        {
            Server,
            Client
        }
        
        private static ItemRegistryInspectorEditor _window;
        private static Vector2 _scrollPosition;

        public static string itemGuid { get; set; }
        
        private static Source _source;
        
        [MenuItem("Tools/" + RucksackConstants.ProductName + "/Item inspector", false)]
        public static void ShowWindow()
        {
            _window = GetWindow<ItemRegistryInspectorEditor>();
            _window.name = "Item inspector";
            _window.Show();
        }

        private void OnGUI()
        {
            _source = (Source)EditorGUILayout.EnumPopup("Source", _source);
            itemGuid = EditorGUILayout.TextField("Item GUID", itemGuid);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

            if (string.IsNullOrEmpty(itemGuid))
            {
                if (_source == Source.Server)
                {
                    var items = ServerItemRegistry.GetAll();
                    foreach (var item in items)
                    {
                        DrawItemInfo(item);
                    }
                }
                else if (_source == Source.Client)
                {
                    var items = ItemRegistry.GetAll();
                    foreach (var item in items)
                    {
                        DrawItemInfo(item);
                    }
                }
            }
            else
            {
                System.Guid guid;
                if (System.Guid.TryParse(itemGuid, out guid))
                {
                    if (_source == Source.Server)
                    {
                        var item = ServerItemRegistry.Get(guid);
                        DrawItemInfo(item);
                    }
                    else if (_source == Source.Client)
                    {
                        var col = ItemRegistry.Get(guid);
                        DrawItemInfo(col);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("guid is not a valid parsable GUID value");
                }
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        
        private void DrawItemInfo(IItemInstance item)
        {
            if (item == null)
            {
                EditorGUILayout.LabelField("Collection not found...");
                return;
            }
            
            var col = item as IUnityItemInstance;

            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.LabelField("Item: " + item, UnityEditor.EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Guid: " + item.ID);
            EditorGUILayout.LabelField("Definition: " + item.itemDefinition.ID);
            EditorGUILayout.LabelField("Type: " + item.GetType().Name);

            if (col?.collectionEntry != null)
            {
                EditorGUILayout.LabelField("Amount: " + col.collectionEntry.amount + "/" + col.itemDefinition.maxStackSize);

                var unetCol = col.collectionEntry.collection as IUNetCollection;
                if (unetCol != null)
                {
                    EditorGUILayout.LabelField("Collection: " + unetCol.collectionName, UnityEditor.EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Guid: " + unetCol.ID);
                    
                    if (GUILayout.Button("Inspect collection", "minibutton", GUILayout.Width(130f)))
                    {
                        CollectionInspectorEditor.ShowWindow();
                        CollectionInspectorEditor.collectionNameOrGuid = unetCol.ID.ToString();
                    }   

                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}