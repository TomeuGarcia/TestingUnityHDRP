using UnityEditor;
using UnityEngine;



namespace TextureGeneration
{
    [CustomPropertyDrawer(typeof(Alphabet))]
    public class AlphabetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.LabelField("Alphabet Elements");
            EditorGUILayout.IntSlider(property.FindPropertyRelative("_rows"), 1, 16);
            EditorGUILayout.IntSlider(property.FindPropertyRelative("_columns"), 1, 16);
            EditorGUILayout.Space();
            DrawElementsList(
                property.FindPropertyRelative("_elements"), 
                property.FindPropertyRelative("_rows").intValue, 
                property.FindPropertyRelative("_columns").intValue
            );
        }


        private void DrawElementsList(SerializedProperty list, int rows, int columns)
        {
            const float width = 120f;
            GUIStyle style = new GUIStyle(EditorStyles.textField);
            style.alignment = TextAnchor.MiddleCenter;
            GUIStyleState normal = style.normal;
            normal.textColor = Color.LerpUnclamped(Color.white, Color.cyan, 0.5f);
            GUIStyleState hover = style.hover;
            hover.textColor = Color.LerpUnclamped(Color.white, Color.yellow, 0.5f);

            GUILayoutOption[] areaOptions = new[] { GUILayout.Width(width), GUILayout.Height(18 + 20) };
            GUILayoutOption[] options = new[] { GUILayout.Width(width) };

            int originalIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;
            EditorGUILayout.BeginVertical();

            int i = 0;
            for (int rowI = 0; rowI < rows; ++rowI)
            {
                EditorGUILayout.BeginHorizontal();
                for (int columnI = 0; columnI < columns; ++columnI)
                {
                    if (i < list.arraySize)
                    {
                        SerializedProperty element = list.GetArrayElementAtIndex(i);

                        SerializedProperty serializedProperty_character = element.FindPropertyRelative("_character");
                        SerializedProperty serializedProperty_widthRatio = element.FindPropertyRelative("_widthRatio");

                        Rect r = EditorGUILayout.BeginVertical(areaOptions);
                        r.x += 1 + 9; r.y -= 1;
                        r.width -= 2 + 4; r.height -= 1;
                        EditorGUI.DrawRect(r, Color.LerpUnclamped(Color.gray, Color.black, 0.75f));

                        string t = ((char)(serializedProperty_character.intValue)).ToString();
                        t = EditorGUILayout.TextField(t, style, options);
                        serializedProperty_character.intValue = t.Length > 0 ? t[0] : 0;

                        //EditorGUILayout.LabelField("W:", options);
                        EditorGUILayout.Slider(serializedProperty_widthRatio, 0.0f, 1.0f, GUIContent.none, options);

                        EditorGUILayout.EndVertical();
                    }

                    ++i;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(2);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);

            EditorGUI.indentLevel = originalIndentLevel;
        }

    }

}