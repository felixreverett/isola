using Isola.Drawing;
using Isola.engine.graphics.text;
using Isola.ui;
using Isola.Utilities;
using OpenTK.Mathematics;

namespace Isola.game.GUI
{
    public class TextUI : UI
    {
        public string Text { get; set; }
        public FontAtlasManager FontAtlasManager { get; set; }
        public bool IsRelativeSize { get; set; }
        public int FontSize { get; set; }
        public float NativeHeight { get; set; } = 360f;
        public float NativeWidth { get; set; } = 640f;

        public TextUI
        (
            float width, float height, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            string text, int fontSize = 1, bool isRelativeSize = false, string atlasName = "Font Atlas"
        ) : base
        (
            width, height, anchor, scale, isDrawable, toggleDraw, isClickable
        )
        {
            Text = text;
            IsRelativeSize = isRelativeSize;
            FontSize = fontSize;
            FontAtlasManager = (FontAtlasManager)AssetLibrary.TextureAtlasManagerList[atlasName];
            BatchRenderer = AssetLibrary.BatchRendererList[atlasName];
            
        }
        public override void Draw()
        {
            if (string.IsNullOrEmpty(Text))
            {
                return;
            }

            if (BatchRenderer == null)
            {
                Console.WriteLine("[W] Attempted to Draw() TextUI element while batchrenderer was null.");
                return;
            }

            if (ToggleDraw)
            {
                BatchRenderer.StartBatch();

                float currentX = 0f;
                float currentY = 0f;

                float fontScale = (float)FontSize / FontAtlasManager.LineHeight;
                float baseLine = FontAtlasManager.Base * fontScale;

                for (int i = 0; i < Text.Length; i++)
                {
                    char c = Text[i];
                    if (FontAtlasManager.Characters.TryGetValue(c, out CharData charData))
                    {
                        float charWidth = charData.Width * fontScale;
                        float charHeight = charData.Height * fontScale;
                        float charX = currentX + charData.XOffset * fontScale;
                        float charY = currentY + (charData.YOffset + FontAtlasManager.Base) * fontScale;

                        float ndcMinX = NDCs.MinX + charX / (IsRelativeSize ? Width : NativeWidth) * (NDCs.MaxX - NDCs.MinX);
                        float ndcMaxX = ndcMinX + charWidth / (IsRelativeSize ? Width : NativeWidth) * (NDCs.MaxX - NDCs.MinX);

                        float ndcTopY = NDCs.MaxY - (charY / (IsRelativeSize ? Height : NativeHeight)) * (NDCs.MaxY - NDCs.MinY);
                        float ndcBottomY = ndcTopY - (charHeight / (IsRelativeSize ? Height : NativeHeight)) * (NDCs.MaxY - NDCs.MinY);

                        Box2 rect = new Box2(ndcMinX, ndcBottomY, ndcMaxX, ndcTopY);
                        TexCoords texCoords = FontAtlasManager.GetIndexedAtlasCoords(c);

                        //Console.WriteLine($"Attempting to draw at NDCs: {ndcMinX}, {ndcBottomY}, {ndcMaxX}, {ndcTopY}\nAt texCoords: x{texCoords.MinX}, y{texCoords.MinY}, x{texCoords.MaxX}, y{texCoords.MaxY}");

                        BatchRenderer.AddQuadToBatch(rect, texCoords);
                        currentX += charData.XAdvance * fontScale;
                    }
                }

                BatchRenderer.EndBatch();
            }

            if (Children.Count != 0 && ToggleDraw)
            {
                foreach (UI ui in Children.Values)
                {
                    ui.Draw();
                }
            }
        }
    }
}
