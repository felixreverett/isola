using Isola.Drawing;
using Isola.engine.graphics.text;
using Isola.Entities;
using Isola.game.GUI;
using Isola.Inventories;
using Isola.Utilities;
using Isola.World;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Isola.ui {
    public class PlayerInvUI : UI {
        PlayerInventory Inventory;
        PlayerEntity OwnerPlayer;
        protected PrecisionTextureAtlasManager AtlasManager;
        private TextUI _countTextHelper;

        /// <summary>
        /// Creates an InventoryUI class to manage all inventory interfacing.
        /// </summary>
        /// <param name="width">The total width of the UI element</param>
        /// <param name="height">The total height of the UI element</param>
        /// <param name="anchor">The eAnchor type</param>
        /// <param name="scale">The scale of the UI element</param>
        /// <param name="isDrawable">Whether the UI element will use a texture atlas</param>
        /// <param name="toggleDraw">Whether the UI element is currently drawn</param>
        /// <param name="isClickable">Whether the UI element subscribes to Mouse Click events</param>
        /// <param name="inventory">The associated inventory of the UI element</param>
        public PlayerInvUI (
            float width, float height, eAnchor anchor, float scale, AssetLibrary assets, bool isDrawable, bool toggleDraw, bool isClickable,
            PlayerInventory playerInventory, PlayerEntity ownerPlayer, string atlasName
        ) : base (
            width, height, anchor, scale, assets, isDrawable, toggleDraw, isClickable
        ) {
            Inventory = playerInventory;
            OwnerPlayer = ownerPlayer;
            AtlasManager = (PrecisionTextureAtlasManager)_assets.TextureAtlasManagerList[atlasName];
            BatchRenderer = _assets.BatchRendererList[atlasName];
            SetTextureCoords(0, 0, 196, 110);
            GenerateUISlots();

            _countTextHelper = new TextUI(16f, 16f, eAnchor.BottomRight, 1.0f, _assets, true, true, false, "0", 12, "Font Atlas");
        }

        public override void Draw() {
            base.Draw();

            if (ToggleDraw) {
                int totalSlots = Inventory.Rows * Inventory.Cols;

                for (int i = 0; i < totalSlots; i++) {
                    ItemStack item = Inventory.ItemStackList[i];

                    if (!item.Equals(default(ItemStack)) && item.Amount > 1) {
                        if (Children.TryGetValue(i.ToString(), out UI slotUI)) {
                            string amountText = item.Amount.ToString();
                            _countTextHelper.Text = amountText;

                            float textPixelWidth = 0f;
                            float fontScale = (float)_countTextHelper.FontSize / _countTextHelper.FontAtlasManager.LineHeight;

                            foreach (char c in amountText) {
                                if (_countTextHelper.FontAtlasManager.Characters.TryGetValue(c, out CharData charData)) {
                                    textPixelWidth += charData.XAdvance * fontScale;
                                }
                            }

                            float slotRight = slotUI.AbsoluteRect.Right;
                            float slotBottom = slotUI.AbsoluteRect.Bottom;

                            float padX = 2f;
                            float padY = 2f;
                            float verticalOffset = 9f;

                            float startX = slotRight - padX - textPixelWidth;
                            float startY = slotBottom + padY - verticalOffset;

                            _countTextHelper.AbsoluteRect = new PixelRect(startX, startY, 16f, 16f);

                            float vw = 640f;
                            float vh = 360f;

                            _countTextHelper.NDCs.MinX = (_countTextHelper.AbsoluteRect.X       / vw) * 2f - 1f;
                            _countTextHelper.NDCs.MaxX = (_countTextHelper.AbsoluteRect.Right   / vw) * 2f - 1f;
                            _countTextHelper.NDCs.MinY = (_countTextHelper.AbsoluteRect.Y       / vh) * 2f - 1f;
                            _countTextHelper.NDCs.MaxY = (_countTextHelper.AbsoluteRect.Top     / vh) * 2f - 1f;

                            _countTextHelper.Draw();
                        }
                    }
                }
            }
        }

        private void GenerateUISlots() {
            // Set dimensions of UI slots
            float itemSlotHeight = 16f;
            float itemSlotWidth = 16f;
            float edgePadding = 9f;
            float itemSlotPadding = 2f;
            
            /* Get the coordinates for each UI Slot
                10-19
                20-29
                30-39
                40-49
                =====
                0 - 9 */

            int slotIndex = 0;
            for (int row = 0; row < Inventory.Rows; row++) {
                for (int col = 0; col < Inventory.Cols; col++) {
                    float x = edgePadding + col * (itemSlotWidth + itemSlotPadding);
                    float y;

                    if (row == 0) {
                        y = edgePadding;
                    } else {
                        y = Height - edgePadding - (row * itemSlotHeight) - (row - 1) * itemSlotPadding;
                    }

                    Children.Add($"{slotIndex}", new SlotUI(
                        itemSlotWidth, itemSlotHeight, eAnchor.None, 1f, _assets, true, false, true,
                        slotIndex, Inventory, x, y, OwnerPlayer, "Item Atlas"));

                    slotIndex++;
                }
            }

            Children.Add("mouseSlot", new MouseSlotUI(itemSlotWidth, itemSlotHeight, eAnchor.None, 1f, _assets, true, false, true, OwnerPlayer, "Item Atlas"));
        }

        public void SetTextureCoords(float x, float y, float textureWidth, float textureHeight) {
            TexCoords = AtlasManager.GetPrecisionAtlasCoords(x, y, textureWidth, textureHeight);
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world) {
            if (!IsMouseInBounds(mousePosition)) {
                OnExternalClick();
                return;
            }

            if (Children.Count > 0) {
                foreach (UI ui in Children.Values) {
                    ui.OnLeftClick(mousePosition, world);
                }
            }

            if (IsClickable) {
                // functionality here
            }
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Keys.Q) {
                if (!Inventory.MouseSlotItemStack.Equals(default(ItemStack))) {
                    // Drop item on mouse slot
                    ItemStack itemStack = Inventory.MouseSlotItemStack;
                    Inventory.MouseSlotItemStack = default(ItemStack);
                    OwnerPlayer.CurrentWorld.AddEntityToWorld(new ItemEntity(OwnerPlayer.Position, itemStack.ItemName, itemStack.Amount, _assets));
                } else {
                    // drop if an item is being hovered over?
                }
            }
        }

        public void OnExternalClick() {
            if (!Inventory.MouseSlotItemStack.Equals(default(ItemStack))) {
                ItemStack itemStack = Inventory.MouseSlotItemStack;
                Inventory.MouseSlotItemStack = default(ItemStack);
                OwnerPlayer.CurrentWorld.AddEntityToWorld(new ItemEntity(OwnerPlayer.Position, itemStack.ItemName, itemStack.Amount, _assets));
            }
        }
    }
}
