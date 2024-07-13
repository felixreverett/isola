using FeloxGame.Rendering;
using FeloxGame.Items;

namespace FeloxGame.Utilities
{
    public static class AssetLibrary
    {
        public static List<Item>? ItemList;

        public static List<TileData> TileList;

        public static Dictionary<string, TextureAtlas> TextureAtlasList = new();

        public static bool GetItemFromItemName(string itemName, out Item? item)
        {
            item = ItemList.Where(i => i.ItemName == itemName).FirstOrDefault();

            if (item != null)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public static void InitialiseItemList()
        {
            if (ItemList is not null)
            {
                return;
            }

            ItemList = new List<Item>();
            ItemList.Add(new Item("Persimmon", 0));
            ItemList.Add(new RiceSeedsItem("RiceSeeds", 42));
        }
    }
}
