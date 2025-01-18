using FeloxGame.World;
using OpenTK.Mathematics;

namespace FeloxGame.Items
{
    public class Item_WoodChest : Item
    {
        public Item_WoodChest(string itemName, int textureIndex, int stackLimit = 999)
            : base(itemName, textureIndex, stackLimit)
        {
        }

        public override void OnRightClick(Vector2 mousePosition, WorldManager world)
        {
            Console.WriteLine($"Right button clicked with {ItemName} on Tilled Soil");
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {

        }
    }
}
