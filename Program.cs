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
        }
    }
}