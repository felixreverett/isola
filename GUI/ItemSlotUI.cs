using FeloxGame.Core.Management;
using FeloxGame.Core.Rendering;

namespace FeloxGame.GUI
{
    public class ItemSlotUI : SlotUI
    {
        public ItemSlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            int itemSlotID, Inventory inventory, RPC koPosition
        ) 
            : base(koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable,
                  itemSlotID, inventory, koPosition)
        {
            
        }

        public override void OnClick()
        {
            base.Inventory.OnItemSlotClick(ItemSlotID);
        }

    }
}
