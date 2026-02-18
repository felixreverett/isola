using Isola.Drawing;
using Isola.Utilities;

namespace Isola {
    /// <summary>
    /// A class used to store static tile information that doesn't need to be repeated in world data
    /// </summary>
    public class TileData {
        public string Name { get; init; }
        public int TileID { get; init; }
        public int TextureIndex { get; init; }
        public bool IsCollidable { get; init; }      
        public TexCoords TexCoords;

        public TileData(string name, int tileID, int textureIndex, bool isCollidable) {
            Name = name;
            TileID = tileID;
            TextureIndex = textureIndex;
            IsCollidable = isCollidable;
        }

        public void CalculateTexCoords(IndexedTextureAtlasManager atlasManager) {
            TexCoords = atlasManager.GetIndexedAtlasCoords(TextureIndex);
        }

    }
}
