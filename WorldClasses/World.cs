using FeloxGame.Core.Management;
using FeloxGame.Core.Rendering;
using OpenTK.Graphics.OpenGL4;
using SharpNoise;

namespace FeloxGame.WorldClasses // rename this later?
{
    /// <summary>
    /// Class to encapsulate everything a "world" would need.
    /// Currently active chunks + chunk loading/saving
    /// </summary>
    public class World
    {
        public Dictionary<string, Chunk> LoadedChunks { get; private set; }
        private string _worldFolderPath = @"../../../Resources/World/WorldFiles";
        public int Seed { get; private set; }

        // Rendering
        private readonly float[] _vertices =
        {   //Vertices        //texCoords //texColors
            1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //top right (1,1)
            1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom right (1, 0)
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom left (0, 0)
            0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f  //top left (0, 1)
        };

        private uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;
        private Texture2D WorldTexture { get; set; }

        public World(int seed = 1)
        {
            LoadedChunks = new Dictionary<string, Chunk>();
            Seed = seed;
            OnLoad();
        }

        public void OnLoad()
        {
            WorldTexture = ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/WorldTextures.png", 0);

            _vertexArray = new();
            _vertexBuffer = new VertexBuffer(_vertices);

            BufferLayout layout = new();
            layout.Add<float>(3); // Positions
            layout.Add<float>(2); // Texture Coords
            layout.Add<float>(3); // Texture Color
            //layout.Add<float>(1); // Texture Slot

            _vertexArray.AddBuffer(_vertexBuffer, layout);
            _indexBuffer = new IndexBuffer(_indices);
        }

        public void Update(Player player)
        {
            int worldX = (int)Math.Floor(player.Position.X);
            int worldY = (int)Math.Floor(player.Position.Y);
            int chunkX = worldX >= 0 ? worldX / 16 : worldX / 16 - 1;
            int chunkY = worldY >= 0 ? worldY / 16 : worldY / 16 - 1;
            // load chunks around the player
            for (int x = chunkX - player.RenderDistance; x <= chunkX + player.RenderDistance; x++)
            {
                for (int y = chunkY - player.RenderDistance; y <= chunkY + player.RenderDistance; y++)
                {
                    string chunkID = $"x{x}y{y}";
                    if (!LoadedChunks.ContainsKey(chunkID))
                    {
                        Chunk newChunk = LoadOrGenerateChunk($"{_worldFolderPath}/{chunkID}.txt", x, y); // load the chunk
                        LoadedChunks.Add(chunkID, newChunk);
                    }
                }
            }

            // unload chunks around the player
            foreach (Chunk chunk in LoadedChunks.Values)
            {
                if (Math.Abs(chunk.ChunkPosX - chunkX) > player.RenderDistance || Math.Abs(chunk.ChunkPosY - chunkY) > player.RenderDistance)
                {
                    LoadedChunks.Remove($"x{chunk.ChunkPosX}y{chunk.ChunkPosY}");
                }
            }
        }

        public void Draw(List<Tile> _tileList) // todo: remove inputs?
        {
            WorldTexture.Use();
            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();

            foreach (Chunk loadedChunk in LoadedChunks.Values)
            {
                for (int y = 0; y < loadedChunk.Tiles.GetLength(1); y++)
                {
                    for (int x = 0; x < loadedChunk.Tiles.GetLength(0); x++)
                    {
                        _vertices[0] = loadedChunk.ChunkPosX * 16 + x + 1; _vertices[1] = loadedChunk.ChunkPosY * 16 + y + 1; // top right (1, 1)
                        _vertices[8] = loadedChunk.ChunkPosX * 16 + x + 1; _vertices[9] = loadedChunk.ChunkPosY * 16 + y; // bottom right (1, 0)
                        _vertices[16] = loadedChunk.ChunkPosX * 16 + x; _vertices[17] = loadedChunk.ChunkPosY * 16 + y; // bottom left (0, 0)
                        _vertices[24] = loadedChunk.ChunkPosX * 16 + x; _vertices[25] = loadedChunk.ChunkPosY * 16 + y + 1; // top left (0, 1)
                        string textureName = loadedChunk.Tiles[x, y];
                        int textureIndex = _tileList.Where(t => t.Name.ToLower() == textureName.ToLower()).FirstOrDefault().TextureIndex;
                        TexCoords texCoords = WorldManager.Instance.GetSubTextureCoordinates(textureIndex);
                        _vertices[3] = texCoords.MaxX; _vertices[4] = texCoords.MaxY;   // (1, 1)
                        _vertices[11] = texCoords.MaxX; _vertices[12] = texCoords.MinY; // (1, 0)
                        _vertices[19] = texCoords.MinX; _vertices[20] = texCoords.MinY; // (0, 0)
                        _vertices[27] = texCoords.MinX; _vertices[28] = texCoords.MaxY; // (0, 1)
                        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * _vertices.Length, _vertices);
                        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
                    }
                }
            }
        }



        // Terrain Generation code

        public Chunk LoadOrGenerateChunk(string filePath, int chunkPosX, int chunkPosY)
        {
            if (File.Exists(filePath))
            {
                return LoadChunk(filePath, chunkPosX, chunkPosY);
            }
            else
            {
                return GenerateChunk(chunkPosX, chunkPosY);
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

        public Chunk GenerateChunk(int chunkPosX, int chunkPosY, int seed = 1)
        {
            NoiseMap noiseMap = NoiseGenerator.GenerateNoiseMap(chunkPosX, chunkPosY, 16, Seed, 200f, 4);
            Chunk newChunk = ApplyNoiseMap(noiseMap, new Chunk(chunkPosX, chunkPosY));
            return newChunk;
        }

        public Chunk ApplyNoiseMap(NoiseMap noiseMap, Chunk chunk)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    float noiseValue = noiseMap.GetValue(x, y);
                    if (noiseValue < -0.35f)
                    {
                        chunk.SetTile(x, y, "Water_4");
                    }
                    else if (noiseValue < -0.25f)
                    {
                        chunk.SetTile(x, y, "Water_3");
                    }
                    else if (noiseValue < -0.15f)
                    {
                        chunk.SetTile(x, y, "Water_2");
                    }
                    else if (noiseValue < -0.1f)
                    {
                        chunk.SetTile(x, y, "Water");
                    }
                    else if (noiseValue < 0.0f)
                    {
                        chunk.SetTile(x, y, "Sand");
                    }
                    else
                    {
                        chunk.SetTile(x, y, "Grass");
                    }
                    // -0.35, -0.25, -0.15, -0.1, 0, 0.1, 0.2, 0.5, 0.7, 0.8, 0.85, 0.9, 1
                }
            }

            return chunk;
        }

        public void SaveChunk(string folder, Chunk chunk)
        { }



        // Updates (code for world interaction)
        public void UpdateTile(int worldX, int worldY) //Todo: add error checking
        {
            int chunkX = worldX >= 0 ? worldX / 16 : worldX / 16 -1;
            int chunkY = worldY >= 0 ? worldY / 16 : worldY / 16 -1;

            int x = worldX >= 0 ? worldX % 16 : 16 + worldX % 16;
            int y = worldY >= 0 ? worldY % 16 : 16 + worldY % 16;
            
            LoadedChunks[$"x{chunkX}y{chunkY}"].Tiles[x, y] = "Grass";
        }
    }
}
