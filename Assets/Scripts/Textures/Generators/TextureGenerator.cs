using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noise.Generators.Textures
{
    public sealed class TextureGenerator : MonoBehaviour
    {
        [Header("Texture Generator Settings")]
        [SerializeField] private string textureName = "Generated Texture";
        [SerializeField] private int resolution = 256;

        [Header("Random Noise Settings")]
        [SerializeField] private RandomNoise randomNoiseSettings;

        [Header("Lattice Noise Settings")]
        [SerializeField] private LatticeNoise latticeNoiseSettings;

        [Header("Value Noise Settings")]
        [SerializeField] private ValueNoise valueNoiseSettings;

        [Header("Perlin Noise Settings")]
        [SerializeField] private PerlinNoise perlinNoiseSettings;

        [Header("Fractal Noise Settings")]
        [SerializeField] private FractalNoise fractalNoiseSettings;

        [Header("Worley Noise Settings")]
        [SerializeField] private WorleyNoise worleyNoiseSettings;

        [HideInInspector] public bool showRandomNoiseSettings = false;
        [HideInInspector] public bool showLatticeNoiseSettings = false;
        [HideInInspector] public bool showValueNoiseSettings = false;
        [HideInInspector] public bool showPerlinNoiseSettings = false;
        [HideInInspector] public bool showFractalNoiseSettings = false;
        [HideInInspector] public bool showWorleyNoiseSettings = false;

        private Texture2D generatedTexture;

        private NoiseGenerator.NoiseMethod noiseMethod;

        public RandomNoise RandomNoiseSettings { get => randomNoiseSettings; }
        public LatticeNoise LatticeNoiseSettings { get => latticeNoiseSettings; }
        public ValueNoise ValueNoiseSettings { get => valueNoiseSettings; }
        public PerlinNoise PerlinNoiseSettings { get => perlinNoiseSettings; }
        public FractalNoise FractalNoiseSettings { get => fractalNoiseSettings; }
        public WorleyNoise WorleyNoiseSettings { get => worleyNoiseSettings; }

        private void Awake()
        {
            generatedTexture = new Texture2D(resolution, resolution, TextureFormat.RGB48, true);
            generatedTexture.name = textureName;
            generatedTexture.wrapMode = TextureWrapMode.Clamp;
            generatedTexture.filterMode = FilterMode.Trilinear;
            generatedTexture.anisoLevel = 9;

            this.GetComponent<MeshRenderer>().material.mainTexture = generatedTexture;

            NoiseGenerator.Initialize(latticeNoiseSettings, valueNoiseSettings, perlinNoiseSettings, fractalNoiseSettings);
        }

        public void GenerateRandomValueNoiseTexture()
        {
            if (generatedTexture.width != resolution)
                generatedTexture.Reinitialize(resolution, resolution);

            Random.InitState(randomNoiseSettings.Seed);

            for (int x = 0; x < generatedTexture.width; x++)
                for (int y = 0; y < generatedTexture.height; y++)
                    generatedTexture.SetPixel(x, y, Color.white * Random.value);

            generatedTexture.Apply();
        }

        public void GenerateLatticeNoiseTexture()
        {
            if (generatedTexture.width != resolution)
                generatedTexture.Reinitialize(resolution, resolution);

            List<Vector3> quadLocalCoordinates = GetQuadMeshCornersLocalCoordinates();
            List<Vector3> quadWorldCoordinates = GetQuadMeshCornersWorldCoordinates(quadLocalCoordinates);

            float inverseResolution = 1.0f / resolution;

            latticeNoiseSettings.Initialize();

            for (int y = 0; y < generatedTexture.width; y++)
            {
                Vector3 leftColumnPoint = Vector3.Lerp(
                                                        quadWorldCoordinates[0],
                                                        quadWorldCoordinates[2],
                                                        (y + 0.5f) * inverseResolution
                                                       );

                Vector3 rightColumnPoint = Vector3.Lerp(
                                                         quadWorldCoordinates[1],
                                                         quadWorldCoordinates[3],
                                                         (y + 0.5f) * inverseResolution
                                                        );

                for (int x = 0; x < generatedTexture.height; x++)
                {
                    Vector3 pointInWorldCoordinates = Vector3.Lerp(
                                                                    leftColumnPoint,
                                                                    rightColumnPoint,
                                                                    (x + 0.5f) * inverseResolution
                                                                   );

                    generatedTexture.SetPixel(x, y, Color.white *
                                                NoiseGenerator.LatticeNoiseValue(
                                                    pointInWorldCoordinates,
                                                    latticeNoiseSettings.Frequency
                                                )
                                              );
                }
            }

            generatedTexture.Apply();
        }

        public void GenerateValueNoiseTexture()
        {
            if (generatedTexture.width != resolution)
                generatedTexture.Reinitialize(resolution, resolution);

            List<Vector3> quadLocalCoordinates = GetQuadMeshCornersLocalCoordinates();
            List<Vector3> quadWorldCoordinates = GetQuadMeshCornersWorldCoordinates(quadLocalCoordinates);

            float inverseResolution = 1.0f / resolution;

            valueNoiseSettings.Initialize();

            for (int y = 0; y < generatedTexture.width; y++)
            {
                Vector3 leftColumnPoint = Vector3.Lerp(
                                                        quadWorldCoordinates[0],
                                                        quadWorldCoordinates[2],
                                                        (y + 0.5f) * inverseResolution
                                                       );

                Vector3 rightColumnPoint = Vector3.Lerp(
                                                         quadWorldCoordinates[1],
                                                         quadWorldCoordinates[3],
                                                         (y + 0.5f) * inverseResolution
                                                        );

                for (int x = 0; x < generatedTexture.height; x++)
                {
                    Vector3 pointInWorldCoordinates = Vector3.Lerp(
                                                                    leftColumnPoint,
                                                                    rightColumnPoint,
                                                                    (x + 0.5f) * inverseResolution
                                                                   );

                    generatedTexture.SetPixel(x, y, Color.white *
                                                NoiseGenerator.ValueNoiseValue(
                                                    pointInWorldCoordinates,
                                                    valueNoiseSettings.Frequency
                                                )
                                              );
                }
            }

            generatedTexture.Apply();
        }

        public void GeneratePerlinNoiseTexture()
        {
            if (generatedTexture.width != resolution)
                generatedTexture.Reinitialize(resolution, resolution);

            List<Vector3> quadLocalCoordinates = GetQuadMeshCornersLocalCoordinates();
            List<Vector3> quadWorldCoordinates = GetQuadMeshCornersWorldCoordinates(quadLocalCoordinates);

            float inverseResolution = 1.0f / resolution;

            perlinNoiseSettings.Initialize();

            for (int y = 0; y < generatedTexture.width; y++)
            {
                Vector3 leftColumnPoint = Vector3.Lerp(
                                                        quadWorldCoordinates[0],
                                                        quadWorldCoordinates[2],
                                                        (y + 0.5f) * inverseResolution
                                                       );

                Vector3 rightColumnPoint = Vector3.Lerp(
                                                         quadWorldCoordinates[1],
                                                         quadWorldCoordinates[3],
                                                         (y + 0.5f) * inverseResolution
                                                        );

                for (int x = 0; x < generatedTexture.height; x++)
                {
                    Vector3 pointInWorldCoordinates = Vector3.Lerp(
                                                                    leftColumnPoint,
                                                                    rightColumnPoint,
                                                                    (x + 0.5f) * inverseResolution
                                                                   );

                    generatedTexture.SetPixel(x, y, Color.white *
                                                NoiseGenerator.PerlinNoiseValue(
                                                    pointInWorldCoordinates,
                                                    perlinNoiseSettings.Frequency
                                                )
                                              );
                }
            }

            generatedTexture.Apply();
        }

        public void GenerateFractalNoiseTexture()
        {
            if (generatedTexture.width != resolution)
                generatedTexture.Reinitialize(resolution, resolution);

            List<Vector3> quadLocalCoordinates = GetQuadMeshCornersLocalCoordinates();
            List<Vector3> quadWorldCoordinates = GetQuadMeshCornersWorldCoordinates(quadLocalCoordinates);

            float inverseResolution = 1.0f / resolution;

            perlinNoiseSettings.Initialize();
            latticeNoiseSettings.Initialize();
            valueNoiseSettings.Initialize();

            for (int y = 0; y < generatedTexture.width; y++)
            {
                Vector3 leftColumnPoint = Vector3.Lerp(
                                                        quadWorldCoordinates[0],
                                                        quadWorldCoordinates[2],
                                                        (y + 0.5f) * inverseResolution
                                                       );

                Vector3 rightColumnPoint = Vector3.Lerp(
                                                         quadWorldCoordinates[1],
                                                         quadWorldCoordinates[3],
                                                         (y + 0.5f) * inverseResolution
                                                        );

                for (int x = 0; x < generatedTexture.height; x++)
                {
                    Vector3 pointInWorldCoordinates = Vector3.Lerp(
                                                                    leftColumnPoint,
                                                                    rightColumnPoint,
                                                                    (x + 0.5f) * inverseResolution
                                                                  );

                    if (fractalNoiseSettings.NoiseType == TypeInfos.FractalNoiseType.PERLIN)
                        noiseMethod = NoiseGenerator.PerlinNoiseValue;
                    else if (fractalNoiseSettings.NoiseType == TypeInfos.FractalNoiseType.LATTICE)
                        noiseMethod = NoiseGenerator.LatticeNoiseValue;
                    else
                        noiseMethod = NoiseGenerator.ValueNoiseValue;

                    float sample = NoiseGenerator.FractalNoiseValue(noiseMethod, pointInWorldCoordinates, fractalNoiseSettings.Frequency);

                    generatedTexture.SetPixel(x, y, Color.white * sample);
                }
            }

            generatedTexture.Apply();
        }

        public void GenerateWorleyNoiseTexture()
        {
            if (generatedTexture.width != resolution)
                generatedTexture.Reinitialize(resolution, resolution);

            worleyNoiseSettings.Initialize(resolution);

            for (int y = 0; y < generatedTexture.width; y++)
                for (int x = 0; x < generatedTexture.height; x++)
                {
                    Vector2 point = new Vector2(x, y);

                    float sample = NoiseGenerator.WorleyNoiseValue(point, resolution, worleyNoiseSettings);
                    sample = worleyNoiseSettings.ShouldInvertNoise ? worleyNoiseSettings.InversionValue - sample : sample;

                    generatedTexture.SetPixel(x, y, Color.white * sample);
                }

            generatedTexture.Apply();
        }

        private List<Vector3> GetQuadMeshCornersLocalCoordinates()
        {
            List<Vector3> quadCorners = new List<Vector3>();

            float quadHalfWidth = this.transform.localScale.x / 2;
            float quadHalfHeight = this.transform.localScale.y / 2;

            Vector3 bottomLeftCorner = new Vector3(-quadHalfWidth, -quadHalfHeight);
            Vector3 bottomRightCorner = new Vector3(quadHalfWidth, -quadHalfHeight);
            Vector3 topLeftCorner = new Vector3(-quadHalfWidth, quadHalfHeight);
            Vector3 topRightCorner = new Vector3(quadHalfWidth, quadHalfHeight);

            quadCorners.Add(bottomLeftCorner);
            quadCorners.Add(bottomRightCorner);
            quadCorners.Add(topLeftCorner);
            quadCorners.Add(topRightCorner);

            return quadCorners;
        }

        private List<Vector3> GetQuadMeshCornersWorldCoordinates(List<Vector3> localCoordinates)
        {
            List<Vector3> quadCorners = new List<Vector3>();

            foreach (var coordinate in localCoordinates)
                quadCorners.Add(this.transform.TransformPoint(coordinate));

            return quadCorners;
        }
    }
}
