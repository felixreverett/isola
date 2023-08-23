using FeloxGame.Core.Management;

namespace FeloxGame.GUI
{
    public class ItemSlotUI : UI
    {
        public ItemSlotUI(float koWidth, float koHeight, eAnchor anchor, float scale, bool drawable, bool toggleDraw, TexCoords koPosition) 
            : base(koWidth, koHeight, anchor, scale, drawable, toggleDraw)
        {
            this.KoPosition = koPosition;
            
            SetTextureCoords(4, 840, 346, 180, 1024, 1024);
        }

        /// <summary>
        /// Updates information on the item contained within 
        /// </summary>
        public void UpdateItem(/*feed it the item info*/)
        {
            // if (no item anymore) { ToggleDraw = false; }
            // else
            //SetTextureCoords();
        }

    }
}
