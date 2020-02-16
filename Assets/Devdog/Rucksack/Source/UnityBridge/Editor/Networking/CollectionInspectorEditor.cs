using System;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Collections.Editor
{
    public class CollectionInspectorEditor : EditorWindow
    {
        private enum Source
        {
            Server,
            Client
        }
        
        private static CollectionInspectorEditor _window;
        private static Vector2 _scrollPosition;

        public static string collectionNameOrGuid { get; set; }
        
        private static Source _source;
        private static int _columnCount = 4;
        
        [MenuItem("Tools/" + RucksackConstants.ProductName + "/Collection inspector", false)]
        public static void ShowWindow()
        {
            _window = GetWindow<CollectionInspectorEditor>();
            _window.name = "Collection inspector";
            _window.Show();
        }

        private void OnGUI()
        {
            _columnCount = Mathf.Max(EditorGUILayout.IntField("Column Count", _columnCount), 1);
            _source = (Source)EditorGUILayout.EnumPopup("Source", _source);
            collectionNameOrGuid = EditorGUILayout.TextField("Collection name/GUID", collectionNameOrGuid);
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

            if (string.IsNullOrEmpty(collectionNameOrGuid) == false)
            {
                System.Guid guid;
                if (System.Guid.TryParse(collectionNameOrGuid, out guid))
                {
                    // It's a GUID
                    if (_source == Source.Server)
                    {
                        var col = ServerCollectionRegistry.byID.Get(guid);
                        DrawCollection(col);
                    } 
                    else if (_source == Source.Client)
                    {
                        var col = CollectionRegistry.byID.Get(guid);
                        DrawCollection(col);
                    }
                }
                else
                {
                    // It's a collection name
                    if (_source == Source.Server)
                    {
//                        var col = ServerCollectionRegistry.byName.Get(collectionNameOrGuid);
//                        DrawCollection(col);
                    } 
                    else if (_source == Source.Client)
                    {
                        var col = CollectionRegistry.byName.Get(collectionNameOrGuid);
                        DrawCollection(col);
                    }
                }
            }

            
            EditorGUILayout.EndScrollView();
        }
        
        
        private void DrawCollection(ICollection collection)
        {
            if (collection == null)
            {
                EditorGUILayout.LabelField("Collection not found...");
                return;
            }

            EditorGUILayout.LabelField("Collection Type: ", GetFriendlyName(collection.GetType()));
            
            const int slotSize = 100;
            
            EditorGUILayout.BeginVertical();
            int row = -1;
            for (int i = 0; i < collection.slotCount; i++)
            {
                if (i % _columnCount == 0)
                {
                    row++;
                }
                
                GUI.BeginGroup(new Rect((i % _columnCount) * slotSize + 10f, row * slotSize + 40f, slotSize, slotSize), GUIContent.none, "box");

                var vendorCol = collection as IReadOnlyCollection<IVendorProduct<IItemInstance>>;
                if (vendorCol != null)
                {
                    EditorGUI.LabelField(new Rect(0,10f,slotSize, 20f), vendorCol[i]?.ToString());
                    EditorGUI.LabelField(new Rect(0,30f,slotSize, 20f), vendorCol[i]?.ID.ToString());
                    EditorGUI.LabelField(new Rect(0,50f,slotSize, 20f), vendorCol.GetAmount(i) + "/" + vendorCol[i]?.maxStackSize, EditorStyles.centeredGreyMiniLabel);
                }

                var itemCol = collection as IReadOnlyCollection<IItemInstance>;
                if(itemCol != null)
                {
                    EditorGUI.LabelField(new Rect(0,10f,slotSize, 20f), itemCol[i]?.ToString());
                    EditorGUI.LabelField(new Rect(0,30f,slotSize, 20f), itemCol[i]?.ID.ToString());
                    EditorGUI.LabelField(new Rect(0,50f,slotSize, 20f), itemCol.GetAmount(i) + "/" + itemCol[i]?.maxStackSize, EditorStyles.centeredGreyMiniLabel);
                }
                
                GUI.EndGroup();
            }
            EditorGUILayout.EndVertical();
        }
        
        public static string GetFriendlyName(Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = typeParameters[i].Name;
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }
    }
}