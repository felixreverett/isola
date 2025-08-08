using OpenTK.Mathematics;
using LibNoise.Primitive;
using LibNoise;

namespace Isola.World
{
    public class Noise
    {
        private SimplexPerlin perlin;
        private Vector2[] octaveOffsets;
        private Random random;
        
        public Noise(int seed, int octaves)
        {
            perlin = new SimplexPerlin(seed, LibNoise.NoiseQuality.Best);
            octaveOffsets = new Vector2[octaves];
            random = new Random(seed);

            for (int i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000);
                float offsetY = random.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
        }

        public float[,] GenerateNoiseMap(int chunkX, int chunkY, int chunkWidth, int chunkHeight, int seed, float scale, int octaves, float persistance, float lacunarity)
        {
            float[,] noiseMap = new float[chunkWidth,chunkHeight];

            // must have more than 0 octaves of noise
            if (octaves < 1) {  octaves = 1; }

            if (octaveOffsets.Length < octaves)
            {
                throw new ArgumentException("Insufficient number of octave offsets provided.");
            }

            // set a minimum scale
            if (scale <= 0f) { scale = 0.0001f; }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // centre the zoom
            float halfWidth = chunkWidth / 2f;
            float halfHeight = chunkHeight / 2f;

            // Calculate the octave offsets based on the chunk's coordinates and seed
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = (chunkX * chunkWidth + halfWidth) / scale * octaveOffsets[i].X;
                float offsetY = (chunkY * chunkHeight + halfHeight) / scale * octaveOffsets[i].Y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            // main loop?
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    int absX = chunkWidth * chunkX + x;
                    int absY = chunkHeight * chunkY + y;

                    // Define base values for amplitude, frequency, and noiseHeight:
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    // Calculate noise for each octave
                    for (int i = 0; i < octaves; i++)
                    {
                        // Sample a point (x, y)
                        float sampleX = (absX - halfWidth) / scale * frequency + octaveOffsets[i].X;
                        float sampleY = (absY - halfHeight) / scale * frequency + octaveOffsets[i].Y;

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

                    noiseMap[x,y] = noiseHeight;
                }
            }

            // Make sure noise falls between values 0f and 1f with InverseLerp:
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    noiseMap[x, y] = InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }

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
