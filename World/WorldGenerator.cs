using System.Reflection.Metadata.Ecma335;

namespace FeloxGame.World
{
    public class WorldGenerator
    {
        private static WorldGenerator _instance = null;
        private static readonly object _loc = new();

        public static WorldGenerator Instance
        {
            get
            {
                lock (_loc)
                {
                    if (_instance == null)
                    {
                        _instance = new WorldGenerator();
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Loads chunk if found in directory, otherwise generates new chunk
        /// </summary>
        /// <returns></returns>
        public Chunk LoadOrGenerateChunk(string filePath, int chunkPosX, int chunkPosY)
        {
            if (File.Exists(filePath))
            {
                return LoadChunk(filePath, chunkPosX, chunkPosY);
            }
            else
            {
                return GenerateChunk(chunkPosX, chunkPosY); //TODO: add chunk coords
            }
        }

        public Chunk LoadChunk(string filePath, int chunkPosX, int chunkPosY)
        {
            string[] rows = File.ReadAllText(filePath).Trim().Replace("\r", "").Split("\n").ToArray();
            Chunk newChunk = new(chunkPosX, chunkPosY);

            for (int y = 0; y < rows.Length; y++)
            {
                string row = rows[y];
                string[] cols = row.Split(" ");
                for (int x = 0; x < cols.Length; x++)
                {
                    newChunk.Tiles[x, y] = cols[x];
                }
            }

            return newChunk;
        }

        /*
        public Chunk LoadChunk(string filePath, int chunkPosX, int chunkPosY) //will replace with source later (world loaded into memory?)
        {
            string[] rows = File.ReadAllText(filePath).Trim().Replace("\r", "").Split("\n").ToArray();
            Chunk newChunk = new(chunkPosX, chunkPosY);

            for (int y = 0; y < rows.Length; y++)
            {
                string row = rows[y];
                string[] cols = row.Split(" ");
                for (int x = 0; x < cols.Length; x++)
                {
                    newChunk.Tiles[x, y] = new Tile(cols[x]);
                }
            }

            return newChunk;
        }
        */

        /// <summary>
        /// Generates a new chunk 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public Chunk GenerateChunk(int chunkPosX, int chunkPosY, int seed = 0)
        {
            return LoadChunk("Resources/World/worldTest.txt", chunkPosX, chunkPosY); //currently just loads same chonk
        }
    }
}
