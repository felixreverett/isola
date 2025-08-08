using Isola.Drawing;
using Isola.Utilities;
using OpenTK.Windowing.Common;

namespace Isola.GUI
{
    public class ActiveHotbarSlotUI : UI
    {
        // fields
        private int activeIndex;
        
        // properties
        public int ActiveIndex
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
        protected PrecisionTextureAtlasManager AtlasManager;
        

        public ActiveHotbarSlotUI
        (
            float width, float height, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            float x, float y, float textureWidth, float textureHeight,
            RPC basePosition, int minIndex, int maxIndex, int activeIndex, string atlasName = "Inventory Atlas"
        )
            : base(width, height, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            BasePosition = basePosition;
            MinIndex = minIndex;
            MaxIndex = maxIndex;
            ActiveIndex = activeIndex;
            AtlasManager = (PrecisionTextureAtlasManager)AssetLibrary.TextureAtlasManagerList[atlasName];
            BatchRenderer = AssetLibrary.BatchRendererList[atlasName];
            SetTextureCoords(x, y, textureWidth, textureHeight);
        }

        protected void SetRPCs()
        {
            Position.MinX = BasePosition.MinX + ActiveIndex * Width;
            Position.MaxX = Position.MinX + Width;
            Position.MinY = BasePosition.MinY;
            Position.MaxY = BasePosition.MaxY;
        }

        public void SetTextureCoords(float x, float y, float textureWidth, float textureHeight)
        {
            TexCoords = AtlasManager.GetPrecisionAtlasCoords(x, y, textureWidth, textureHeight);
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
