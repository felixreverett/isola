using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;

namespace Isola.World
{
    public static class NoiseGenerator
    {
        public static NoiseMap GenerateNoiseMap(int chunkX, int chunkY, int chunkSize, int seed, float scale, int octaveCount)
        {
            Perlin noiseSource = new Perlin()
            {
                Seed = seed,                //3
                Frequency = 1,
                Persistence = 0.5,          //0.5
                Lacunarity = 3,
                OctaveCount = octaveCount,  //9
                Quality = NoiseQuality.Standard
            };

            NoiseMap noiseMap = new NoiseMap();
            PlaneNoiseMapBuilder noiseMapBuilder = new PlaneNoiseMapBuilder
            {
                DestNoiseMap = noiseMap,
                SourceModule = noiseSource
            };

            noiseMapBuilder.SetDestSize(chunkSize, chunkSize);

            noiseMapBuilder.SetBounds(
                (chunkSize * chunkX) / scale,
                (chunkSize * (chunkX + 1) -1) / scale,
                (chunkSize * chunkY) / scale,
                (chunkSize * (chunkY + 1) -1) / scale
                );

            noiseMapBuilder.Build();

            return noiseMap;
        }
    }
}
