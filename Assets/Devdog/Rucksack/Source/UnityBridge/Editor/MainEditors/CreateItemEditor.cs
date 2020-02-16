using Devdog.General2;
using Devdog.General2.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Items.Editor
{
    public class CreateItemEditor : EditorWindow
    {

        //private Editor itemPicker;
        private ScriptPickerEditor _picker;
        private System.Type _firstStepType;
        public bool forceFocus { get; private set; }
        public System.Action<System.Type, EditorWindow> callback { get; set; }

        public static EditorWindow Get(System.Action<System.Type, EditorWindow> callback, string windowTitle = "Create new item")
        {
            var window = EditorWindow.GetWindow<CreateItemEditor>(true, "Create a new item", true);
            window.minSize = new Vector2(400, 500);
            window.maxSize = new Vector2(400, 500);
            window.titleContent = new GUIContent(windowTitle);
            window.callback = callback;
            window.forceFocus = false;

            return window;
        }

        public void OnEnable()
        {
            Reset();
        }

        public void Reset()
        {
            _firstStepType = null;

            _picker = ScriptPickerEditor.Get(typeof(UnityItemDefinition));
            _picker.Show(true);
            _picker.Close();

            _picker.OnPickObject += type =>
            {
                _firstStepType = type;
                Repaint();
            };

            Focus();
        }

        public void OnGUI()
        {
            // Recompiled or something?? No callback found
            if (callback == null)
            {
                Close();
            }

            if (forceFocus)
            {
                EditorWindow.FocusWindowIfItsOpen<CreateItemEditor>();
            }

            EditorGUILayout.BeginVertical();
            GUILayout.Space(30);

            Step0();

            EditorGUILayout.EndVertical();
        }


        public void Step0()
        {
            if (_firstStepType == null)
            {
                if (Event.current.isKey)
                {
                    _picker.Repaint();
                    Repaint();
                }
                
                _picker.OnGUI();
            }
            else
            {
                EditorGUILayout.LabelField("Selected type: " + _firstStepType.Name, (GUIStyle)"BoldLabel");
                if (GUILayout.Button("Create item", (GUIStyle) "LargeButton"))
                {
                    CreateItem(_firstStepType);
                }
            }
        }

        private void CreateItem(System.Type type)
        {
            if (callback != null)
                callback(type, this);
        }
    }
}