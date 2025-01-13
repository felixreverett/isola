using FeloxGame.Drawing;
using FeloxGame.Inventories;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using FeloxGame.World;
using FeloxGame.Entities;

namespace FeloxGame.GUI
{
    public class PlayerInvUI : UI
    {
        /// <summary>
        /// Creates an InventoryUI class to manage all inventory interfacing.
        /// </summary>
        /// <param name="koWidth">The total width of the UI element</param>
        /// <param name="koHeight">The total height of the UI element</param>
        /// <param name="anchor">The eAnchor type</param>
        /// <param name="scale">The scale of the UI element</param>
        /// <param name="isDrawable">Whether the UI element will use a texture atlas</param>
        /// <param name="toggleDraw">Whether the UI element is currently drawn</param>
        /// <param name="isClickable">Whether the UI element subscribes to Mouse Click events</param>
        /// <param name="inventory">The associated inventory of the UI element</param>
        public PlayerInvUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            PlayerInventory playerInventory, HotbarUI hotbarUI, PlayerEntity ownerPlayer
        ) : base
        (
            koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable
        )
        {
            this.Inventory = playerInventory;
            this.HotbarUI = hotbarUI;
            this.OwnerPlayer = ownerPlayer;
            GenerateUISlots();
        }

        // Fields
        PlayerInventory Inventory;
        HotbarUI HotbarUI;
        PlayerEntity OwnerPlayer;

        private void GenerateUISlots()
        {
            // Set dimensions of UI slots
            float itemSlotHeight = 16f;
            float itemSlotWidth = 16f;
            float edgePadding = 9f;
            float hotbarPadding = 6f;
            float itemSlotPadding = 2f;
            
            float availableWidth = KoWidth - 2 * edgePadding;
            float availableHeight = KoHeight - 2 * edgePadding - hotbarPadding;

            /* Get the coordinates for each UI Slot
                10-19
                20-29
                30-39
                40-49
                =====
                0 - 9 */

            int slotIndex = 0;
            for (int row = 0; row < Inventory.Rows; row++)
            {
                for (int col = 0; col < Inventory.Cols; col++)
                {
                    RPC koPosition = new();

                    koPosition.MinX = edgePadding + col * (itemSlotWidth + itemSlotPadding);

                    if (row == 0)
                    {
                        koPosition.MinY = edgePadding;
                    }
                    else
                    {
                        koPosition.MinY = KoHeight - edgePadding - (row * itemSlotHeight) - (row - 1) * itemSlotPadding;
                    }

                    koPosition.MaxX = koPosition.MinX + itemSlotWidth;
                    koPosition.MaxY = koPosition.MinY + itemSlotHeight;

                    Kodomo.Add($"{slotIndex}", new SlotUI(itemSlotWidth, itemSlotHeight, eAnchor.None, 1f, true, false, true, slotIndex, Inventory, koPosition, OwnerPlayer));

                    slotIndex++;
                }
            }

            Kodomo.Add("mouseSlot", new MouseSlotUI(itemSlotWidth, itemSlotHeight, eAnchor.None, 1f, true, false, true, OwnerPlayer));
        }

        private void HandleInventoryChanged()
        {
            for (int i = 0; i < Inventory.Rows * Inventory.Cols; i++)
            {
                if (!Inventory.ItemStackList[i].Equals(default(ItemStack)))
                {
                    Kodomo[$"{i}"].ToggleDraw = true;
                    ((SlotUI)Kodomo[$"{i}"]).UpdateItem(Inventory.ItemStackList[i]);
                }
                else
                {
                    Kodomo[$"{i}"].ToggleDraw = false;
                }
            }

            if (!Inventory.MouseSlotItemStack.Equals(default(ItemStack)))
            {
                Kodomo["mouseSlot"].ToggleDraw = true;
                ((MouseSlotUI)Kodomo["mouseSlot"]).UpdateItem(Inventory.MouseSlotItemStack);
            }
            
            else
            {
                Kodomo["mouseSlot"].ToggleDraw = false;
            }
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {
            if (!IsMouseInBounds(mousePosition))
            {
                OnExternalClick();
                return;
            }

            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.OnLeftClick(mousePosition, world);
                }
            }

            if (this.IsClickable)
            {
                // functionality here
            }

            HandleInventoryChanged();
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Keys.Q)
            {
                if (!Inventory.MouseSlotItemStack.Equals(default(ItemStack)))
                {
                    // Drop item on mouse slot
                    ItemStack itemStack = Inventory.MouseSlotItemStack;
                    Inventory.MouseSlotItemStack = default(ItemStack);
                    OwnerPlayer.CurrentWorld.AddEntityToWorld(new ItemEntity(eEntityType.ItemEntity, OwnerPlayer.Position, itemStack.ItemName, itemStack.Amount));
                }
                else
                {
                    // drop if an item is being hovered over?
                }
            }

            HandleInventoryChanged();
        }

        public void OnExternalClick()
        {
            if (!Inventory.MouseSlotItemStack.Equals(default(ItemStack)))
            {
                ItemStack itemStack = Inventory.MouseSlotItemStack;
                Inventory.MouseSlotItemStack = default(ItemStack);
                OwnerPlayer.CurrentWorld.AddEntityToWorld(new ItemEntity(eEntityType.ItemEntity, OwnerPlayer.Position, itemStack.ItemName, itemStack.Amount));
            }

            HandleInventoryChanged();
        }
    }
}
