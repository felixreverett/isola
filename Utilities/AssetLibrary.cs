using FeloxGame.Rendering;
using FeloxGame.Items;

namespace FeloxGame.Utilities
{
    public static class AssetLibrary
    {
        public static List<Item>? ItemList;

        public static List<TileData>? TileList;

        public static Dictionary<string, TextureAtlas> TextureAtlasList = new();

        public static bool GetItemFromItemName(string itemName, out Items.Item? item)
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
                // Seeds
                new Item_Seeds_WheatSeeds("Wheat Seeds", 43),
                // Tools
                new Item_Hoe("Stone Hoe", 84)
            };
        }

        public static void InitialiseTileList()
        {
            if (TileList is not null)
            {
                return;
            }

            TileList = new List<TileData>
            {
                new TileData("Grass", 0, "grass.png", 1, false),
                new TileData("Sand", 1, "sand.png", 2, false),
                new TileData("Water", 2, "water.png", 3, true),
                new TileData("Water_2", 3, "water_2.png", 4, true),
                new TileData("Water_3", 4, "water_3.png", 5, true),
                new TileData("Water_4", 5, "water_4.png", 6, true),
                new TileData("Tilled Soil", 7, "Tilled Soil.png", 7, false)
            };
        }
    }
}
