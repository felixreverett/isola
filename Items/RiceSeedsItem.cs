using FeloxGame.World;
using OpenTK.Mathematics;

namespace FeloxGame.Items
{
    public class RiceSeedsItem : Item
    {
        public RiceSeedsItem(string itemName, int textureIndex, int stackLimit = 999)
            : base(itemName, textureIndex, stackLimit)
        {
            ItemName = itemName;
            TextureIndex = textureIndex;
            StackLimit = stackLimit;
        }

        public override void OnRightClick(Vector2 mousePosition, WorldManager world)
        {
            if (world.GetTile(mousePosition.X, mousePosition.Y).GetTileName() == "Grass")
            {
                Console.WriteLine("Right button clicked with rice on grass");
            }
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {

        }
    }
}
