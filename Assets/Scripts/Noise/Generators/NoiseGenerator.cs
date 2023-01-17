using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Noise.Generators
{
    public sealed class NoiseGenerator
    {
        public delegate float NoiseMethod(Vector3 point, float frequency);

        private static readonly float squareRootOf2 = Mathf.Sqrt(2);

        private static LatticeNoise latticeNoiseSettings;
        private static ValueNoise valueNoiseSettings;
        private static PerlinNoise perlinNoiseSettings;
        private static FractalNoise fractalNoiseSettings;

        public static void Initialize(LatticeNoise latticeNoise, ValueNoise valueNoise, PerlinNoise perlinNoise, FractalNoise fractalNoise)
        {
            latticeNoiseSettings = latticeNoise;
            valueNoiseSettings = valueNoise;
            perlinNoiseSettings = perlinNoise;
            fractalNoiseSettings = fractalNoise;
        }

        public static float LatticeNoiseValue(Vector3 point, float frequency)
        {
            point *= frequency;

            int xShade = Mathf.FloorToInt(point.x);
            int yShade = Mathf.FloorToInt(point.y);
            int zShade = Mathf.FloorToInt(point.z);

            int mask = (latticeNoiseSettings.HashList.Count - 1);

            xShade &= mask;
            yShade &= mask;
            zShade &= mask;

            // We can avoid remask everything in noise2D and noise 3D inside the list get
            // but this would requeire us to duplicate/triplicate the hashList lenght values
            // this would slow our performance for better clarity -> though we rather have performances here
            var noise1D = latticeNoiseSettings.HashList[xShade] / (float) mask;

            var noise2D = latticeNoiseSettings.HashList[
                            (latticeNoiseSettings.HashList[xShade] + yShade) & mask
                            ] / (float)mask;

            var noise3D = latticeNoiseSettings.HashList[(latticeNoiseSettings.HashList[
                            (latticeNoiseSettings.HashList[xShade] + yShade) & mask
                            ] + zShade) & mask] / (float) mask;

            return latticeNoiseSettings.Dimension == TypeInfos.LatticeNoiseDimension.MONODIMENSIONAL ? noise1D :
                    latticeNoiseSettings.Dimension == TypeInfos.LatticeNoiseDimension.BIDIMENSIONAL ? noise2D :
                    noise3D;
        }

        public static float ValueNoiseValue(Vector3 point, float frequency)
        {
            point *= frequency;

            int xShade = Mathf.FloorToInt(point.x);
            int yShade = Mathf.FloorToInt(point.y);
            int zShade = Mathf.FloorToInt(point.z);

            float distanceBetweenSamplePointHashPointX = point.x - xShade;
            float distanceBetweenSamplePointHashPointY = point.y - yShade;
            float distanceBetweenSamplePointHashPointZ = point.z - zShade;

            int mask = (valueNoiseSettings.HashList.Count - 1);

            xShade &= mask;
            yShade &= mask;
            zShade &= mask;

            int nextXShade = (xShade + 1) % valueNoiseSettings.HashList.Count;
            int nextYShade = (yShade + 1) % valueNoiseSettings.HashList.Count;
            int nextZShade = (zShade + 1) % valueNoiseSettings.HashList.Count;

            int currentXHashValue = valueNoiseSettings.HashList[xShade];
            int nextXHashValue = valueNoiseSettings.HashList[nextXShade];

            int currentYHashValue = valueNoiseSettings.HashList[(currentXHashValue + yShade) & mask];
            int nextXCurrentYHashValue = valueNoiseSettings.HashList[(nextXHashValue + yShade) & mask];
            int currentXnextYHashValue = valueNoiseSettings.HashList[(currentXHashValue + nextYShade) & mask];
            int nextYHashValue = valueNoiseSettings.HashList[(nextXHashValue + nextYShade) & mask];

            int h000 = valueNoiseSettings.HashList[(currentYHashValue + zShade) & mask];
            int h100 = valueNoiseSettings.HashList[(nextXCurrentYHashValue + zShade) & mask];
            int h010 = valueNoiseSettings.HashList[(currentXnextYHashValue + zShade) & mask];
            int h110 = valueNoiseSettings.HashList[(nextYHashValue + zShade) & mask];
            int h001 = valueNoiseSettings.HashList[(currentYHashValue + nextZShade) & mask];
            int h101 = valueNoiseSettings.HashList[(nextXCurrentYHashValue + nextZShade) & mask];
            int h011 = valueNoiseSettings.HashList[(currentXnextYHashValue + nextZShade) & mask];
            int h111 = valueNoiseSettings.HashList[(nextYHashValue + nextZShade) & mask];

            distanceBetweenSamplePointHashPointX = SmoothDistanceBetweenSamplePointHashPoint(distanceBetweenSamplePointHashPointX);
            distanceBetweenSamplePointHashPointY = SmoothDistanceBetweenSamplePointHashPoint(distanceBetweenSamplePointHashPointY);
            distanceBetweenSamplePointHashPointZ = SmoothDistanceBetweenSamplePointHashPoint(distanceBetweenSamplePointHashPointZ);

            // We can avoid remask everything in noise2D and noise 3D inside the list get
            // but this would requeire us to duplicate/triplicate the hashList lenght values
            // this would slow our performance for better clarity -> though we rather have performances here
            var noise1D = Mathf.Lerp(currentXHashValue, nextXHashValue, distanceBetweenSamplePointHashPointX) * (1f / mask);

            var noise2D = Mathf.Lerp(
                Mathf.Lerp(currentYHashValue, nextXCurrentYHashValue, distanceBetweenSamplePointHashPointX),
                Mathf.Lerp(currentXnextYHashValue, nextYHashValue, distanceBetweenSamplePointHashPointX),
                distanceBetweenSamplePointHashPointY
                ) * (1f / mask);

            var noise3D = Mathf.Lerp(
                Mathf.Lerp(Mathf.Lerp(h000, h100, distanceBetweenSamplePointHashPointX), Mathf.Lerp(h010, h110, distanceBetweenSamplePointHashPointX), distanceBetweenSamplePointHashPointY),
                Mathf.Lerp(Mathf.Lerp(h001, h101, distanceBetweenSamplePointHashPointX), Mathf.Lerp(h011, h111, distanceBetweenSamplePointHashPointX), distanceBetweenSamplePointHashPointY),
                distanceBetweenSamplePointHashPointZ
            ) * (1f / mask);

            return valueNoiseSettings.Dimension == TypeInfos.ValueNoiseDimension.MONODIMENSIONAL ? noise1D :
                    valueNoiseSettings.Dimension == TypeInfos.ValueNoiseDimension.BIDIMENSIONAL ? noise2D :
                    noise3D;
        }

        public static float PerlinNoiseValue(Vector3 point, float frequency)
        {
            point *= frequency;

            int xShade = Mathf.FloorToInt(point.x);
            int yShade = Mathf.FloorToInt(point.y);
            int zShade = Mathf.FloorToInt(point.z);

            float rightDistanceBetweenSamplePointHashPointX = point.x - xShade;
            float rightDistanceBetweenSamplePointHashPointY = point.y - yShade;
            float rightDistanceBetweenSamplePointHashPointZ = point.z - zShade;

            // This values might be negatives so we may need a normalization in the range [0, 1]
            float leftDistanceBetweenSamplePointHashPointX = rightDistanceBetweenSamplePointHashPointX - 1.0f;
            float leftDistanceBetweenSamplePointHashPointY = rightDistanceBetweenSamplePointHashPointY - 1.0f;
            float leftDistanceBetweenSamplePointHashPointZ = rightDistanceBetweenSamplePointHashPointZ - 1.0f;

            int hashMask = (perlinNoiseSettings.HashList.Count - 1);
            int gradientMask1D = (PerlinNoise.Gradients1D.Count - 1);
            int gradientMask2D = (PerlinNoise.Gradients2D.Count - 1);
            int gradientMask3D = (PerlinNoise.Gradients3D.Count - 1);

            xShade &= hashMask;
            yShade &= hashMask;
            zShade &= hashMask;

            int nextXShade = (xShade + 1) % perlinNoiseSettings.HashList.Count;
            int nextYShade = (yShade + 1) % perlinNoiseSettings.HashList.Count;
            int nextZShade = (zShade + 1) % perlinNoiseSettings.HashList.Count;

            int currentXHashValue = perlinNoiseSettings.HashList[xShade];
            int nextXHashValue = perlinNoiseSettings.HashList[nextXShade];

            int currentYHashValue = perlinNoiseSettings.HashList[(currentXHashValue + yShade) & hashMask];
            int nextXCurrentYHashValue = perlinNoiseSettings.HashList[(nextXHashValue + yShade) & hashMask];
            int currentXnextYHashValue = perlinNoiseSettings.HashList[(currentXHashValue + nextYShade) & hashMask];
            int nextYHashValue = perlinNoiseSettings.HashList[(nextXHashValue + nextYShade) & hashMask];

            int h000 = perlinNoiseSettings.HashList[(currentYHashValue + zShade) & hashMask];
            int h100 = perlinNoiseSettings.HashList[(nextXCurrentYHashValue + zShade) & hashMask];
            int h010 = perlinNoiseSettings.HashList[(currentXnextYHashValue + zShade) & hashMask];
            int h110 = perlinNoiseSettings.HashList[(nextYHashValue + zShade) & hashMask];
            int h001 = perlinNoiseSettings.HashList[(currentYHashValue + nextZShade) & hashMask];
            int h101 = perlinNoiseSettings.HashList[(nextXCurrentYHashValue + nextZShade) & hashMask];
            int h011 = perlinNoiseSettings.HashList[(currentXnextYHashValue + nextZShade) & hashMask];
            int h111 = perlinNoiseSettings.HashList[(nextYHashValue + nextZShade) & hashMask];

            float gradient0XDirection = PerlinNoise.Gradients1D[currentXHashValue & gradientMask1D];
            float gradient1XDirection = PerlinNoise.Gradients1D[nextXHashValue & gradientMask1D];

            // Using a 1D function g(x) = x;
            float gradientX0Value = gradient0XDirection * rightDistanceBetweenSamplePointHashPointX;
            float gradientX1Value = gradient1XDirection * leftDistanceBetweenSamplePointHashPointX;

            Vector2 gradient00XYDirection = PerlinNoise.Gradients2D[currentYHashValue & gradientMask2D];
            Vector2 gradient10XYDirection = PerlinNoise.Gradients2D[nextXCurrentYHashValue & gradientMask2D];
            Vector2 gradient01XYDirection = PerlinNoise.Gradients2D[currentXnextYHashValue & gradientMask2D];
            Vector2 gradient11XYDirection = PerlinNoise.Gradients2D[nextYHashValue & gradientMask2D];

            float gradient00XYValue = Gradient2D(gradient00XYDirection, rightDistanceBetweenSamplePointHashPointX, rightDistanceBetweenSamplePointHashPointY);
            float gradient10XYValue = Gradient2D(gradient10XYDirection, leftDistanceBetweenSamplePointHashPointX, rightDistanceBetweenSamplePointHashPointY);
            float gradient01XYValue = Gradient2D(gradient01XYDirection, rightDistanceBetweenSamplePointHashPointX, leftDistanceBetweenSamplePointHashPointY);
            float gradient11XYValue = Gradient2D(gradient11XYDirection, leftDistanceBetweenSamplePointHashPointX, leftDistanceBetweenSamplePointHashPointY);

            Vector3 g000 = PerlinNoise.Gradients3D[h000 & gradientMask3D];
            Vector3 g100 = PerlinNoise.Gradients3D[h100 & gradientMask3D];
            Vector3 g010 = PerlinNoise.Gradients3D[h010 & gradientMask3D];
            Vector3 g110 = PerlinNoise.Gradients3D[h110 & gradientMask3D];
            Vector3 g001 = PerlinNoise.Gradients3D[h001 & gradientMask3D];
            Vector3 g101 = PerlinNoise.Gradients3D[h101 & gradientMask3D];
            Vector3 g011 = PerlinNoise.Gradients3D[h011 & gradientMask3D];
            Vector3 g111 = PerlinNoise.Gradients3D[h111 & gradientMask3D];

            float v000 = Gradient3D(g000, rightDistanceBetweenSamplePointHashPointX, rightDistanceBetweenSamplePointHashPointY, rightDistanceBetweenSamplePointHashPointZ);
            float v100 = Gradient3D(g100, leftDistanceBetweenSamplePointHashPointX, rightDistanceBetweenSamplePointHashPointY, rightDistanceBetweenSamplePointHashPointZ);
            float v010 = Gradient3D(g010, rightDistanceBetweenSamplePointHashPointX, leftDistanceBetweenSamplePointHashPointY, rightDistanceBetweenSamplePointHashPointZ);
            float v110 = Gradient3D(g110, leftDistanceBetweenSamplePointHashPointX, leftDistanceBetweenSamplePointHashPointY, rightDistanceBetweenSamplePointHashPointZ);
            float v001 = Gradient3D(g001, rightDistanceBetweenSamplePointHashPointX, rightDistanceBetweenSamplePointHashPointY, leftDistanceBetweenSamplePointHashPointZ);
            float v101 = Gradient3D(g101, leftDistanceBetweenSamplePointHashPointX, rightDistanceBetweenSamplePointHashPointY, leftDistanceBetweenSamplePointHashPointZ);
            float v011 = Gradient3D(g011, rightDistanceBetweenSamplePointHashPointX, leftDistanceBetweenSamplePointHashPointY, leftDistanceBetweenSamplePointHashPointZ);
            float v111 = Gradient3D(g111, leftDistanceBetweenSamplePointHashPointX, leftDistanceBetweenSamplePointHashPointY, leftDistanceBetweenSamplePointHashPointZ);

            float tx = SmoothDistanceBetweenSamplePointHashPoint(rightDistanceBetweenSamplePointHashPointX);
            float ty = SmoothDistanceBetweenSamplePointHashPoint(rightDistanceBetweenSamplePointHashPointY);
            float tz = SmoothDistanceBetweenSamplePointHashPoint(rightDistanceBetweenSamplePointHashPointZ);

            // If two gradients point in opposite directions (that should be the maximum value) we would obtain 0.5f, this is not correct,
            // infact we want the value to be at his maximum if the two gradients are opposite, so we multiply by two and the range goes from
            // [-0.5f, 0.5f].
            var sample1D = Mathf.Lerp(gradientX0Value, gradientX1Value, tx) * 2.0f;
            var normalizedSample1D = sample1D * 0.5f + 0.5f; // clamping my values between [0, 1]

            // The maximum value in this case is determined by all 4 gradients pointing in all the 4 opposite directions
            // this means that the maximum value won't be 0.5f but 0.70710678118f or better 1 / sqrt(2) so in order to have
            // the maximum value to be 1 we simply multiply by the square root of 2
            var sample2D = Mathf.Lerp(
                                Mathf.Lerp(gradient00XYValue, gradient10XYValue, tx),
                                Mathf.Lerp(gradient01XYValue, gradient11XYValue, tx),
                                ty
                           ) * squareRootOf2;
            var normalizedSample2D = sample2D * 0.5f + 0.5f;

            var sample3D = Mathf.Lerp(
                                Mathf.Lerp(Mathf.Lerp(v000, v100, tx), Mathf.Lerp(v010, v110, tx), ty),
                                Mathf.Lerp(Mathf.Lerp(v001, v101, tx), Mathf.Lerp(v011, v111, tx), ty),
                                tz
                           );
            var normalizedSample3D = sample3D * 0.5f + 0.5f;

            var noise1D = perlinNoiseSettings.NormalizedValues ? normalizedSample1D : sample1D;
            var noise2D = perlinNoiseSettings.NormalizedValues ? normalizedSample2D : sample2D;
            var noise3D = perlinNoiseSettings.NormalizedValues ? normalizedSample3D : sample3D;

            return perlinNoiseSettings.Dimension == TypeInfos.PerlinNoiseDimension.MONODIMENSIONAL ? noise1D :
                    perlinNoiseSettings.Dimension == TypeInfos.PerlinNoiseDimension.BIDIMENSIONAL ? noise2D :
                    noise3D;
        }

        public static float FractalNoiseValue(NoiseMethod method, Vector3 point, float frequency)
        {
            float sum = method(point, frequency);
            float amplitude = 1.0f;
            float range = 1.0f;

            for (int octave = 1; octave < fractalNoiseSettings.Octaves; octave++)
            {
                frequency *= fractalNoiseSettings.Lacunarity;
                amplitude *= fractalNoiseSettings.Persistance;
                range += amplitude;
                sum += method(point, frequency) * amplitude;
            }

            return sum / range;
        }

        public static float WorleyNoiseValue(Vector2 point, int textureResolution, WorleyNoise worleySettings)
        {
            float minDistance = float.PositiveInfinity;

            for (int i = 0; i < worleySettings.GridPoints.Count; i++)
                minDistance = (worleySettings.GridPoints[i] - point).magnitude < minDistance ?
                                (worleySettings.GridPoints[i] - point).magnitude : minDistance;

            return minDistance / textureResolution;
        }

        /// <summary>
        /// Smooth function that basically pad our t value with a mathematical funciton.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static float SmoothDistanceBetweenSamplePointHashPoint(float t) => t * t * t * (t * (t * 6f - 15f) + 10f);

        /// <summary>
        /// Computes the gradient vector, starting from vector <paramref name="g"/> with components <paramref name="x"/> and <paramref name="y"/>.
        /// The formula used to compute the gradient is: g(x, y) = ax + by.
        /// Substantially we are dealing with a dot product between <paramref name="g"/> and a vector composed by (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="g">The initial vector, this might be a versor (unitary vector).</param>
        /// <param name="x">The first parameter to our gradient funciton.</param>
        /// <param name="y">The second parameter to our gradient function.</param>
        /// <returns></returns>
        private static float Gradient2D(Vector2 g, float x, float y) => g.x * x + g.y * y;

        /// <summary>
        /// Computes the gradient vector, starting from vector <paramref name="g"/> with components <paramref name="x"/>, <paramref name="y"/> adn <paramref name="z"/>.
        /// The formula used to compute the gradient is: g(x, y) = ax + by.
        /// Substantially we are dealing with a dot product between <paramref name="g"/> and a vector composed by (<paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>).
        /// </summary>
        /// <param name="g">The initial vector, this might be a versor (unitary vector).</param>
        /// <param name="x">The first parameter to our gradient funciton.</param>
        /// <param name="y">The second parameter to our gradient function.</param>
        /// <param name="z">The third parameter to our gradient function.</param>
        /// <returns></returns>
        private static float Gradient3D(Vector3 g, float x, float y, float z) => g.x * x + g.y * y + g.z * z;
    }
}
