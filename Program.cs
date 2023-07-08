using FeloxGame.Core;
using FeloxGame.World;
using System;

namespace FeloxGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game1 game = new Game1(800, 600, "Does this work");
            game.Run();
            
            /*float[] texCoords = WorldManager.Instance.GetSubTextureCoordinates(0);
            foreach (float texCoord in texCoords)
            {
                Console.WriteLine(texCoord);
            }

            float[] texCoords2 = WorldManager.Instance.GetSubTextureCoordinates(1);
            foreach (float texCoord in texCoords2)
            {
                Console.WriteLine(texCoord);
            }*/

        }
    }
}