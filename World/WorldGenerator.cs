namespace FeloxGame.World
{
    public class WorldGenerator
    {
        private static WorldGenerator _instance = null;
        private static readonly object _loc = new();

        public static WorldGenerator Instance
        {
            get
            {
                lock (_loc)
                {
                    if (_instance == null)
                    {
                        _instance = new WorldGenerator();
                    }
                    return _instance;
                }
            }
        }
        public Chunk LoadChunk(string filePath) //will replace with source later (world loaded into memory?)
        {
            string[] rows = File.ReadAllText(filePath).Trim().Replace("\r", "").Split("\n").ToArray();
            Chunk newChunk = new(0, 0);

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

        /// <summary>
        /// Run this code if existing chunk cannot be found on load
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public Chunk GenerateChunk(int seed)
        {
            return new Chunk(0, 0);
        }
    }
}
