using FeloxGame.Rendering;
using FeloxGame.GameClasses;
using FeloxGame.Utilities;
using SharpNoise;
using FeloxGame.Entities;
using System.Text.Json;
using OpenTK.Mathematics;
using System.Diagnostics; //debug

namespace FeloxGame.World
{
    /// <summary>
    /// Class to encapsulate everything a "world" would need.
    /// Currently active chunks + chunk loading/saving
    /// </summary>
    public class WorldManager : IDrawable
    {
        public string WorldName { get; set; } = "Example";
        public Dictionary<string, Chunk> LoadedChunks { get; private set; }
        public List<Entity> LoadedEntityList { get; set; }
        //private string _worldFolderPath = @"../../../Resources/World/WorldFiles";
        private string _worldFolderPath = @"../../../Saves/SampleWorldStructure";
        private GameConfig _config;
        public int Seed { get; private set; }
        public SpriteBatch batch { get; private set; }

        public WorldManager(int seed, GameConfig config)
        {
            Seed = seed;
            _config = config;
            LoadedChunks = new Dictionary<string, Chunk>();
            LoadedEntityList = new List<Entity>();
            batch = new SpriteBatch("Tile Atlas");
        }

        // Updates the world around the player
        public void Update(PlayerEntity player)
        {
            int worldX = (int)Math.Floor(player.Position.X);
            int worldY = (int)Math.Floor(player.Position.Y);
            int chunkX = worldX >= 0 ? worldX / 16 : worldX / 16 - 1;
            int chunkY = worldY >= 0 ? worldY / 16 : worldY / 16 - 1;
            // load chunks around the player
            for (int x = chunkX - _config.RenderDistance; x <= chunkX + _config.RenderDistance; x++)
            {
                for (int y = chunkY - _config.RenderDistance; y <= chunkY + _config.RenderDistance; y++)
                {
                    string chunkID = $"x{x}y{y}";
                    if (!LoadedChunks.ContainsKey(chunkID))
                    {
                        LoadOrGenerateChunk(_worldFolderPath, x, y); // load the chunk
                    }
                }
            }

            // unload chunks around the player
            foreach (Chunk chunk in LoadedChunks.Values)
            {
                if (Math.Abs(chunk.ChunkPosX - chunkX) > _config.RenderDistance || Math.Abs(chunk.ChunkPosY - chunkY) > _config.RenderDistance)
                {
                    UnloadChunk(_worldFolderPath, chunk.ChunkPosX, chunk.ChunkPosY);
                }
            }
        }
        
        public void Draw()
        {
            DrawChunks();
            DrawEntities();
        }

        private void DrawChunks()
        {
            var timer = new Stopwatch();
            timer.Start();
            foreach (Chunk loadedChunk in LoadedChunks.Values)
            {
                batch.Begin();

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        Box2 rect = new Box2(
                            loadedChunk.ChunkPosX * 16 + x,
                            loadedChunk.ChunkPosY * 16 + y,
                            loadedChunk.ChunkPosX * 16 + x + 1,
                            loadedChunk.ChunkPosY * 16 + y + 1);
                        
                        TexCoords texCoords = AssetLibrary.TileList
                            .Where(t => t.TileID == loadedChunk.GetTile(x, y).TileID)
                            .FirstOrDefault().TexCoords;

                        batch.DrawQuad(rect, texCoords);
                    }
                }

