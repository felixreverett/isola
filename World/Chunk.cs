using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeloxGame
{
    public class Chunk
    {
        public Tile[,] Tiles { get; set; } = new Tile[16, 16];
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
