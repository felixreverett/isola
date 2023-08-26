namespace FeloxGame
{
    public class Item
    {
        public string ItemName { get; set; }
        public int TextureIndex { get; set; }
        
        // item category or type
        public Item(string itemName, int textureIndex)
        {
            ItemName = itemName;
            TextureIndex = textureIndex;
        }
    }
}
