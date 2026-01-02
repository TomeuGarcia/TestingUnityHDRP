using UnityEditor;
using UnityEngine;



namespace TextureGeneration
{

    [CustomEditor(typeof(AlphabetTextureDrawerTester))]
    public class DynamicTextureGeneratorEditor : UnityEditor.Editor
    {
        private AlphabetTextureDrawerTester _dynamicTextureGenerator;


        private void OnEnable()
        {
            _dynamicTextureGenerator = (AlphabetTextureDrawerTester)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Color originalColor = GUI.color;
            GUI.color = Color.LerpUnclamped(Color.green, Color.cyan, 0.75f);

            if (GUILayout.Button("Draw Text Entries"))
            {
                _dynamicTextureGenerator.DrawTextEntries();
            }

            GUI.color = originalColor;
        }
    }

}