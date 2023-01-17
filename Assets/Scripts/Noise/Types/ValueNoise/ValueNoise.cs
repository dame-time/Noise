using System.Collections;
using System.Collections.Generic;
using Noise.Generators.TypeInfos;
using UnityEngine;

namespace Noise.Generators
{
    [CreateAssetMenu(fileName = "Value Noise Settings", menuName = "Noise Generators/Value Noise", order = 2)]
    public class ValueNoise : NoiseType
    {
        [Header("Noise General Settings")]
        [Tooltip("Set this parameter in order to zoom out or in the World Coordinates of your model")]
        [Range(0, 10000)] [SerializeField] private float frequency;
        [SerializeField] private ValueNoiseDimension dimension = ValueNoiseDimension.MONODIMENSIONAL;

        [Header("Noise Value Settings")]
        [Range(0, 10)] [SerializeField] private int numberOfShades = 3;
        [SerializeField] private bool shuffleNoise;

        private List<int> hashList;

        public float Frequency { get => frequency; }
        public ValueNoiseDimension Dimension { get => dimension; }
        public int HashLength { get => numberOfShades; }
        public bool ShuffleNoise { get => shuffleNoise; }

        public List<int> HashList { get => hashList; }

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
