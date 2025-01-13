using FeloxGame.Utilities;
using FeloxGame.Drawing;
using FeloxGame.Inventories;
using FeloxGame.Items;
using OpenTK.Mathematics;
using FeloxGame.World;

namespace FeloxGame.GUI
{
    public class SlotUI : UI
    {
        protected int ItemSlotID;
        protected Inventory Inventory;
        protected PlayerEntity OwnerPlayer;
        protected new IndexedTextureAtlasManager AtlasManager;

        public SlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            int itemSlotID, Inventory inventory, RPC koPosition, PlayerEntity ownerPlayer
        )
            : base(koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            ItemSlotID = itemSlotID;
            Inventory = inventory;
            OwnerPlayer = ownerPlayer;
            KoPosition = koPosition;
            AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Item Atlas"];
        }

        /// <summary>
        /// Updates information about the item at this item slot. 
        /// </summary>
        public void UpdateItem(ItemStack itemStack)
        {
            int textureIndex = 0;

            Item matchingItem = AssetLibrary.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);

            if (matchingItem != null)
            {
                textureIndex = matchingItem.TextureIndex;
            }

            TextureCoordinates = AtlasManager.GetIndexedAtlasCoords(textureIndex);
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {
            // Swap slots
            if (IsMouseInBounds(mousePosition))
            {
                ItemStack itemStack = OwnerPlayer.Inventory.ItemStackList[ItemSlotID];
                OwnerPlayer.Inventory.ItemStackList[ItemSlotID] = OwnerPlayer.Inventory.MouseSlotItemStack;
                OwnerPlayer.Inventory.MouseSlotItemStack = itemStack;
            }
        }
    }
}
