using FeloxGame.Core;
using FeloxGame.Core.Management;
using FeloxGame.Core.Rendering;

namespace FeloxGame
{
    public class Tile
    {
        public string Name { get; private set; }
        public string TextureLocation { get; private set; }
        public int TextureIndex { get; private set; }
        public bool IsCollidable { get; private set; } = false;

        [NonSerialized]
        protected IndexedTextureAtlas Atlas = (IndexedTextureAtlas)AssetLibrary.TextureAtlasList["Tile Atlas"];
        public TexCoords TexCoords;


        public Tile(string name, string textureLocation, int textureIndex, bool isCollidable)
        {
            Name = name;
            TextureLocation = textureLocation;
            TextureIndex = textureIndex;
            IsCollidable = isCollidable;
            TexCoords = Atlas.GetIndexedAtlasCoords(textureIndex);
        }

    }
}
