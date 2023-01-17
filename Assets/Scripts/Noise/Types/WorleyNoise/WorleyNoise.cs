using System.Collections;
using System.Collections.Generic;
using Noise.Generators.TypeInfos;
using UnityEngine;

namespace Noise.Generators
{
    [CreateAssetMenu(fileName = "Worley Noise Settings", menuName = "Noise Generators/Worley Noise", order = 5)]
    public class WorleyNoise : NoiseType
    {
        [Header("Noise General Settings")]
        [SerializeField] private WorleyNoiseDimension dimension = WorleyNoiseDimension.MONODIMENSIONAL;

        [Header("Noise Value Settings")]
        [Range(1, 100)][SerializeField] private int numberOfGridPoints = 10;
        [SerializeField] private float inversionValue = 2.0f;
        [SerializeField] private bool shouldInvertNoise = true;

        private List<Vector2> gridPoints;

        public WorleyNoiseDimension Dimension { get => dimension; }

        public List<Vector2> GridPoints { get => gridPoints; }
        public bool ShouldInvertNoise { get => shouldInvertNoise; }
        public float InversionValue { get => inversionValue; }

        public void Initialize(int textureResolution)
        {
            gridPoints = new List<Vector2>();

            // TODO: The 3D version requires 3D textures, more advance, we'll see a cool implementation based on that.
            for (int i = 0; i < numberOfGridPoints; i++)
                if (dimension is WorleyNoiseDimension.MONODIMENSIONAL)
                    gridPoints.Add(new Vector2(Random.Range(0, textureResolution), Mathf.FloorToInt(textureResolution / 2)));
                else
                    gridPoints.Add(new Vector2(Random.Range(0, textureResolution), Random.Range(0, textureResolution)));
        }
    }
}
