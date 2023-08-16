using FeloxGame.GUI;
using FeloxGame.WorldClasses;
using OpenTK.Mathematics;
using SharpNoise;

namespace FeloxGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game1 game = new Game1(1280, 720, "FeloxGame");
            game.Run();
        }
    }
}