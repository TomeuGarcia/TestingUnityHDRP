using UnityEditor;
using UnityEngine;



namespace TextureGeneration
{

    [CustomEditor(typeof(DynamicTextureGenerator))]
    public class DynamicTextureGeneratorEditor : UnityEditor.Editor
    {
        private DynamicTextureGenerator _dynamicTextureGenerator;


        private void OnEnable()
        {
            _dynamicTextureGenerator = (DynamicTextureGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Color originalColor = GUI.color;
            GUI.color = Color.LerpUnclamped(Color.green, Color.cyan, 0.75f);

            if (GUILayout.Button("Bake Text Entries"))
            {
                _dynamicTextureGenerator.BakeTextEntries();
            }

            GUI.color = originalColor;
        }
    }

}