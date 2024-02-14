using FeloxGame.Rendering;

namespace FeloxGame.GUI
{
    public class ActiveHotbarSlotUI : UI
    {
        // fields
        private int _activeIndex;

        public ActiveHotbarSlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            float x, float y, float textureWidth, float textureHeight,
            RPC koPosition, int activeIndex
        )
            : base(koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            this.KoPosition = koPosition;
            this._activeIndex = activeIndex;
            base.SetTextureCoords(x, y, textureWidth, textureHeight);
        }

        public void SetRPCs()
        {
            // use math to update RPCs (relative to parent class)
            // set RPCs based on a min point multiplied by the slotIndex
        }
    }
}
