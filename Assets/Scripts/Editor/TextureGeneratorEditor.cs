using System.Collections;
using System.Collections.Generic;
using Noise.Generators.Textures;
using UnityEditor;
using UnityEngine;

namespace Editors.Textures
{
    [CustomEditor(typeof(TextureGenerator))]
    public sealed class TextureGeneratorEditor : Editor
    {
        private TextureGenerator textureGenerator;

        private Editor randomNoiseEditor;
        private Editor latticeNoiseEditor;
        private Editor valueNoiseEditor;
        private Editor perlinNoiseEditor;
        private Editor fractalNoiseEditor;
        private Editor worleyNoiseEditor;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (textureGenerator.RandomNoiseSettings is not null)
                DrawSettingsEditor(textureGenerator.RandomNoiseSettings, ref textureGenerator.showRandomNoiseSettings, ref randomNoiseEditor);

            if (textureGenerator.LatticeNoiseSettings is not null)
                DrawSettingsEditor(textureGenerator.LatticeNoiseSettings, ref textureGenerator.showLatticeNoiseSettings, ref latticeNoiseEditor);

            if (textureGenerator.ValueNoiseSettings is not null)
                DrawSettingsEditor(textureGenerator.ValueNoiseSettings, ref textureGenerator.showValueNoiseSettings, ref valueNoiseEditor);

            if (textureGenerator.PerlinNoiseSettings is not null)
                DrawSettingsEditor(textureGenerator.PerlinNoiseSettings, ref textureGenerator.showPerlinNoiseSettings, ref perlinNoiseEditor);

            if (textureGenerator.FractalNoiseSettings is not null)
                DrawSettingsEditor(textureGenerator.FractalNoiseSettings, ref textureGenerator.showFractalNoiseSettings, ref fractalNoiseEditor);

            if (textureGenerator.FractalNoiseSettings is not null)
                DrawSettingsEditor(textureGenerator.WorleyNoiseSettings, ref textureGenerator.showWorleyNoiseSettings, ref worleyNoiseEditor);

            if (textureGenerator.RandomNoiseSettings && GUILayout.Button("Generate Random Value Noise Texture"))
                textureGenerator.GenerateRandomValueNoiseTexture();

            if (textureGenerator.LatticeNoiseSettings is not null &&  GUILayout.Button("Generate Lattice Noise Texture"))
                textureGenerator.GenerateLatticeNoiseTexture();

            if (textureGenerator.ValueNoiseSettings is not null && GUILayout.Button("Generate Value Noise Texture"))
                textureGenerator.GenerateValueNoiseTexture();

            if (textureGenerator.PerlinNoiseSettings is not null && GUILayout.Button("Generate Perlin Noise Texture"))
                textureGenerator.GeneratePerlinNoiseTexture();

            if (textureGenerator.FractalNoiseSettings is not null && GUILayout.Button("Generate Fractal Noise Texture"))
                textureGenerator.GenerateFractalNoiseTexture();

            if (textureGenerator.WorleyNoiseSettings is not null && GUILayout.Button("Generate Worley Noise Texture"))
                textureGenerator.GenerateWorleyNoiseTexture();
        }

        private void DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor)
        {
            if (settings != null)
            {
                foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    if (foldout)
                    {
                        CreateCachedEditor(settings, null, ref editor);
                        editor.OnInspectorGUI();
                    }
                }
            }
        }

        private void OnEnable() => textureGenerator = target as TextureGenerator;
    }
}
