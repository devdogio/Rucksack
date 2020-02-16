using Devdog.General2.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.Rucksack.Items.Editor
{
    [CustomPropertyDrawer(typeof(UnityShape2D))]
    public class UnityShape2DEditor : PropertyDrawer
    {
        public const string RowsName = "_shape";
        public const string ColumnsName = "cols";
        
        public const int BlockSize = 40;
        public const int ControlsSize = 50;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (property.FindPropertyRelative(RowsName).arraySize + 1) * BlockSize + ControlsSize;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rows = property.FindPropertyRelative(RowsName);
            var columnCount = GetColumnCount(property);

//            var startPos = position;
            var labelPos = position;
            labelPos.width = EditorGUIUtility.labelWidth;
            GUI.Label(labelPos, label);
            
            position.x += EditorGUIUtility.labelWidth;
            for (int x = 0; x < rows.arraySize; x++)
            {
                var row = rows.GetArrayElementAtIndex(x);
                var cols = row.FindPropertyRelative(ColumnsName);
                for (int y = 0; y < cols.arraySize; y++)
                {
                    var col = cols.GetArrayElementAtIndex(y);
                    var r = new Rect(position.x + (x * BlockSize), position.y + (y * BlockSize), BlockSize, BlockSize);

                    if (col.boolValue)
                    {
                        // Checked
                        GUI.color = new Color(0f, 1f, 0f, GUI.color.a);
                    }
                    
                    if (GUI.Button(r, "", General2.Editors.EditorStyles.boxStyle))
                    {
                        col.boolValue = !col.boolValue;
                    }

                    GUI.color = new Color(1f, 1f, 1f, GUI.color.a);
                }
            }
            
            
            var addColRect = new Rect(position.x + (rows.arraySize * BlockSize), position.y, BlockSize, BlockSize);
            if (GUI.Button(addColRect, new GUIContent("+", "Add a column"), General2.Editors.EditorStyles.boxStyle))
            {
                rows.InsertArrayElementAtIndex(rows.arraySize);
                var cols = rows.GetArrayElementAtIndex(rows.arraySize - 1).FindPropertyRelative(ColumnsName);
                cols.arraySize = GetColumnCount(property);
            }

            addColRect.y += BlockSize;
            if (rows.arraySize > 1 && GUI.Button(addColRect, new GUIContent("-", "Remove a column"), General2.Editors.EditorStyles.boxStyle))
            {
                rows.DeleteArrayElementAtIndex(rows.arraySize - 1);
            }
            
            var addRowRect = new Rect(position.x, position.y + (GetColumnCount(property) * BlockSize), BlockSize, BlockSize);
            if (GUI.Button(addRowRect, new GUIContent("+", "Add a row"), General2.Editors.EditorStyles.boxStyle))
            {
                for (int x = 0; x < rows.arraySize; x++)
                {
                    var cols = rows.GetArrayElementAtIndex(x).FindPropertyRelative(ColumnsName);
                    cols.arraySize = columnCount + 1;
                }
            }

            addRowRect.x += BlockSize;
            if (columnCount > 1 && GUI.Button(addRowRect, new GUIContent("-", "Remove a row"), General2.Editors.EditorStyles.boxStyle))
            {
                if (columnCount > 1)
                {
                    for (int x = 0; x < rows.arraySize; x++)
                    {
                        var cols = rows.GetArrayElementAtIndex(x).FindPropertyRelative(ColumnsName);
                        cols.arraySize = columnCount - 1;
                    }
                }
            }
            
            property.serializedObject.ApplyModifiedProperties();
        }

        protected int GetColumnCount(SerializedProperty property, int column = 0)
        {
            var rows = property.FindPropertyRelative(RowsName);
            return rows.GetArrayElementAtIndex(column).FindPropertyRelative(ColumnsName).arraySize;
        }
    }
}