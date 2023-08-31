using FeloxGame.Core.Management;
using FeloxGame.InventoryClasses;

namespace FeloxGame.GUI
{
    public class InventoryUI : UI
    {
        // ItemSlots
        private int _rows;
        private int _cols;
        private float _itemSlotHeight;
        private float _itemSlotWidth;

        // Hard coded for now
        private float _edgePadding = 4f;
        private float _hotbarPadding = 4f;
        private float _itemSlotPadding = 2f;
        Inventory Inventory;

        public InventoryUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool drawable, bool toggleDraw, bool isClickable,
            int rows, int cols, float itemSlotHeight, float itemSlotWidth, Inventory inventory
        )
            : base(koWidth, koHeight, anchor, scale, drawable, toggleDraw, isClickable)
        {
            this._rows = rows;
            this._cols = cols;
            this._itemSlotHeight = itemSlotHeight;
            this._itemSlotWidth = itemSlotWidth;
            this.Inventory = inventory;
            GenerateKodomo();
        }

        public void GenerateKodomo()
        {
            // Set the width and height of a UI Slot
            float availableWidth = this.KoWidth - 2 * _edgePadding;
            float availableHeight = this.KoHeight - 2 * _edgePadding - _hotbarPadding;

            /// Get the coordinates for each UI Slot
            /// 10-19
            /// 20-29
            /// 30-39
            /// 40-49
            /// =====
            /// 0 - 9
            int slotIndex = 0; 

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    TexCoords koPosition = new();

                    koPosition.MinX = _edgePadding + col * (_itemSlotWidth + _itemSlotPadding);

                    if (row == 0)
                    {
                        koPosition.MinY = _edgePadding;
                    }
                    else
                    {
                        koPosition.MinY = KoHeight - _edgePadding - (row *  _itemSlotHeight) - (row - 1) * _itemSlotPadding;
                    }

                    koPosition.MaxX = koPosition.MinX + _itemSlotWidth;
                    koPosition.MaxY = koPosition.MinY + _itemSlotHeight;

                    Kodomo.Add($"{slotIndex}", new ItemSlotUI(_itemSlotWidth, _itemSlotHeight, eAnchor.None, 1f, true, false, true, koPosition, slotIndex, Inventory));

                    slotIndex++;
                }
            }
        }

        public void SubscribeToInventory(Inventory inventory)
        {
            inventory.InventoryChanged += HandleInventoryChanged;
        }

        private void HandleInventoryChanged(ItemStack[] itemStackList)
        {
            for (int i = 0; i < _rows * _cols; i++)
            {
                if (itemStackList[i] != null)
                {
                    Kodomo[$"{i}"].ToggleDraw = true;
                    ((ItemSlotUI)Kodomo[$"{i}"]).UpdateItem(itemStackList[i]);
                }
                else
                {
                    Kodomo[$"{i}"].ToggleDraw = false;
                }
            }
        }

        public override void OnMouseMove()
        {
            base.OnMouseMove();
            {

            }
        }
    }
}
