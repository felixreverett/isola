using FeloxGame.Core.Management;

namespace FeloxGame.GUI
{
    public class ItemSlotUI : SlotUI
    {
        public ItemSlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool drawable, bool toggleDraw, bool isClickable,
            int itemSlotID, Inventory inventory, TexCoords koPosition
        ) 
            : base(koWidth, koHeight, anchor, scale, drawable, toggleDraw, isClickable,
                  itemSlotID, inventory, koPosition)
        {
            
        }

        public override void OnClick()
        {
            base.Inventory.OnItemSlotClick(ItemSlotID);
        }

    }
}
