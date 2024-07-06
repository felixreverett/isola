using FeloxGame.Rendering;
using FeloxGame.WorldClasses;
using OpenTK.Mathematics;

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

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {
            if (IsMouseInBounds(mousePosition))
            {
                base.Inventory.OnItemSlotLeftClick(ItemSlotID);
            }
        }
    }
}
