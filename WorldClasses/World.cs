using FeloxGame.Core;
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
        public List<Entity> LoadedEntityList { get; set; }
        //private string _worldFolderPath = @"../../../Resources/World/WorldFiles";
        private string _worldFolderPath = @"../../../Saves/SampleWorldStructure/ChunkData";
        private Config _config = new Config(true); // Todo: make config load from Game1
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
        private IndexedTextureAtlas WorldTextureAtlas { get; set; }

        public World(int seed = 1)
        {
            LoadedChunks = new Dictionary<string, Chunk>();
            LoadedEntityList = new List<Entity>();
            Seed = seed;
            OnLoad();
        }

        public void OnLoad()
        {
            WorldTextureAtlas = (IndexedTextureAtlas)AssetLibrary.TextureAtlasList["Tile Atlas"];

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
                        Chunk newChunk = LoadOrGenerateChunk(_worldFolderPath, x, y); // load the chunk
                        LoadedChunks.Add(chunkID, newChunk);
                    }
                }
            }

            // unload chunks around the player
            foreach (Chunk chunk in LoadedChunks.Values)
            {
                if (Math.Abs(chunk.ChunkPosX - chunkX) > player.RenderDistance || Math.Abs(chunk.ChunkPosY - chunkY) > player.RenderDistance)
                {
                    UnloadChunk(_worldFolderPath, chunk.ChunkPosX, chunk.ChunkPosY);
                    //LoadedChunks.Remove($"x{chunk.ChunkPosX}y{chunk.ChunkPosY}");
                }
            }
        }

        // ===== Rendering =====

        public void Draw()
        {
            WorldTextureAtlas.Texture.Use();
            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();
                        
            DrawChunks();

            DrawEntities();
        }

        private void DrawChunks()
        {
            //var Stopwatch = System.Diagnostics.Stopwatch.StartNew();
            foreach (Chunk loadedChunk in LoadedChunks.Values)
            {
                
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        _vertices[0]  = loadedChunk.ChunkPosX * 16 + x + 1; _vertices[1]  = loadedChunk.ChunkPosY * 16 + y + 1; // top right (1, 1)
                        _vertices[8]  = loadedChunk.ChunkPosX * 16 + x + 1; _vertices[9]  = loadedChunk.ChunkPosY * 16 + y;     // bottom right (1, 0)
                        _vertices[16] = loadedChunk.ChunkPosX * 16 + x;     _vertices[17] = loadedChunk.ChunkPosY * 16 + y;     // bottom left (0, 0)
                        _vertices[24] = loadedChunk.ChunkPosX * 16 + x;     _vertices[25] = loadedChunk.ChunkPosY * 16 + y + 1; // top left (0, 1)
                        
                        ChunkTile currentTile = loadedChunk.GetTile(x, y);
                        TexCoords texCoords = AssetLibrary.TileList.Where(t => t.TileID == currentTile.TileID).FirstOrDefault().TexCoords;
                        
                        _vertices[3] = texCoords.MaxX; _vertices[4] = texCoords.MaxY;   // (1, 1)
                        _vertices[11] = texCoords.MaxX; _vertices[12] = texCoords.MinY; // (1, 0)
                        _vertices[19] = texCoords.MinX; _vertices[20] = texCoords.MinY; // (0, 0)
                        _vertices[27] = texCoords.MinX; _vertices[28] = texCoords.MaxY; // (0, 1)

                        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * _vertices.Length, _vertices);
                        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
                    }
                }
                
            }
            //Console.WriteLine($"This frame: {Stopwatch.Elapsed.TotalMilliseconds}");
        }

        private void DrawEntities()
        {
            LoadedEntityList = LoadedEntityList.OrderByDescending(i => i.Position.Y).ToList();
            foreach (Entity entity in LoadedEntityList)
            {
                entity.Draw();
            }
        }

        // ===== Terrain Generation & Loading =====

        public Chunk LoadOrGenerateChunk(string folderPath, int chunkPosX, int chunkPosY)
        {
            string filePath = folderPath + $@"/x{chunkPosX}y{chunkPosY}.json";
            string filePathTest = folderPath + "/x0y0.json"; //test
            if (File.Exists(filePath))
            {
                return LoadChunk(filePath);
            }
            else
            {
                return GenerateChunk(chunkPosX, chunkPosY);
            }
        }

        /// <summary>
        /// Deserialises a JSON chunk given a filepath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Chunk LoadChunk(string filePath)
        {
            Chunk newChunk = Loading.LoadObject<Chunk>(filePath);
            return newChunk;
        }

        /// <summary>
        /// Procedurally generates a chunk given chunk coordinates and a seed
        /// </summary>
        /// <param name="chunkPosX"></param>
        /// <param name="chunkPosY"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public Chunk GenerateChunk(int chunkPosX, int chunkPosY, int seed = 1)
        {
            NoiseMap noiseMap = NoiseGenerator.GenerateNoiseMap(chunkPosX, chunkPosY, 16, Seed, 200f, 4);
            Chunk newChunk = ApplyNoiseMap(noiseMap, new Chunk(chunkPosX, chunkPosY));
            return newChunk;
        }

        // Todo: make this load from a terrain config file
        /// <summary>
        /// Applies noisemap to set the values of a chunk.
        /// </summary>
        /// <param name="noiseMap"></param>
        /// <param name="chunk"></param>
        /// <returns></returns>
        public Chunk ApplyNoiseMap(NoiseMap noiseMap, Chunk chunk)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    float noiseValue = noiseMap.GetValue(x, y);
                    if (noiseValue < -0.35f)
                    {
                        chunk.SetTile(x, y, new ChunkTile(5)); // Water_4
                    }
                    else if (noiseValue < -0.25f)
                    {
                        chunk.SetTile(x, y, new ChunkTile(4)); // Water_3
                    }
                    else if (noiseValue < -0.15f)
                    {
                        chunk.SetTile(x, y, new ChunkTile(3)); // Water_2
                    }
                    else if (noiseValue < -0.1f)
                    {
                        chunk.SetTile(x, y, new ChunkTile(2)); // Water
                    }
                    else if (noiseValue < 0.0f)
                    {
                        chunk.SetTile(x, y, new ChunkTile(1)); // Sand
                    }
                    else
                    {
                        chunk.SetTile(x, y, new ChunkTile(0)); // Grass
                    }
                    // -0.35, -0.25, -0.15, -0.1, 0, 0.1, 0.2, 0.5, 0.7, 0.8, 0.85, 0.9, 1
                }
            }

            return chunk;
        }

        /// <summary>
        /// Saves a chunk in the specified folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="chunkPosX"></param>
        /// <param name="chunkPosY"></param>
        public void SaveChunk(string folder, int chunkPosX, int chunkPosY)
        {
            // Todo: add error checking
            Chunk chunk = LoadedChunks[$"x{chunkPosX}y{chunkPosY}"];
            Loading.SaveObject(chunk, $"{folder}/x{chunkPosX}y{chunkPosY}.json");
        }

        public void UnloadChunk(string folder, int chunkPosX, int chunkPosY)
        {
            if (_config.AllowSaving)
            {
                UnloadChunkEntities(chunkPosX, chunkPosY);
                SaveChunk(folder, chunkPosX, chunkPosY);
            }
            LoadedChunks.Remove($"x{chunkPosX}y{chunkPosY}");
        }

        public void UnloadChunkEntities(int chunkPosX, int chunkPosY)
        {
            List<Entity> entitiesToRemove = new List<Entity>();

            foreach (Entity entity in LoadedEntityList)
            {
                if (GetChunkFromWorldCoordinate((int)Math.Floor(entity.Position.Y)) == chunkPosY
                    &&
                    GetChunkFromWorldCoordinate((int)Math.Floor(entity.Position.X)) == chunkPosX)
                {
                    entitiesToRemove.Add(entity);
                }
            }

            foreach (Entity entity in entitiesToRemove)
            {
                LoadedChunks[$"x{chunkPosX}y{chunkPosY}"].ChunkEntities.Add(entity);
                LoadedEntityList.Remove(entity);
            }
        }

        // Todo: Make this update adjacent tiles
        /// <summary>
        /// Updates a tile at a position in the world.
        /// </summary>
        /// <param name="worldX"></param>
        /// <param name="worldY"></param>
        public void UpdateTile(int worldX, int worldY) //Todo: add error checking
        {
            int chunkX = GetChunkFromWorldCoordinate(worldX);
            int chunkY = GetChunkFromWorldCoordinate(worldY);

            int x = worldX >= 0 ? worldX % 16 : worldX % 16 == 0 ? 0 : 16 + worldX % 16;
            int y = worldY >= 0 ? worldY % 16 : worldY % 16 == 0 ? 0 : 16 + worldY % 16;
            
            // Todo: prevent this from being hard-coded
            LoadedChunks[$"x{chunkX}y{chunkY}"].SetTile(x, y, new ChunkTile(0)); // sets to grass
        }

        public int GetChunkFromWorldCoordinate(int worldCoordinate)
        {
            return worldCoordinate >= 0 ? worldCoordinate / 16 : worldCoordinate % 16 == 0 ? worldCoordinate / 16 : worldCoordinate / 16 - 1;
        }

        /// <summary>
        /// Gets a tile at a position in the world by polling the appropriate chunk
        /// </summary>
        /// <param name="worldX"></param>
        /// <param name="worldY"></param>
        /// <returns></returns>
        public ChunkTile GetTile(int worldX, int worldY)
        {
            int chunkX = worldX >= 0 ? worldX / 16 : worldX % 16 == 0 ? worldX / 16 : worldX / 16 - 1;
            int chunkY = worldY >= 0 ? worldY / 16 : worldY % 16 == 0 ? worldY / 16 : worldY / 16 - 1;

            int x = worldX >= 0 ? worldX % 16 : worldX % 16 == 0 ? 0 : 16 + worldX % 16;
            int y = worldY >= 0 ? worldY % 16 : worldY % 16 == 0 ? 0 : 16 + worldY % 16;

            return LoadedChunks[$"x{chunkX}y{chunkY}"].GetTile(x, y);
        }

        /// <summary>
        /// Adds an entity to the world's entity list
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntityToWorld(Entity entity)
        {
            LoadedEntityList.Add(entity);
        }
    }
}
