using Isola.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Isola.World;
using Isola.engine.graphics;

namespace Isola.ui {
    /// <summary>
    /// Represents a rectangle in absolute screen pixels,
    /// with origin (x, y) at bottom left to match OpenGL.
    /// </summary>
    public struct PixelRect {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public float Left => X;
        public float Right => X + Width;
        public float Bottom => Y;
        public float Top => Y + Height;

        public PixelRect(float x, float y, float w, float h) {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public override string ToString() => $"PixelRect(X: {X}, Y: {Y}, Width: {Width}, Height: {Height})";
    }

    public abstract class UI : IDrawable {
        public eAnchor Anchor { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        protected float Scale { get; set; }
        /// <summary>
        /// Stores the local offset and size in pixels.
        /// If eAnchor is None, X/Y are relative to parent's Bottom-Left.
        /// </summary>
        public PixelRect LocalRect;
        /// <summary>
        /// Absolute Rectangle on the virtual screen.
        /// </summary>
        public PixelRect AbsoluteRect;
        public NDC NDCs { get; set; }
        protected TexCoords ?TexCoords { get; set; }
        public Dictionary<string, UI> Children { get; set; }
        public virtual BatchRenderer ?BatchRenderer { get; set; }
        protected bool IsDrawable { get; set; }
        public bool ToggleDraw { get; set; }
        protected bool IsClickable { get; set; }

        public UI(float width, float height, eAnchor anchor, float scale, bool isDrawable = false, bool toggleDraw = true, bool isClickable = false) {
            Width = width;
            Height = height;
            Anchor = anchor;
            Scale = Math.Clamp(scale, 0.0f, 10.0f);
            LocalRect = new PixelRect(0, 0, width * Scale, height * Scale);
            AbsoluteRect = new PixelRect(0, 0, width * Scale, height * Scale);
            NDCs = new NDC();
            Children = new Dictionary<string, UI>();
            IsDrawable = isDrawable;
            ToggleDraw = toggleDraw;
            IsClickable = isClickable;
        }

        public virtual void Draw() {
            if (BatchRenderer == null) return;

            if (IsDrawable && ToggleDraw) {
                Box2 rect = new Box2(NDCs.MinX, NDCs.MinY, NDCs.MaxX, NDCs.MaxY);
                BatchRenderer.StartBatch();
                BatchRenderer.AddQuadToBatch(rect, TexCoords);
                BatchRenderer.EndBatch();
            }
            
            if (Children.Count != 0 && ToggleDraw) {
                foreach (UI ui in Children.Values) ui.Draw();
            }
        }

        public virtual void Update() {
            foreach (UI ui in Children.Values) ui.Update();
        }

        /// <summary>
        /// Called when window resizes or on refresh.
        /// Passes virtual dimensions not window dimensions.
        /// </summary>
        /// <param name="oyaWidth"></param>
        /// <param name="oyaHeight"></param>
        /// <param name="oyaNDCs"></param>
        public void OnResize(float virtualW, float virtualH) {
            PixelRect screenRect = new PixelRect(0, 0, virtualW, virtualH);
            SetNDCs(screenRect, virtualW, virtualH);
        }

        public virtual void OnMouseMove(Vector2 mouseNDCs) {
            foreach (UI ui in Children.Values) ui.OnMouseMove(mouseNDCs);
        }

        public virtual void OnLeftClick(Vector2 mousePixels, WorldManager world) {
            if (IsMouseInBounds(mousePixels)) {
                foreach (UI ui in Children.Values) ui.OnLeftClick(mousePixels, world);
                
                if (this.IsClickable) {
                    // functionality here
                }
            }
        }

        public virtual void OnRightClick(Vector2 mousePixels, WorldManager world) {
            if (IsMouseInBounds(mousePixels)) {
                foreach (UI ui in Children.Values) ui.OnRightClick(mousePixels, world);
                
                if (this.IsClickable) {
                    // functionality here
                }
            }
        }

        public virtual void OnMouseWheel(MouseWheelEventArgs e) {
            foreach (UI ui in Children.Values) ui.OnMouseWheel(e);
        }

        public virtual void OnKeyDown(KeyboardKeyEventArgs e) {
            foreach (UI ui in Children.Values) ui.OnKeyDown(e);
        }

        public bool IsMouseInBounds(Vector2 mousePixels) {
            return  mousePixels.X >= AbsoluteRect.Left &&
                    mousePixels.X <= AbsoluteRect.Right &&
                    mousePixels.Y >= AbsoluteRect.Bottom &&
                    mousePixels.Y <= AbsoluteRect.Top;
        }

        /// <summary>
        /// Calculates NDCs based on absolute pixel coordinates, which are either anchored or relative to the parent container.
        /// </summary>
        /// <param name="parentRect">The absolute pixel rectangle of the parent element.</param>
        /// <param name="virtualScreenW">The total virtual width of the game screen.</param>
        /// <param name="virtualScreenH">The total virtual height of the game screen.</param>
        public virtual void SetNDCs(PixelRect parentRect, float virtualScreenW, float virtualScreenH) {
            if (Anchor != eAnchor.None) {
                LocalRect = GetAnchoredRect(parentRect.Width, parentRect.Height);
            }

            AbsoluteRect.X = parentRect.X + LocalRect.X;
            AbsoluteRect.Y = parentRect.Y + LocalRect.Y;
            AbsoluteRect.Width = LocalRect.Width;
            AbsoluteRect.Height = LocalRect.Height;

            // Mapping virtual pixels to NDCs
            float ndcMinX = (AbsoluteRect.Left      / virtualScreenW) * 2f - 1f;
            float ndcMaxX = (AbsoluteRect.Right     / virtualScreenW) * 2f - 1f;
            float ndcMinY = (AbsoluteRect.Bottom    / virtualScreenH) * 2f - 1f;
            float ndcMaxY = (AbsoluteRect.Top       / virtualScreenH) * 2f - 1f;

            NDCs.MinX = ndcMinX;
            NDCs.MaxX = ndcMaxX;
            NDCs.MinY = ndcMinY;
            NDCs.MaxY = ndcMaxY;

            if (Children.Count > 0) {
                foreach (UI ui in Children.Values) {
                    ui.SetNDCs(AbsoluteRect, virtualScreenW, virtualScreenH);
                }
            }
        }

        /// <summary>
        /// Returns the local position relative to the parent's Bottom-Left corner.
        /// </summary>
        protected PixelRect GetAnchoredRect(float parentW, float parentH) {
            float effectiveW = Width * Scale;
            float effectiveH = Height * Scale;
            float x = 0;
            float y = 0;

            switch (Anchor) {
                case eAnchor.BottomLeft:
                case eAnchor.None:
                    x = 0;
                    y = 0;
                    break;
                case eAnchor.Bottom:
                    x = (parentW - effectiveW) / 2f;
                    y = 0;
                    break;
                case eAnchor.BottomRight:
                    x = parentW - effectiveW;
                    y = 0;
                    break;
                case eAnchor.Left:
                    x = 0;
                    y = (parentH - effectiveH) / 2f;
                    break;
                case eAnchor.Middle:
                    x = (parentW - effectiveW) / 2f;
                    y = (parentH - effectiveH) / 2f;
                    break;
                case eAnchor.Right:
                    x = parentW - effectiveW;
                    y = (parentH - effectiveH) / 2f;
                    break;
                case eAnchor.TopLeft:
                    x = 0;
                    y = parentH - effectiveH;
                    break;
                case eAnchor.Top:
                    x = (parentW - effectiveW) / 2f;
                    y = parentH - effectiveH;
                    break;
                case eAnchor.TopRight:
                    x = parentW - effectiveW;
                    y = parentH - effectiveH;
                    break;
            }

            return new PixelRect(x, y, effectiveW, effectiveH);
        }
    }
}
