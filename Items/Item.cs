using FeloxGame.World;
using OpenTK.Mathematics;

namespace FeloxGame.Items
{
    // Represents item functionality
    public class Item
    {
        public string ItemName { get; set; }
        public int TextureIndex { get; set; }
        public int StackLimit { get; set; }

        // item category or type
        public Item(string itemName, int textureIndex, int stackLimit = 999)
        {
            ItemName = itemName;
            TextureIndex = textureIndex;
            StackLimit = stackLimit;
        }

        public virtual void OnRightClick(Vector2 mousePosition, WorldManager world)
        {
            Console.WriteLine("Right button clicked");
        }

        public virtual void OnLeftClick(Vector2 mousePosition, WorldManager world) { }
    }
}
