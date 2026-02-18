using Isola.Entities;
using Isola.Utilities;
using Isola.World;
using OpenTK.Mathematics;

namespace Isola.Items {
    public class Item_WoodChest : Item {
        public Item_WoodChest(string itemName, int textureIndex, int stackLimit = 999)
            : base(itemName, textureIndex, stackLimit) {
        }

        public override void OnRightClick(Vector2 mousePosition, WorldManager world, AssetLibrary assets) {
            Console.WriteLine($"Adding new {ItemName} to world.");
            world.AddEntityToWorld(new TileEntity_Chest(mousePosition, new Vector2(0f, 0f), assets));
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world, AssetLibrary assets) {

        }
    }
}
