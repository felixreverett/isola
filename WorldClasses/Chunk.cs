using FeloxGame.EntityClasses;
using FeloxGame.WorldClasses;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeloxGame
{
    public class Chunk
    {
        public string ChunkID { get; set; } // todo: make private again
        public int ChunkPosX { get; set; } // todo: make private again
        public int ChunkPosY { get; set; } // todo: make private again
        public List<EntitySaveData> ChunkEntitySaveData { get; set; }
        public ChunkTile[] ChunkTiles { get; set; }

        public Chunk(int chunkPosX, int chunkPosY)
        {
            ChunkPosX = chunkPosX;
            ChunkPosY = chunkPosY;
            ChunkID = $"x{chunkPosX}y{chunkPosY}";
            ChunkTiles = new ChunkTile[16 * 16];
            ChunkEntitySaveData = new();
        }

        [JsonConstructor] public Chunk(int chunkPosX, int chunkPosY, ChunkTile[] chunkTiles, List<EntitySaveData> chunkEntitySaveData)
        {
            ChunkPosX = chunkPosX;
            ChunkPosY = chunkPosY;
            ChunkID = $"x{chunkPosX}y{chunkPosY}";
            ChunkTiles = chunkTiles;
            ChunkEntitySaveData = chunkEntitySaveData;
        }

        public ChunkTile GetTile(int x, int y)
        {
            return ChunkTiles[y * 16 + x];
        }
                
        public void SetTile(int x, int y, ChunkTile chunkTile)
        {
            ChunkTiles[y * 16 + x] = chunkTile;
        }
    }
}
