using FeloxGame.WorldClasses;

namespace FeloxGame
{
    public class Chunk
    {
        public string ChunkID { get; private set; }
        public int ChunkPosX { get; private set; }
        public int ChunkPosY { get; private set; }
        public List<Entity> ChunkEntities { get; set; }
        public ChunkTile[] ChunkTiles { get; set; }

        public Chunk(int chunkPosX, int chunkPosY)
        {
            ChunkPosX = chunkPosX;
            ChunkPosY = chunkPosY;
            ChunkID = $"x{chunkPosX}y{chunkPosY}";
            ChunkTiles = new ChunkTile[16 * 16];
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
