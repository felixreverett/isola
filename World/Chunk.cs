using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeloxGame
{
    public class Chunk
    {
        public string ChunkID { get; private set; }
        public int ChunkPosX { get; private set; }
        public int ChunkPosY { get; private set; }
        public Tile[,] Tiles { get; set; } = new Tile[16, 16];
        
        public Chunk(int chunkPosX, int chunkPosY)
        {
            ChunkPosX = chunkPosX;
            ChunkPosY = chunkPosY;
            ChunkID = $"x{chunkPosX}y{chunkPosY}";
        }

        //public List<Entity> Entities { get; set; }

        public Tile GetTile(int x, int y)
        {
            return Tiles[x, y];
        }

        public void SetTile()
        {

        }
    }
}
