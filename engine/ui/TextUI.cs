using Isola.Drawing;
using Isola.engine.graphics.text;
using Isola.ui;
using Isola.Utilities;
using OpenTK.Mathematics;

namespace Isola.game.GUI {

    public enum eTextColor {
        Black,
        White,
        Green,
        LightGrey,
        Red
    }

    public class TextUI : UI {
        public string Text { get; set; }
        public FontAtlasManager FontAtlasManager { get; set; }
        public int FontSize { get; set; }

        public eTextColor TextColor { get; set; }

        public TextUI(
            float width, float height, eAnchor anchor, float scale, AssetLibrary assets, bool isDrawable, bool toggleDraw, bool isClickable,
            string text, int fontSize = 12, string atlasName = "Font Atlas", eTextColor textColor = eTextColor.White
        ) : base (
            width, height, anchor, scale, assets, isDrawable, toggleDraw, isClickable
        ) {
            Text = text;
            FontSize = fontSize;
            FontAtlasManager = (FontAtlasManager)_assets.TextureAtlasManagerList[atlasName];
            BatchRenderer = _assets.BatchRendererList[atlasName];
            TextColor = textColor;
        }

        public override void Draw() {
            if (string.IsNullOrEmpty(Text) || BatchRenderer == null || !ToggleDraw) return;

            BatchRenderer.SetVector4("u_TextColor", GetColorVector(TextColor));

            float ndcWidth = NDCs.MaxX - NDCs.MinX;
            float ndcHeight = NDCs.MaxY - NDCs.MinY;

            // Prevent divide by zero if element has zero size
            if (AbsoluteRect.Width <= 0.001f || AbsoluteRect.Height <= 0.001f) return;

            BatchRenderer.StartBatch();

            float ndcPerPixelX = ndcWidth / AbsoluteRect.Width;
            float ndcPerPixelY = ndcHeight / AbsoluteRect.Height;

            float fontScale = (float)FontSize / FontAtlasManager.LineHeight;

            float cursorPixelX = 0f;

            for (int i = 0; i < Text.Length; i++) {
                char c = Text[i];
                if (FontAtlasManager.Characters.TryGetValue(c, out CharData charData)) {
                    float charPixelWidth = charData.Width * fontScale;
                    float charPixelHeight = charData.Height * fontScale;
                    float charPixelX = cursorPixelX + (charData.XOffset * fontScale);
                    float charPixelTopFromBoxTop = charData.YOffset * fontScale;

                    float ndcMinX = NDCs.MinX + (charPixelX * ndcPerPixelX);
                    float ndcMaxX = ndcMinX + (charPixelWidth * ndcPerPixelX);

                    float ndcTopY = NDCs.MaxY - (charPixelTopFromBoxTop * ndcPerPixelY);
                    float ndcBottomY = ndcTopY - (charPixelHeight * ndcPerPixelY);

                    Box2 rect = new Box2(ndcMinX, ndcBottomY, ndcMaxX, ndcTopY);
                    TexCoords texCoords = FontAtlasManager.GetIndexedAtlasCoords(c);

                    BatchRenderer.AddQuadToBatch(rect, texCoords);

                    cursorPixelX += charData.XAdvance * fontScale;
                }
            }

            BatchRenderer.EndBatch();

            if (Children.Count != 0) {
                foreach (UI ui in Children.Values) {
                    ui.Draw();
                }
            }

            return;
        }

        private Vector4 GetColorVector(eTextColor color) {
            switch (color) {
                case eTextColor.Black:      return new Vector4(0f, 0f, 0f, 1f);
                case eTextColor.Green:      return new Vector4(0f, 1f, 0f, 1f);
                case eTextColor.Red:        return new Vector4(1f, 0f, 0f, 1f);
                case eTextColor.LightGrey:  return new Vector4(0.9f, 0.9f, 0.9f, 1f);
                case eTextColor.White:
                default:                    return new Vector4(1f, 1f, 1f, 1f);
            }
        }
    }
}
