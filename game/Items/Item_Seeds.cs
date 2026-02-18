using Isola.Utilities;
using Isola.World;
using OpenTK.Mathematics;

namespace Isola.Items {
    public class Item_Seeds : Item {
        public Item_Seeds(string itemName, int textureIndex, int stackLimit = 999)
            : base(itemName, textureIndex, stackLimit) {
        }

        public override void OnRightClick(Vector2 mousePosition, WorldManager world, AssetLibrary assets) {
            ChunkTile currentTile = world.GetTile(mousePosition.X, mousePosition.Y);

            if (currentTile.GetTileName(assets) == "Tilled Soil") {
                Console.WriteLine($"Right button clicked with {ItemName} on Tilled Soil");
            }
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world, AssetLibrary assets) {

        }
    }
}
