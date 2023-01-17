using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noise.Generators
{
    [CreateAssetMenu(fileName = "Random Noise Settings", menuName = "Noise Generators/Random Noise", order = 0)]
    public class RandomNoise : ScriptableObject
    {
        [SerializeField] private int seed = 1;

        public int Seed { get => seed; }
    }
}
