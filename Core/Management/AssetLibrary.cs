using FeloxGame.Core.Rendering;

namespace FeloxGame.Core
{
    public static class AssetLibrary
    {
        public static List<Item> ItemList;

        public static List<Tile> TileList;

        public static Dictionary<string, TextureAtlas> TextureAtlasList = new();
    }
}
