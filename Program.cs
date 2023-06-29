using FeloxGame.Core;
using System;

namespace FeloxGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game1 game = new Game1(800, 600, "Does this work");
            game.Run();
        }
    }
}