using FeloxGame.Rendering;
using OpenTK.Windowing.Common;

namespace FeloxGame.GUI
{
    public class ActiveHotbarSlotUI : UI
    {
        // fields
        private int activeIndex;
        
        // properties
        protected int ActiveIndex
        {
            get
            {
                return activeIndex;
            }
            set
            {
                if (value > MaxIndex)
                {
                    value = MinIndex;
                }
                else if (value < MinIndex)
                {
                    value = MaxIndex;
                }
                activeIndex = value;
                SetRPCs();
             }
        }
        protected int MinIndex { get; set; }
        protected int MaxIndex { get; set; }
        private RPC BasePosition { get; set; }

        public ActiveHotbarSlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            float x, float y, float textureWidth, float textureHeight,
            RPC basePosition, int minIndex, int maxIndex, int activeIndex
        )
            : base(koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            this.BasePosition = basePosition;
            this.MinIndex = minIndex;
            this.MaxIndex = maxIndex;
            this.KoPosition = new(); // Todo: Why isn't this done in base class UI.cs? Currently has to happen before activeindex is set

            this.ActiveIndex = activeIndex;
            base.SetTextureCoords(x, y, textureWidth, textureHeight);
        }

        protected void SetRPCs()
        {
            this.KoPosition.MinX = this.BasePosition.MinX + ActiveIndex * KoWidth;
            this.KoPosition.MaxX = this.KoPosition.MinX + KoWidth;
            this.KoPosition.MinY = this.BasePosition.MinY;
            this.KoPosition.MaxY = this.BasePosition.MaxY;
        }

        public override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.OffsetY > 0)
            {
                ActiveIndex--;
            }
            else if (e.OffsetY < 0)
            {
                ActiveIndex++;
            }
        }
    }
}
