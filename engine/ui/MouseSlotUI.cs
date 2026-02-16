using Isola.Drawing;
using Isola.game.GUI;
using Isola.Inventories;
using Isola.Items;
using Isola.Utilities;
using OpenTK.Mathematics;

namespace Isola.ui {
    public class MouseSlotUI : UI {
        private Vector2 _currentMousePixels;
        protected PlayerEntity OwnerPlayer;
        protected IndexedTextureAtlasManager AtlasManager;
        private TextUI _countTextHelper;

        public MouseSlotUI (
            float width, float height, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            PlayerEntity ownerPlayer, string atlasName
        ) : base(width, height, anchor, scale, isDrawable, toggleDraw, isClickable) {
            OwnerPlayer = ownerPlayer;
            AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList[atlasName];
            BatchRenderer = AssetLibrary.BatchRendererList[atlasName];
            _countTextHelper = new TextUI(16f, 16f, eAnchor.BottomRight, 1.0f, true, true, false, "0", 12, "Font Atlas");
        }

        public override void Update()
        {
            ItemStack itemStack = OwnerPlayer.Inventory.MouseSlotItemStack;

            if (itemStack.Equals(default(ItemStack))) ToggleDraw = false;
            else {
                int textureIndex = 0;
                Item matchingItem = AssetLibrary.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);
                if (matchingItem != null) {
                    textureIndex = matchingItem.TextureIndex;
                    TexCoords = AtlasManager.GetIndexedAtlasCoords(textureIndex);
                } else {
                    Console.WriteLine($"Error: No matching item found in library with name {itemStack.ItemName}. Using default texture 0.");
                }
                ToggleDraw = true;
            }
            base.Update();
        }

        public override void OnMouseMove(Vector2 mousePixels)
        {
            _currentMousePixels = mousePixels;
        }

        public override void Draw() {
            if (ToggleDraw && IsDrawable && BatchRenderer != null) {
                float w = Width * Scale;
                float h = Height * Scale;

                AbsoluteRect.X = _currentMousePixels.X - (w / 2f);
                AbsoluteRect.Y = _currentMousePixels.Y - (h / 2f);
                AbsoluteRect.Width = w;
                AbsoluteRect.Height = h;

                float vw = 640f;
                float vh = 360f;

                NDCs.MinX = (AbsoluteRect.X     / vw) * 2f - 1f;
                NDCs.MaxX = (AbsoluteRect.Right / vw) * 2f - 1f;
                NDCs.MinY = (AbsoluteRect.Y     / vh) * 2f - 1f;
                NDCs.MaxY = (AbsoluteRect.Top   / vh) * 2f - 1f;

                Box2 rect = new Box2(NDCs.MinX, NDCs.MinY, NDCs.MaxX, NDCs.MaxY);
                BatchRenderer.StartBatch();
                BatchRenderer.AddQuadToBatch(rect, TexCoords);
                BatchRenderer.EndBatch();

                ItemStack itemStack = OwnerPlayer.Inventory.MouseSlotItemStack;
                if (itemStack.Amount > 1) {
                    _countTextHelper.Text = itemStack.Amount.ToString();

                    float pad = 2f;
                    float vertOffset = 9f;

                    float startX = AbsoluteRect.Right - pad - 8f;
                    float startY = AbsoluteRect.Bottom + pad - vertOffset;

                    _countTextHelper.AbsoluteRect = new PixelRect(startX, startY, 16f, 16f);

                    _countTextHelper.NDCs.MinX = (_countTextHelper.AbsoluteRect.X / vw) * 2f - 1f;
                    _countTextHelper.NDCs.MaxX = (_countTextHelper.AbsoluteRect.Right / vw) * 2f - 1f;
                    _countTextHelper.NDCs.MinY = (_countTextHelper.AbsoluteRect.Y / vh) * 2f - 1f;
                    _countTextHelper.NDCs.MaxY = (_countTextHelper.AbsoluteRect.Top / vh) * 2f - 1f;

                    _countTextHelper.Draw();
                }
            }
        }
    }
}
