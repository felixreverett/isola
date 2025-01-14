using FeloxGame.Drawing;
using FeloxGame.Inventories;
using FeloxGame.Items;
using FeloxGame.Utilities;
using OpenTK.Mathematics;

namespace FeloxGame.GUI
{
    public class MouseSlotUI : UI
    {
        private Vector2 MouseNDCs { get; set; }
        protected Vector2 NDCDimensions;
        protected PlayerEntity OwnerPlayer;

        public MouseSlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            PlayerEntity ownerPlayer
        )
            : base(koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            OwnerPlayer = ownerPlayer;
            AtlasManager = AssetLibrary.TextureAtlasManagerList["Item Atlas"];
        }

        public void UpdateItem(ItemStack itemStack)
        {
            int textureIndex = 0;

            Item matchingItem = AssetLibrary.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);

            if (matchingItem != null)
            {
                textureIndex = matchingItem.TextureIndex;
            }

            TextureCoordinates = AtlasManager.GetIndexedAtlasCoords(textureIndex);
        }


        public override void OnMouseMove(Vector2 mouseNDCs)
        {
            MouseNDCs = mouseNDCs;
            SetNDCs();
        }

        // This one updates the NDCs when the parent UI element calls it
        public override void SetNDCs(float oyaWidth, float oyaHeight, NDC oyaNDCs)
        {
            NDCDimensions.X = (KoWidth / oyaWidth) * (oyaNDCs.MaxX - oyaNDCs.MinX);
            NDCDimensions.Y = (KoHeight / oyaHeight) * (oyaNDCs.MaxY - oyaNDCs.MinY);
        }

        // This one updates the NDCs when the mouse moves
        protected void SetNDCs()
        {
            KoNDCs.MaxX = MouseNDCs.X + NDCDimensions.X / 2.0f;
            KoNDCs.MinX = MouseNDCs.X - NDCDimensions.X / 2.0f;
            KoNDCs.MaxY = MouseNDCs.Y + NDCDimensions.Y / 2.0f;
            KoNDCs.MinY = MouseNDCs.Y - NDCDimensions.Y / 2.0f;
        }

    }
}
