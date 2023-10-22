using FeloxGame.GUI;
using FeloxGame.WorldClasses;
using FeloxGame.Core;
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


            //string myfilepath = @"D:\Programming\C#\FeloxGame\Saves\SampleWorldStructure\ChunkData";
            //Chunk newChunk = World.GenerateChunk
            //Loading.SaveObject<Chunk>(newChunk, myfilepath);
        }
    }
}