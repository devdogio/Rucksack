using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.General2.Editors;
using UnityEditor;
using UnityEngine;

using Photon.Pun;

namespace Devdog.Rucksack.Collections.Editor
{
    public class PUN2CollectionsDebuggerEditorWindow : EditorWindow
    {
        private enum Source
        {
            ServerAll,
            ServerFromSelection,
            ServerRegistry,
            ClientRegistry
        }

        private static PUN2CollectionsDebuggerEditorWindow _window;
        private static Vector2 _scrollPosition;
        private static Source _source;

        [MenuItem("Tools/" + RucksackConstants.ProductName + "/PUN2 Collection Debugger", false)]
        public static void ShowWindow()
        {
            Selection.selectionChanged += SelectionChanged;
            _window = GetWindow<PUN2CollectionsDebuggerEditorWindow>();
            _window.name = "Collection debugger";
            _window.Show();
        }

        private static void SelectionChanged()
        {
            if (_window != null)
            {
                _window.Repaint();
            }
        }

        private void OnGUI()
        {
            var obj = Selection.activeObject as GameObject;
            var identity = obj?.GetComponent<PhotonView>();

            _source = (Source)EditorGUILayout.EnumPopup(new GUIContent("Show types"), _source);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
            if (_source == Source.ServerAll)
            {
                EditorGUILayout.LabelField("[Server] ALL with permission", UnityEditor.EditorStyles.boldLabel);
                foreach (var val in PUN2PermissionsRegistry.collections.GetAll())
                {
                    DrawCollection(val.Item1.obj, val.Item1.permission, val.Item2);
                }
            }
            else if (_source == Source.ServerRegistry)
            {
                EditorGUILayout.LabelField("[Server] Collection registry", UnityEditor.EditorStyles.boldLabel);
                //                EditorGUILayout.LabelField("By Name", UnityEditor.EditorStyles.boldLabel);
                //                var cols2 = ServerCollectionRegistry.byName.values;
                //                foreach (var col in cols2)
                //                {
                //                    DrawCollection(col as IPUN2Collection, null);
                //                }

                EditorGUILayout.LabelField("By ID", UnityEditor.EditorStyles.boldLabel);
                var cols3 = ServerCollectionRegistry.byID.values;
                foreach (var col in cols3)
                {
                    DrawCollection(col as IPUN2Collection, null);
                }
            }
            else if (_source == Source.ClientRegistry)
            {
                EditorGUILayout.LabelField("[Client] Collection registry", UnityEditor.EditorStyles.boldLabel);
                EditorGUILayout.LabelField("By Name", UnityEditor.EditorStyles.boldLabel);
                var cols2 = CollectionRegistry.byName.values;
                foreach (var col in cols2)
                {
                    DrawCollection(col as IPUN2Collection, null);
                }

                EditorGUILayout.LabelField("By ID", UnityEditor.EditorStyles.boldLabel);
                var cols3 = CollectionRegistry.byID.values;
                foreach (var col in cols3)
                {
                    DrawCollection(col as IPUN2Collection, null);
                }
            }
            else if (_source == Source.ServerFromSelection)
            {
                if (identity == null)
                {
                    EditorGUILayout.LabelField("No NetworkIdentity found on selected object...");
                }
                else
                {
                    EditorGUILayout.LabelField("[Server] Selection permissions", UnityEditor.EditorStyles.boldLabel);
                    var cols = PUN2PermissionsRegistry.collections.GetAllWithPermission(identity);
                    foreach (var col in cols)
                    {
                        DrawCollection(col.obj, col.permission);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }


        private void DrawCollection(IPUN2Collection collection, PhotonView identity)
        {
            if (collection == null)
            {
                return;
            }

            ReadWritePermission? permission = null;
            if (identity != null)
            {
                permission = PUN2PermissionsRegistry.collections.GetPermission(collection, identity);
            }

            DrawCollection(collection, permission.GetValueOrDefault());
        }

        private void DrawCollection(IPUN2Collection collection, ReadWritePermission permission)
        {
            DrawCollection(collection, permission, new List<PhotonView>());
        }

        private void DrawCollection(IPUN2Collection collection, ReadWritePermission permission, IEnumerable<PhotonView> identities)
        {
            var col = collection as ICollection;
            if (collection == null || col == null)
            {
                return;
            }

            using (new VerticalLayoutBlock("box"))
            {
                EditorGUILayout.LabelField(new GUIContent("Collection Name"), new GUIContent(collection.collectionName), UnityEditor.EditorStyles.boldLabel);
                EditorGUILayout.LabelField(new GUIContent("GUID"), new GUIContent(collection.ID.ToString()));
                EditorGUILayout.LabelField(new GUIContent("Permissions"), new GUIContent(permission.ToString()));
                EditorGUILayout.LabelField(new GUIContent("Type"), new GUIContent(GetFriendlyName(collection.GetType())));
                EditorGUILayout.LabelField(new GUIContent("Slot count"), new GUIContent(col.slotCount.ToString()));
                EditorGUILayout.LabelField(new GUIContent("Owner ViewID"), new GUIContent(collection.owner?.ViewID.ToString()));

                if (identities.Any())
                {
                    using (new VerticalLayoutBlock("box"))
                    {
                        EditorGUILayout.LabelField(new GUIContent("All identities with access"));
                        foreach (var identity in identities)
                        {
                            if (identity == null)
                            {
                                EditorGUILayout.LabelField("<NULL RECORD>");
                                continue;
                            }

                            EditorGUILayout.BeginHorizontal();

                            if (identity == collection.owner)
                            {
                                GUI.color = Color.green;
                            }

                            EditorGUILayout.LabelField(new GUIContent(identity.name), GUILayout.Width(150f));
                            EditorGUILayout.LabelField(new GUIContent("ViewID: " + identity.ViewID), GUILayout.Width(80f));
                            EditorGUILayout.LabelField(new GUIContent(PUN2PermissionsRegistry.collections.GetPermission(collection, identity).ToString()), GUILayout.Width(100));
                            if (GUILayout.Button("Select", "minibutton"))
                            {
                                Selection.activeObject = identity.gameObject;
                            }

                            GUI.color = Color.white;

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }


                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select"))
                {
                    Selection.activeGameObject = collection.owner?.gameObject;
                }
                if (GUILayout.Button("Inspect"))
                {
                    CollectionInspectorEditor.ShowWindow();
                    CollectionInspectorEditor.collectionNameOrGuid = collection.ID.ToString();
                }
                EditorGUILayout.EndHorizontal();
            }
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