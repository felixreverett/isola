using FeloxGame.Drawing;
using FeloxGame.Items;

namespace FeloxGame.Utilities
{
    public static class AssetLibrary
    {
        public static List<Item>? ItemList;
        public static List<TileData>? TileList;
        public static Dictionary<string, TextureAtlasManager> TextureAtlasManagerList = new();

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
                // Seeds
                new Item_Seeds_WheatSeeds("Wheat Seeds", 43),
                // Tools
                new Item_Hoe("Stone Hoe", 84),
                // Chests
                new Item_WoodChest("Wood Chest", 126)
            };
        }

        public static void InitialiseTextureAtlasManagerList()
        {
            TextureAtlasManagerList.Add("Tile Atlas", new IndexedTextureAtlasManager(0, "1024 Tile Atlas x16.png", 1024, 16, 8, true, 0.0f));
            TextureAtlasManagerList.Add("Player Atlas", new PrecisionTextureAtlasManager(1, "Player.png", 1024, 1024, 1024, false, 0.001f));
            TextureAtlasManagerList.Add("Inventory Atlas", new PrecisionTextureAtlasManager(2, "1024 UI Atlas x16.png", 1024, 1024, 1024, false));
            TextureAtlasManagerList.Add("Item Atlas", new IndexedTextureAtlasManager(3, "1024 Item Atlas 16x.png", 1024, 16, 8, false, 0.001f));
            // Todo: add entity atlas here
        }

        public static void InitialiseTileList()
        {
            if (TileList is not null)
            {
                return;
            }

            TileList = new List<TileData>
            {
                new TileData("Grass", 0, 1, false),
                new TileData("Sand", 1, 2, false),
                new TileData("Water", 2, 3, true),
                new TileData("Water_2", 3, 4, true),
                new TileData("Water_3", 4, 5, true),
                new TileData("Water_4", 5, 6, true),
                new TileData("Tilled Soil", 7, 7, false)
            };
        }
    }
}
