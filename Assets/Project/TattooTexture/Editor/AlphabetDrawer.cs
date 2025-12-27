using UnityEditor;
using UnityEngine;



namespace TextureGeneration
{
    [CustomPropertyDrawer(typeof(Alphabet))]
    public class AlphabetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.LabelField("ALPHABET");

            EditorGUILayout.PropertyField(property.FindPropertyRelative("_alphabetTexture"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_renderTextureFormat"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_renderTextureReadWrite"));

            EditorGUILayout.IntSlider(property.FindPropertyRelative("_rows"), 1, 16);
            EditorGUILayout.IntSlider(property.FindPropertyRelative("_columns"), 1, 16);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ELEMENTS");

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


            if (list.isExpanded)
            {
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
                            r.x += 1; r.y -= 1;
                            r.width -= 2; r.height -= 2;
                            EditorGUI.DrawRect(r, Color.LerpUnclamped(Color.gray, Color.black, 0.75f));

                            string t = ((char)(serializedProperty_character.intValue)).ToString();
                            t = EditorGUILayout.TextField(t, style, options);
                            serializedProperty_character.intValue = t.Length > 0 ? t[0] : 0;

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
            }

            //EditorGUI.indentLevel -= 1;
        }

    }

}