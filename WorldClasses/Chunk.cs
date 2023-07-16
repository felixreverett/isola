namespace FeloxGame
{
    public class Chunk
    {
        public string ChunkID { get; private set; }
        public int ChunkPosX { get; private set; }
        public int ChunkPosY { get; private set; }
        public string[,] Tiles { get; set; } = new string[16, 16];
        
        public Chunk(int chunkPosX, int chunkPosY)
        {
            ChunkPosX = chunkPosX;
            ChunkPosY = chunkPosY;
            ChunkID = $"x{chunkPosX}y{chunkPosY}";
        }

        //public List<Entity> Entities { get; set; }

        public string GetTile(int x, int y)
        {
            return Tiles[x, y];
        }

        public void SetTile(int x, int y, string tileId)
        {
            Tiles[x, y] = tileId;
        }
    }
}
