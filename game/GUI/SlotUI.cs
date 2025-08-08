using Isola.Utilities;
using Isola.Drawing;
using Isola.Inventories;
using Isola.Items;
using OpenTK.Mathematics;
using Isola.World;

namespace Isola.GUI
{
    public class SlotUI : UI
    {
        protected int ItemSlotID;
        protected Inventory Inventory;
        protected PlayerEntity OwnerPlayer;

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
            AtlasManager = AssetLibrary.TextureAtlasManagerList["Item Atlas"];
        }

        public override void Update()
        {
            ItemStack itemStack = OwnerPlayer.Inventory.ItemStackList[ItemSlotID];

            if (itemStack.Equals(default(ItemStack)))
            {
                ToggleDraw = false;
            }
            else
            {
                int textureIndex = 0;
                Item matchingItem = AssetLibrary.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);
                if (matchingItem != null)
                {
                    textureIndex = matchingItem.TextureIndex;
                    TextureCoordinates = AtlasManager.GetIndexedAtlasCoords(textureIndex);
                }
                else
                {
                    Console.WriteLine($"Error: No matching item found in library with name {itemStack.ItemName}. Using default texture 0.");
                }
                ToggleDraw = true;
            }
            base.Update();
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {
            // Swap slots
            if (IsMouseInBounds(mousePosition))
            {
                ItemStack itemStack = OwnerPlayer.Inventory.ItemStackList[ItemSlotID];

                if (itemStack.ItemName == OwnerPlayer.Inventory.MouseSlotItemStack.ItemName)
                {
                    OwnerPlayer.Inventory.ItemStackList[ItemSlotID].Amount += OwnerPlayer.Inventory.MouseSlotItemStack.Amount;
                    OwnerPlayer.Inventory.MouseSlotItemStack = default(ItemStack);
                }
                else
                {
                    OwnerPlayer.Inventory.ItemStackList[ItemSlotID] = OwnerPlayer.Inventory.MouseSlotItemStack;
                    OwnerPlayer.Inventory.MouseSlotItemStack = itemStack;
                }
            }
        }
    }
}
