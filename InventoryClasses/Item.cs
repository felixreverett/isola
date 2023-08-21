namespace FeloxGame
{
    public class Item
    {

        public string Name { get; set; }
        public int TextureIndex { get; set; }
        
        // item category or type
        public Item(string name, int textureIndex)
        {
            Name = name;
            TextureIndex = textureIndex;
        }
    }
}
