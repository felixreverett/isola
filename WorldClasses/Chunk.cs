using FeloxGame.WorldClasses;

namespace FeloxGame
{
    public class Chunk
    {
        public string ChunkID { get; private set; }
        public int ChunkPosX { get; private set; }
        public int ChunkPosY { get; private set; }
        public List<Entity> ChunkEntities { get; set; }
        public ChunkTile[] Tiles { get; set; }

        public Chunk(int chunkPosX, int chunkPosY)
        {
            ChunkPosX = chunkPosX;
            ChunkPosY = chunkPosY;
            ChunkID = $"x{chunkPosX}y{chunkPosY}";
            Tiles = new ChunkTile[16 * 16];
        }

        public ChunkTile GetTile(int x, int y)
        {
            return Tiles[y * 16 + x];
        }
                
        public void SetTile(int x, int y, ChunkTile chunkTile)
        {
            Tiles[y * 16 + x] = chunkTile;
        }
    }
}
