using FeloxGame.WorldClasses;
using SharpNoise;

namespace FeloxGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game1 game = new Game1(1280, 720, "FeloxGame");
            game.Run();

            /*
            float[] noiseMap = WorldClasses.Noise.GenerateNoiseMap(0, 0, 16, 16, 1, 1, 1, 1, 1, new OpenTK.Mathematics.Vector2(3, 3));
            int i = 0;
            foreach (float noise in noiseMap)
            {
                i += 1;
                Console.Write(noise);
                if (i == 16)
                {
                    i = 0;
                    Console.WriteLine(' ');
                }
            }
            */

            NoiseMap noiseMap2 = WorldClasses.NoiseGenerator.GenerateNoiseMap(0, 0, 16, 3, 100f, 9);
            int i = 0;      
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    i++;
                    Console.Write($"{noiseMap2.GetValue(x, y)}   ");
                    if (i == 16) { Console.WriteLine(' '); i = 0; }
                }
            }


        }
    }
}