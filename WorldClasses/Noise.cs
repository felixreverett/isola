using OpenTK.Mathematics;
using LibNoise.Primitive;

namespace FeloxGame.WorldClasses
{
    public class Noise
    {
        public static float[] GenerateNoiseMap(int mapWidth,  int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            SimplexPerlin perlin = new SimplexPerlin();

            // 1D is more optimised than 2D they say
            float[] noiseMap = new float[mapWidth * mapHeight];
            var random = new Random(seed);

            // must have more than 0 octaves of noise
            if (octaves < 1) {  octaves = 1; }

            Vector2[] octaveOffsets = new Vector2[octaves];
            
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000) + offset.X;
                float offsetY = random.Next(-100000, 100000) + offset.Y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            // set a minimum scale
            if (scale <= 0f) { scale = 0.0001f; }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // centre the zoom
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            // main loop?
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    // Define base values for amplitude, frequency, and noiseHeight:
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    // Calculate noise for each octave
                    for (int i = 0; i < octaves; i++)
                    {
                        // Sample a point (x, y)
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].X;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].Y;

                        // We use Perlin noise (I don't know why I'm doing it like this???
                        float perlinValue = perlin.GetValue(sampleX, sampleY) * 2 - 1;

                        // noiseHeight is our final noise, adding all octaves together
                        noiseHeight += perlinValue * amplitude;
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    // We need to find the min and max noise height in our noisemap so that
                    // we can later interpolate the min and max values between 0 and 1 again
                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[y * mapWidth + x] = noiseHeight;
                }
            }

            // Make sure noise falls between values 0f and 1f with InverseLerp:
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    noiseMap[y * mapWidth + x] = InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[y * mapWidth + x]);
                }
            }

            return noiseMap;
        }

        // ChatGPT wrote this (I'm experimenting with noise, not inverselErPs
        public static float InverseLerp(float start, float end, float value)
        {
            if (start < end)
            {
                if (value <= start)
                    return 0f;
                else if (value >= end)
                    return 1f;
            }
            else
            {
                if (value >= start)
                    return 0f;
                else if (value <= end)
                    return 1f;
            }

            return (value - start) / (end - start);
        }
    }
}
