namespace FeloxGame
{
    public class Chunk
    {
        public string ChunkID { get; private set; }
        public int ChunkPosX { get; private set; }
        public int ChunkPosY { get; private set; }
        public string[] Tiles { get; set; } = new string[16*16];
        public List<Entity> ChunkEntities { get; set; }
        
        public Chunk(int chunkPosX, int chunkPosY)
        {
            ChunkPosX = chunkPosX;
            ChunkPosY = chunkPosY;
            ChunkID = $"x{chunkPosX}y{chunkPosY}";
        }

        public string GetTile(int x, int y)
        {
            return Tiles[y * 16 + x];
        }

        public void SetTile(int x, int y, string tileId)
        {
            Tiles[y* 16 + x] = tileId;
        }
    }
}
