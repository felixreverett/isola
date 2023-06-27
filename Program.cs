using FeloxGame.Core;
using System;

namespace FeloxGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new MyGame("Test", 800, 600);
            game.Run();
        }
    }
}