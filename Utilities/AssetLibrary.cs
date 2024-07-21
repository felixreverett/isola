using FeloxGame.Rendering;
using FeloxGame.Items;

namespace FeloxGame.Utilities
{
    public static class AssetLibrary
    {
        public static List<Item>? ItemList;

        public static List<TileData>? TileList;

        public static Dictionary<string, TextureAtlas> TextureAtlasList = new();

        public static bool GetItemFromItemName(string itemName, out Item? item)
        {
            item = ItemList!.FirstOrDefault(i => i.ItemName == itemName);

            if (item != null)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public static bool GetTileIDFromTileName(string tileName, out int tileID)
        {
            tileID = TileList.FirstOrDefault(i => i.Name == tileName).TileID;
            return tileID != -1;
        }

        public static void InitialiseItemList()
        {
            if (ItemList is not null)
            {
                return;
            }

            ItemList = new List<Item>
            {
                new Item("Persimmon", 1),
                new RiceSeedsItem("RiceSeeds", 43),
                new Hoe("Stone Hoe", 84)
            };
        }
    }
}
