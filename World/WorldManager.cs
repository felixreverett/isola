using System.Reflection.Metadata.Ecma335;

namespace FeloxGame.World
{
    public class WorldManager
    {
        private static WorldManager _instance = null;
        private static readonly object _loc = new();

        public static WorldManager Instance
        {
            get
            {
                lock (_loc)
                {
                    if (_instance == null)
                    {
                        _instance = new WorldManager();
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

        public float[] GetSubTextureCoordinates(int textureIndex)
        {
            int superTexSize = 1024;
            float subTexSize = 32f / superTexSize;

            int col = textureIndex % 32; // 32 = WorldTextures.Width (1024) / Texture.Width (32) (make dynamic?)
            float texCoordXMin = col * subTexSize; // normalise it

            int row = textureIndex / 32;
            float texCoordYMin = 1.0f - ((row + 1) * subTexSize ); // normalise, offset, and "flip" it

            float texCoordXMax = texCoordXMin + subTexSize;
            float texCoordYMax = texCoordYMin + subTexSize;

            float[] texCoords = { texCoordXMin, texCoordYMin, texCoordXMax, texCoordYMax };
            return texCoords;
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
