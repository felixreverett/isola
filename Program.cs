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

            //UI uI = new UI(1920f, 1080f, eAnchor.Middle, 1.0f);
            //Vector2 myVec = uI.GetRelativeDimensions(1920f, 1080f);
            //Console.WriteLine($"X: {myVec.X}. Y: {myVec.Y}");
        }
    }
}