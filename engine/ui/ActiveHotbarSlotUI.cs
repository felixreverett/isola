using Isola.Drawing;
using Isola.Utilities;
using OpenTK.Windowing.Common;

namespace Isola.ui {
    public class ActiveHotbarSlotUI : UI {
        private int activeIndex;
        public int ActiveIndex {
            get => activeIndex;
            set {
                if (value > MaxIndex) value = MinIndex;
                else if (value < MinIndex) value = MaxIndex;
                activeIndex = value;
                UpdatePosition();
             }
        }
        protected int MinIndex { get; set; }
        protected int MaxIndex { get; set; }
        private float BaseX { get; set; }
        private float BaseY { get; set; }
        protected PrecisionTextureAtlasManager AtlasManager;
        
        public ActiveHotbarSlotUI (
            float width, float height, eAnchor anchor, float scale, AssetLibrary assets, bool isDrawable, bool toggleDraw, bool isClickable,
            float textureX, float textureY, float textureWidth, float textureHeight,
            float baseX, float baseY, int minIndex, int maxIndex, int activeIndex, string atlasName = "Inventory Atlas"
        ) : base(width, height, anchor, scale, assets, isDrawable, toggleDraw, isClickable) {
            BaseX = baseX;
            BaseY = baseY;
            MinIndex = minIndex;
            MaxIndex = maxIndex;
            ActiveIndex = activeIndex;

            AtlasManager = (PrecisionTextureAtlasManager)_assets.TextureAtlasManagerList[atlasName];
            BatchRenderer = _assets.BatchRendererList[atlasName];
            SetTextureCoords(textureX, textureY, textureWidth, textureHeight);
        }

        protected void UpdatePosition() {
            float oldLocalX = LocalRect.X;
            float newLocalX = BaseX + (ActiveIndex * Width);
            float deltaX = newLocalX - oldLocalX;

            LocalRect.X = newLocalX;
            LocalRect.Y = BaseY;

            AbsoluteRect.X += deltaX;

            float vw = 640f;

            NDCs.MinX = (AbsoluteRect.X / vw) * 2f - 1f;
            NDCs.MaxX = (AbsoluteRect.Right / vw) * 2f - 1f;
        }

        public void SetTextureCoords(float x, float y, float textureWidth, float textureHeight) {
            TexCoords = AtlasManager.GetPrecisionAtlasCoords(x, y, textureWidth, textureHeight);
        }

        public override void OnMouseWheel(MouseWheelEventArgs e) {
            if (e.OffsetY > 0) ActiveIndex--;
            else if (e.OffsetY < 0) ActiveIndex++;
        }
    }
}
