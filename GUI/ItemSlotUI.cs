using FeloxGame.Core.Management;

namespace FeloxGame.GUI
{
    public class ItemSlotUI : UI
    {
        public ItemSlotUI(float koWidth, float koHeight, eAnchor anchor, float scale, TexCoords koPosition) 
            : base(koWidth, koHeight, anchor, scale)
        {
            this.KoPosition = koPosition;
        }
    }
}
