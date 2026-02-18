using Isola.Utilities;
using Isola.Drawing;
using Isola.Inventories;
using Isola.Items;
using OpenTK.Mathematics;
using Isola.World;

namespace Isola.ui {
    public class SlotUI : UI {
        protected int ItemSlotID;
        protected Inventory Inventory;
        protected PlayerEntity OwnerPlayer;
        protected IndexedTextureAtlasManager AtlasManager;

        public SlotUI (
            float width, float height, eAnchor anchor, float scale, AssetLibrary assets, bool isDrawable, bool toggleDraw, bool isClickable,
            int itemSlotID, Inventory inventory, float x, float y, PlayerEntity ownerPlayer, string atlasName
        ) : base(width, height, anchor, scale, assets,isDrawable, toggleDraw, isClickable) {
            ItemSlotID = itemSlotID;
            Inventory = inventory;
            OwnerPlayer = ownerPlayer;
            LocalRect.X = x;
            LocalRect.Y = y;
            AtlasManager = (IndexedTextureAtlasManager)_assets.TextureAtlasManagerList[atlasName];
            BatchRenderer = _assets.BatchRendererList[atlasName];
        }

        public override void Update() {
            ItemStack itemStack = OwnerPlayer.Inventory.ItemStackList[ItemSlotID];

            if (itemStack.Equals(default(ItemStack))) {
                ToggleDraw = false;
            } else {
                int textureIndex = 0;
                Item matchingItem = _assets.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);
                if (matchingItem != null) {
                    textureIndex = matchingItem.TextureIndex;
                    TexCoords = AtlasManager.GetIndexedAtlasCoords(textureIndex);
                } else {
                    Console.WriteLine($"Error: No matching item found in library with name {itemStack.ItemName}. Using default texture 0.");
                }
                ToggleDraw = true;
            }
            base.Update();
        }

        public override void OnLeftClick(Vector2 mousePixels, WorldManager world) {
            // Swap slots
            if (IsMouseInBounds(mousePixels)) {
                ItemStack itemStack = OwnerPlayer.Inventory.ItemStackList[ItemSlotID];

                if (itemStack.ItemName == OwnerPlayer.Inventory.MouseSlotItemStack.ItemName) {
                    OwnerPlayer.Inventory.ItemStackList[ItemSlotID].Amount += OwnerPlayer.Inventory.MouseSlotItemStack.Amount;
                    OwnerPlayer.Inventory.MouseSlotItemStack = default;
                } else {
                    OwnerPlayer.Inventory.ItemStackList[ItemSlotID] = OwnerPlayer.Inventory.MouseSlotItemStack;
                    OwnerPlayer.Inventory.MouseSlotItemStack = itemStack;
                }
            }
        }
    }
}
