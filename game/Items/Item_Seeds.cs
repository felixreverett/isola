using FeloxGame.World;
using OpenTK.Mathematics;

namespace FeloxGame.Items
{
    public class Item_Seeds : Item
    {
        public Item_Seeds(string itemName, int textureIndex, int stackLimit = 999)
            : base(itemName, textureIndex, stackLimit)
        {
        }

        public override void OnRightClick(Vector2 mousePosition, WorldManager world)
        {
            if (world.GetTile(mousePosition.X, mousePosition.Y).GetTileName() == "Tilled Soil")
            {
                Console.WriteLine($"Right button clicked with {ItemName} on Tilled Soil");
            }
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {

        }
    }
}
