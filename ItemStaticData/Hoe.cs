using FeloxGame.World;
using OpenTK.Mathematics;

namespace FeloxGame.ItemStaticData
{
    public class Hoe : Item
    {
        public Hoe(string itemName, int textureIndex, int stackLimit = 1)
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
                world.SetTile(mousePosition.X, mousePosition.Y, "Tilled Soil");
            }
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {

        }
    }
}
