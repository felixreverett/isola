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
            //Game1 game = new Game1(1280, 720, "FeloxGame");
            //game.Run();

            int _rows = 5; int _cols = 10;

            for (int i = 0; i < _rows * _cols; i++)
            {
                int currentRow = i / _cols;
                int currentCol = i % _cols;
                Console.WriteLine($"Row: {currentRow}, Column: {currentCol}");
            }
        }
    }
}