                batch.End();
            }
            timer.Stop();
            Console.WriteLine($"> Chunk draw cycle took {timer.Elapsed.ToString(@"m\:ss\.fffff")} to run.");
        }

        private void DrawEntities()
        {
            LoadedEntityList = LoadedEntityList.OrderByDescending(i => i.Position.Y).ToList();
            foreach (Entity entity in LoadedEntityList)
            {
                entity.Draw();
            }
        }

        // Handles loading/generating of all aspects of chunk: tiles, entities, tileentities
        private void LoadOrGenerateChunk(string worldFolderPath, int x, int y)
        {
            string chunkID = $"x{x}y{y}";
            string chunkTilesFilePath = $@"{worldFolderPath}/ChunkData/{chunkID}.json";
            string chunkEntityFilePath = $@"{worldFolderPath}/EntityData/{chunkID}.json";

            if (File.Exists(chunkTilesFilePath))
            {
                LoadChunkTiles(chunkID, chunkTilesFilePath);

                if (File.Exists(chunkEntityFilePath))
                {
                    LoadChunkEntities(chunkEntityFilePath);
                }
            }
            else
            {
                GenerateChunk(chunkID, x, y);
            }

            // todo: tile entities
            //string chunkTileEntityFilePath = $@"{worldFolderPath}/TileEntityData/{chunkID}.json";
        }
                
        private void LoadChunkTiles(string chunkID, string filePath)
        {
            LoadedChunks.Add(chunkID, Loading.LoadObject<Chunk>(filePath));
        }

        private void LoadChunkEntities(string chunkEntityFilePath)
        {
            // 1. Deserialize file into a list of "EntitySaveDataObject"s
            List<EntitySaveDataObject> entitySaveDataList = Loading.LoadObject<List<EntitySaveDataObject>>(chunkEntityFilePath);

            // 2. For every EntitySaveDataObject, create an entity based on the provided eEntityType
            foreach (EntitySaveDataObject entitySaveDataObject in entitySaveDataList)
            {
                switch (entitySaveDataObject.EntityType)
                {
                    case eEntityType.Entity:
                        {
                            try
                            {
                                EntitySaveData saveData = JsonSerializer.Deserialize<EntitySaveData>(entitySaveDataObject.SaveDataString);
                                Console.WriteLine("Loading new entity to world"); //debug
                                AddEntityToWorld(new Entity(saveData));
                            }
                            catch { Console.WriteLine("Loading Error: Could not load Entity"); }
                            break;
                        }
                    case eEntityType.ItemEntity:
                        {
                            try
                            {
                                ItemEntitySaveData saveData = JsonSerializer.Deserialize<ItemEntitySaveData>(entitySaveDataObject.SaveDataString);
                                Console.WriteLine("Loading new item entity to world"); //debug
                                AddEntityToWorld(new ItemEntity(saveData));
                            }
                            catch { Console.WriteLine("Loading Error: Could not load Item Entity"); }
                            break;
                        }
                    case eEntityType.PlantTileEntity:
                        {
                            try
                            {
                                PlantTileEntitySaveData saveData = JsonSerializer.Deserialize<PlantTileEntitySaveData>(entitySaveDataObject.SaveDataString);
                                Console.WriteLine("Loading new plant tile entity to world"); //debug
                                AddEntityToWorld(new PlantTileEntity(saveData));
                            }
                            catch { Console.WriteLine("Loading Error: Could not load Plant Tile Entity"); }
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Error: No entity type found");
                            break;
                        }
                }
            }
            
        }

        // Procedurally generates a chunk. todo: also generate tileEntities
        private void GenerateChunk(string chunkID, int chunkPosX, int chunkPosY)
        {
            NoiseMap noiseMap = NoiseGenerator.GenerateNoiseMap(chunkPosX, chunkPosY, 16, Seed, 200f, 4);
            Chunk newChunk = ApplyNoiseMap(noiseMap, new Chunk(chunkPosX, chunkPosY));
            LoadedChunks.Add(chunkID, newChunk);
        }

        // Todo: make this load from a terrain config file
        // Applies noisemap to set the values of a chunk.
        private Chunk ApplyNoiseMap(NoiseMap noiseMap, Chunk chunk)
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

        // ===== Saving & Unloading =====

        public void Save()
        {
            foreach (Chunk c in LoadedChunks.Values)
            {
                SaveChunkEntities(_worldFolderPath, c.ChunkPosX, c.ChunkPosY);
                SaveChunkTiles(_worldFolderPath, c.ChunkPosX, c.ChunkPosY);
            }
        }

        private void UnloadChunk(string worldFolderPath, int chunkPosX, int chunkPosY)
        {
            UnloadChunkEntities(worldFolderPath, chunkPosX, chunkPosY);
            UnloadChunkTiles(worldFolderPath, chunkPosX, chunkPosY);
        }

        // Unloads entities in the given chunk if there are any
        private void UnloadChunkEntities(string worldFolderPath, int chunkPosX, int chunkPosY)
        {
            if (GetChunkEntities(chunkPosX, chunkPosY, out List<Entity> chunkEntities))
            {
                foreach (Entity e in chunkEntities)
                {
                    LoadedEntityList.Remove(e);
                }

                if (_config.AllowSaving)
                {
                    List<EntitySaveDataObject> entitySaveDataList = new();

                    // convert entities to entitysavedata
                    foreach (Entity e in chunkEntities)
                    {
                        entitySaveDataList.Add(e.GetSaveData());
                        Console.WriteLine($"Adding entity {e.EntityType} to save list");
                    }

                    SaveChunkEntities(worldFolderPath, chunkPosX, chunkPosY, entitySaveDataList);
                }
            }
            else
            {
                Console.WriteLine($"No Entities to unload in chunk {chunkPosX}, {chunkPosY}"); //debug
            }
        }
        
        // Gets list of Chunk entities. Returns true if list count > 0. Excludes the Player Entity
        private bool GetChunkEntities(int chunkPosX, int chunkPosY, out List<Entity> chunkEntities)
        {
            chunkEntities = new List<Entity>();
            foreach (Entity e in LoadedEntityList)
            {
                if (GetChunkFromWorldCoordinate((int)e.Position.X) == chunkPosX && 
                    GetChunkFromWorldCoordinate((int)e.Position.Y) == chunkPosY &&
                    e.EntityType != eEntityType.Player)
                {
                    chunkEntities.Add(e);
                }
            }
            
            return chunkEntities.Count > 0;
        }

        private void SaveChunkEntities(string worldFolderPath, int chunkPosX, int chunkPosY)
        {
            if (_config.AllowSaving && GetChunkEntities(chunkPosX, chunkPosY, out List<Entity> chunkEntities))
            {
                List<EntitySaveDataObject> entitySaveDataList = new();

                // convert entities to entitysavedata
                foreach (Entity e in chunkEntities)
                {
                    entitySaveDataList.Add(e.GetSaveData());
                    Console.WriteLine($"Adding entity {e.EntityType} to save list");
                }

                SaveChunkEntities(worldFolderPath, chunkPosX, chunkPosY, entitySaveDataList);
                
                Console.WriteLine($"Saved entities in chunk {chunkPosX}, {chunkPosY}");
            }
        }

        private void SaveChunkEntities(string worldFolderPath, int chunkPosX, int chunkPosY, List<EntitySaveDataObject> entitySaveDataList)
        {
            Loading.SaveObject(entitySaveDataList, $"{worldFolderPath}/EntityData/x{chunkPosX}y{chunkPosY}.json");
        }

        // Removes the specified chunk from the loaded chunk list and saves if enabled
        private void UnloadChunkTiles(string worldFolderPath, int chunkPosX, int chunkPosY)
        {
            if (_config.AllowSaving)
            {
                SaveChunkTiles(worldFolderPath, chunkPosX, chunkPosY);
            }
            LoadedChunks.Remove($"x{chunkPosX}y{chunkPosY}");
        }

        // Saves a chunk and its entities in the specified world folder
        private void SaveChunkTiles(string worldFolderPath, int chunkPosX, int chunkPosY)
        {
            // Todo: add error checking
            Chunk chunk = LoadedChunks[$"x{chunkPosX}y{chunkPosY}"];
            Loading.SaveObject(chunk, $"{worldFolderPath}/ChunkData/x{chunkPosX}y{chunkPosY}.json");
        }

        // Todo: Make this update adjacent tiles
        // Updates a tile at a position in the world.
        public void SetTile(int worldX, int worldY, string tileName)
        {
            int chunkX = GetChunkFromWorldCoordinate(worldX);
            int chunkY = GetChunkFromWorldCoordinate(worldY);

            int x = worldX >= 0 ? worldX % 16 : worldX % 16 == 0 ? 0 : 16 + worldX % 16;
            int y = worldY >= 0 ? worldY % 16 : worldY % 16 == 0 ? 0 : 16 + worldY % 16;

            if (AssetLibrary.GetTileIDFromTileName(tileName, out int tileID))
            {
                try
                {
                    LoadedChunks[$"x{chunkX}y{chunkY}"].SetTile(x, y, new ChunkTile(tileID));
                }
                catch
                {
                    Console.WriteLine($"Error: could not find chunk at world coords ${worldX}, ${worldY} to update");
                }
            }
        }

        public void SetTile(float worldXf, float worldYf, string tileName)
        {
            SetTile((int)Math.Floor(worldXf), (int)Math.Floor(worldYf), tileName);
        }

        private int GetChunkFromWorldCoordinate(int worldCoordinate)
        {
            return worldCoordinate >= 0 ? worldCoordinate / 16 : worldCoordinate % 16 == 0 ? worldCoordinate / 16 : worldCoordinate / 16 - 1;
        }

        /// <summary>
        /// Gets a tile at a position in the world by polling the appropriate chunk
        /// </summary>
        /// <param name="worldX"></param>
        /// <param name="worldY"></param>
        /// <returns></returns>
        public ChunkTile GetTile(int worldX, int worldY) //todo: change to bool with data validation and out var
        {
            int chunkX = worldX >= 0 ? worldX / 16 : worldX % 16 == 0 ? worldX / 16 : worldX / 16 - 1;
            int chunkY = worldY >= 0 ? worldY / 16 : worldY % 16 == 0 ? worldY / 16 : worldY / 16 - 1;

            int x = worldX >= 0 ? worldX % 16 : worldX % 16 == 0 ? 0 : 16 + worldX % 16;
            int y = worldY >= 0 ? worldY % 16 : worldY % 16 == 0 ? 0 : 16 + worldY % 16;
                        
            return LoadedChunks[$"x{chunkX}y{chunkY}"].GetTile(x, y);
        }

        /// <summary>
        /// An overload for GetTile for float inputs
        /// </summary>
        /// <param name="worldXf"></param>
        /// <param name="worldYf"></param>
        /// <returns></returns>
        public ChunkTile GetTile(float worldXf, float worldYf)
        {
            return GetTile((int)Math.Floor(worldXf), (int)Math.Floor(worldYf));
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
