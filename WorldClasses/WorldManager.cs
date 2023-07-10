using FeloxGame.Core.Management;

namespace FeloxGame.WorldClasses
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

        public TexCoords GetSubTextureCoordinates(int textureIndex)
        {
            TexCoords texCoords = new TexCoords();
            int superTexSize = 1024; // texture atlas dimensions
            int padding = 1; // padding between textures
            float normalPadding = (float)padding / superTexSize;
            int rowColLength = 1024 / (32 + padding); // how many textures per row/col
            float subTexSize = 32f / superTexSize;
            float pixelOffset = 0.00001f;

            int col = textureIndex % rowColLength;
            texCoords.MinX = col * (subTexSize + normalPadding) + pixelOffset; // normalise it

            int row = textureIndex / rowColLength;
            texCoords.MinY = 1.0f - ((row + 1) * (subTexSize + normalPadding)) + pixelOffset; // normalise, offset, and "flip" it

            texCoords.MaxX = texCoords.MinX + subTexSize - (2 * pixelOffset);
            texCoords.MaxY = texCoords.MinY + subTexSize - (2 * pixelOffset);

            return texCoords;
        }

        public TexCoords GetSubTextureCoordinatesOLD(int textureIndex)
        {
            TexCoords texCoords = new TexCoords();
            int superTexSize = 1024;
            float subTexSize = 32f / superTexSize;

            int col = textureIndex % 32;
            texCoords.MinX = col * subTexSize; // normalise it
            int row = textureIndex / 32;
            texCoords.MinY = 1.0f - ((row + 1) * subTexSize); // normalise, offset, flip

            texCoords.MaxX = texCoords.MinX + subTexSize;
            texCoords.MaxY = texCoords.MinY + subTexSize;
            return texCoords;
        }
        
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
