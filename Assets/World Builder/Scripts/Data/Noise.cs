using UnityEngine;

namespace Cinderwild.WorldBuilder.Data
{
    public static class Noise
    {
        public static float GetNoise(float x, float y, float noiseScale, float offset)
        {
            return GetNoise(x, y, noiseScale, new Vector2(offset, offset));
        }
        public static float GetNoise(float x, float y, float noiseScale, Vector2 offset)
        {
            float height = GenerateNoise(x * noiseScale + offset.x , y * noiseScale + offset.y);

            height = Mathf.Clamp01(height);
            height = Mathf.Pow(height, 1.2f);

            return height;
        }

        private static float GenerateNoise(float x, float y)
        {
            float value = 0f;
            float amplitude = 1f;
            float frequency = 1f;
            float maxValue = 0f;

            for (int i = 0; i < 4; i++)
            {
                float sampleX = x * frequency;
                float sampleY = y * frequency;

                value += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;
                maxValue += amplitude;

                amplitude *= 0.5f;
                frequency *= 2f;
            }

            return value / maxValue;
        }
    }
}
