using Isola.Drawing;
using Isola.Utilities;

namespace Isola
{
    /// <summary>
    /// A class used to store static tile information that doesn't need to be repeated in world data
    /// </summary>
    public class TileData
    {
        public string Name { get; private set; }
        public int TileID { get; private set; }
        public int TextureIndex { get; private set; }
        public bool IsCollidable { get; private set; } = false;
        
        protected IndexedTextureAtlasManager AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Tile Atlas"];
        public TexCoords TexCoords;


        public TileData(string name, int tileID, int textureIndex, bool isCollidable)
        {
            Name = name;
            TileID = tileID;
            TextureIndex = textureIndex;
            IsCollidable = isCollidable;
            TexCoords = AtlasManager.GetIndexedAtlasCoords(textureIndex);
        }

    }
}
