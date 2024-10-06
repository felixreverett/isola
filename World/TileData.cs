using FeloxGame.Drawing;
using FeloxGame.Utilities;

namespace FeloxGame
{
    /// <summary>
    /// A class used to store static tile information that doesn't need to be repeated in world data
    /// </summary>
    public class TileData
    {
        public string Name { get; private set; }
        public int TileID { get; private set; }
        public string TextureLocation { get; private set; }
        public int TextureIndex { get; private set; }
        public bool IsCollidable { get; private set; } = false;
        
        protected IndexedTextureAtlasManager AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Tile Atlas"];
        public TexCoords TexCoords;


        public TileData(string name, int tileID, string textureLocation, int textureIndex, bool isCollidable)
        {
            Name = name;
            TileID = tileID;
            TextureLocation = textureLocation;
            TextureIndex = textureIndex;
            IsCollidable = isCollidable;
            TexCoords = AtlasManager.GetIndexedAtlasCoords(textureIndex);
        }

    }
}
