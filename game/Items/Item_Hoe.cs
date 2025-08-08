using Isola.World;
using OpenTK.Mathematics;

namespace Isola.Items
{
    public class Item_Hoe : Item
    {
        public Item_Hoe(string itemName, int textureIndex, int stackLimit = 1)
            : base(itemName, textureIndex, stackLimit)
        {
        }

        public override void OnRightClick(Vector2 mousePosition, WorldManager world)
        {
            if (world.GetTile(mousePosition.X, mousePosition.Y).GetTileName() == "Grass")
            {
                world.SetTile(mousePosition.X, mousePosition.Y, "Tilled Soil");
            }
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {

        }
    }
}
