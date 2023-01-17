using System.Collections;
using System.Collections.Generic;
using Noise.Generators.TypeInfos;
using UnityEngine;

namespace Noise.Generators
{
    [CreateAssetMenu(fileName = "Perlin Noise Settings", menuName = "Noise Generators/Perlin Noise", order = 3)]
    public class PerlinNoise : NoiseType
    {
        [Header("Noise General Settings")]
        [Tooltip("Set this parameter in order to zoom out or in the World Coordinates of your model")]
        [Range(0, 10000)] [SerializeField] private float frequency;
        [SerializeField] private PerlinNoiseDimension dimension = PerlinNoiseDimension.MONODIMENSIONAL;

        [Header("Perlin Value Settings")]
        [Range(0, 10)] [SerializeField] private int numberOfShades = 3;
        [SerializeField] private bool normalizedValues = true;
        [SerializeField] private bool shuffleNoise;

        private static readonly List<float> gradients1D = new List<float> { -1.0f, 1.0f };

        private static readonly List<Vector2> gradients2D = new List<Vector2>
        {
            new Vector2( 1f, 0f),
            new Vector2(-1f, 0f),
            new Vector2( 0f, 1f),
            new Vector2( 0f,-1f),
            new Vector2( 1f, 1f).normalized,
            new Vector2(-1f, 1f).normalized,
            new Vector2( 1f,-1f).normalized,
            new Vector2(-1f,-1f).normalized
        };

        private static readonly List<Vector3> gradients3D = new List<Vector3>
        {
            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 1f,-1f, 0f),
            new Vector3(-1f,-1f, 0f),
            new Vector3( 1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f),
            new Vector3( 1f, 0f,-1f),
            new Vector3(-1f, 0f,-1f),
            new Vector3( 0f, 1f, 1f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f, 1f,-1f),
            new Vector3( 0f,-1f,-1f),

            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f,-1f,-1f)
        };

        private List<int> hashList;

        public float Frequency { get => frequency; }
        public PerlinNoiseDimension Dimension { get => dimension; }
        public int HashLength { get => numberOfShades; }
        public bool NormalizedValues { get => normalizedValues; }
        public bool ShuffleNoise { get => shuffleNoise; }

        public List<int> HashList { get => hashList; }

        public static List<float> Gradients1D => gradients1D;
        public static List<Vector2> Gradients2D => gradients2D;
        public static List<Vector3> Gradients3D => gradients3D;

        public override void Initialize()
        {
            hashList = new List<int>();
            int hashLengthNormalized = Mathf.RoundToInt(Mathf.Pow(2, HashLength));

            for (int i = 0; i < hashLengthNormalized; i++)
                hashList.Add(i);

            if (ShuffleNoise)
                hashList = ShuffleList(hashList);
        }

        private static List<int> ShuffleList(List<int> hashList)
        {
            for (int i = 0; i < hashList.Count; i++)
            {
                int temp = hashList[i];
                int randomIndex = Random.Range(i, hashList.Count);
                hashList[i] = hashList[randomIndex];
                hashList[randomIndex] = temp;
            }

            return hashList;
        }
    }
}
