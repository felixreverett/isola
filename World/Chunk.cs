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
        public Tile[,] Tiles { get; set; } = new Tile[16, 16];
        //public List<Entity> Entities { get; set; }

        public Tile GetTile(int x, int y)
        {
            return Tiles[x, y];
        }

        public void SetTile()
        {

        }

        public static Chunk LoadChunk(string filePath)
        {
            string[] rows = File.ReadAllText(filePath).Trim()
                .Replace("\r", "").Split("\n").ToArray();
            Chunk newChunk = new();

            for (int y = 0; y < rows.Length; y++)
            {
                string row = rows[y];
                string[] cols = row.Split(" ");
                for (int x = 0; x < cols.Length; x++)
                {
                    newChunk.Tiles[x, y] = new Tile(cols[x]);
                }
            }

            return newChunk;
        }
    }
}
