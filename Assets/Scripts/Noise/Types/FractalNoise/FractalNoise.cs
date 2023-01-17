using System.Collections;
using System.Collections.Generic;
using Noise.Generators.TypeInfos;
using UnityEngine;

namespace Noise.Generators
{
    [CreateAssetMenu(fileName = "Fractal Noise Settings", menuName = "Noise Generators/Fractal Noise", order = 4)]
    public class FractalNoise : NoiseType
    {
        [Header("Noise General Settings")]
        [Tooltip("Set this parameter in order to zoom out or in the World Coordinates of your model")]
        [Range(0, 10000)] [SerializeField] private float frequency;
        [SerializeField] private FractalNoiseType noiseType;

        [Header("Fractal Value Settings")]
        [Tooltip("The layer of noise that we want to \"weighted\" overlap in order to correctly sample the value of a single pixel")]
        [Range(0, 14)] [SerializeField] private int octaves = 8;
        [Tooltip("In fractal noise we generate noise X number of times, determined by octaves, each time we generate noise" +
            "we increase the frequency at which we sample, the lacunarity set the increase ration of the frequency per each new octave sampled")]
        [SerializeField] private float lacunarity = 2.0f;
        [Tooltip("In fractal noise each octave will contribute at the final result in a fractionary part, example with persistanc 0.5f: " +
            "first octave sample count 1, second octave sample count 0.5f, third octave sample 0.25f, ... " +
            "at the end we will do a weighted average of all the values obtained at each octave sample")]
        [SerializeField] private float persistance = 0.5f;

        public float Frequency { get => frequency; }
        public FractalNoiseType NoiseType { get => noiseType; }

        public int Octaves { get => octaves; }
        public float Lacunarity { get => lacunarity; }
        public float Persistance { get => persistance; }
    }
}