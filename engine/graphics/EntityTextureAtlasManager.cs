using Isola.Drawing;

namespace Isola.engine.graphics
{
    public struct EntityAtlasData
    {
        public EntityAtlasData(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    public class EntityTextureAtlasManager : IAtlasManager
    {
        public string AtlasFileName { get; set; }
        private int AtlasWidth { get; set; }
        private int AtlasHeight { get; set; }
        private Dictionary<string, EntityAtlasData> EntityAtlasData { get; set; } = new Dictionary<string, EntityAtlasData>();

        public EntityTextureAtlasManager(string fileName, int atlasWidth, int atlasHeight)
        {
            AtlasFileName = fileName;
            AtlasWidth = atlasWidth;
            AtlasHeight = atlasHeight;
            BuildAtlasData();
        }

        public TexCoords GetAtlasCoords(string entityId)
        {
            TexCoords texCoords = new TexCoords();

            if (EntityAtlasData.TryGetValue(entityId, out EntityAtlasData eAData))
            {
                texCoords.MinX = (float)eAData.X / AtlasWidth;
                texCoords.MaxX = (float)(eAData.X + eAData.Width) / AtlasWidth;
                texCoords.MinY = (float)eAData.Y / AtlasHeight;
                texCoords.MaxY = (float)(eAData.Y + eAData.Height) / AtlasHeight;
            }
            else
            {
                Console.WriteLine($"Warning: Entity '{entityId}' not found in atlas map.");
                return new TexCoords(0, 0, 1, 1);
            }
            return texCoords;
        }

        private void BuildAtlasData()
        {
            // row 0
            int y = 0;
            EntityAtlasData.Add("Player", new EntityAtlasData(0, y, 16, 32));

            // row 1
            y += 24;

            // row 2
            y += 24;
            EntityAtlasData.Add("Null", new EntityAtlasData(0, y, 16, 16));
            EntityAtlasData.Add("Persimmon", new EntityAtlasData(24, y, 16, 16));

            // row 3
            y += 24;
            EntityAtlasData.Add("Wheat Seeds", new EntityAtlasData(24, y, 16, 16));

            // row 4
            y += 24;
            EntityAtlasData.Add("Stone Hoe", new EntityAtlasData(0, y, 16, 16));

            // row 5
            y += 24;
            EntityAtlasData.Add("Wood Chest", new EntityAtlasData(0, y, 16, 16));
        }
    }
}
