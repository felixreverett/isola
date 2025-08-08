using Isola.Utilities;
using Isola.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Isola.World;
using Isola.engine.graphics;

namespace Isola.ui
{
    public class UI : IDrawable
    {
        // Position
        public eAnchor Anchor { get; set; }
        protected float Width { get; set; }
        protected float Height { get; set; }
        protected float AspectRatio { get; set; }
        protected float Scale { get; set; }
        protected RPC Position { get; set; }
        protected NDC NDCs { get; set; }
        protected TexCoords TexCoords { get; set; }

        // Kodomo
        public Dictionary<string, UI> Children { get; set; }

        // Rendering
        public virtual BatchRenderer BatchRenderer { get; set; }

        protected bool IsDrawable { get; set; }
        public bool ToggleDraw { get; set; }

        protected bool IsClickable { get; set; }

        // Constructor
        public UI(float width, float height, eAnchor anchor, float scale, bool isDrawable = false, bool toggleDraw = true, bool isClickable = false)
        {
            this.Width = width;
            this.Height = height;
            this.AspectRatio = width / height;
            this.Anchor = anchor;
            this.Scale = Math.Clamp(scale, 0.0f, 1.0f);
            this.Position = new();
            this.NDCs = new();
            this.Children = new Dictionary<string, UI>();
            this.IsDrawable = isDrawable;
            this.ToggleDraw = toggleDraw;
            this.IsClickable = isClickable;

            if (IsDrawable) //todo Aug 2025: Consider removing if all inheritees define these anyway. UI.cs never renders (it's the master class for all UI children)
            {
                if (BatchRenderer is null)
                {
                    BatchRenderer = AssetLibrary.BatchRendererList["Inventory Atlas"];
                }
            }
        }

        public virtual void Draw()
        {
            if (IsDrawable && ToggleDraw)
            {
                Box2 rect = new Box2(NDCs.MinX, NDCs.MinY, NDCs.MaxX, NDCs.MaxY);
                BatchRenderer.StartBatch();
                BatchRenderer.AddQuadToBatch(rect, TexCoords);
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

        public virtual void Update()
        {
            if (Children.Count != 0)
            {
                foreach (UI ui in Children.Values)
                {
                    ui.Update();
                }
            }
        }

        public void OnResize(float oyaWidth, float oyaHeight, NDC oyaNDCs)
        {
            this.Width = oyaWidth;
            this.Height = oyaHeight;
            this.AspectRatio = oyaWidth / oyaHeight;
            SetNDCs(oyaWidth, oyaHeight, oyaNDCs);
        }

        public virtual void OnMouseMove(Vector2 mouseNDCs)
        {
            if (Children.Count > 0)
            {
                foreach (UI ui in Children.Values)
                {
                    ui.OnMouseMove(mouseNDCs);
                }
            }
        }

        public virtual void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {
            if (!IsMouseInBounds(mousePosition))
            {
                // out of bounds functionality
            }

            else
            {
                if (Children.Count > 0)
                {
                    foreach (UI ui in Children.Values)
                    {
                        ui.OnLeftClick(mousePosition, world);
                    }
                }

                if (this.IsClickable)
                {
                    // functionality here
                }
            }
        }

        public virtual void OnRightClick(Vector2 mousePosition, WorldManager world)
        {
            if (!IsMouseInBounds(mousePosition))
            {
                // out of bounds functionality
            }

            else
            {
                if (Children.Count > 0)
                {
                    foreach (UI ui in Children.Values)
                    {
                        ui.OnRightClick(mousePosition, world);
                    }
                }

                if (this.IsClickable)
                {
                    // functionality here
                }
            }
        }

        public virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (Children.Count > 0)
            {
                foreach (UI ui in Children.Values)
                {
                    ui.OnMouseWheel(e);
                }
            }
        }

        public virtual void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (Children.Count > 0)
            {
                foreach (UI ui in Children.Values)
                {
                    ui.OnKeyDown(e);
                }
            }
        }

        public bool IsMouseInBounds(Vector2 mouseNDCs)
        {
            if (mouseNDCs.X >= NDCs.MinX && mouseNDCs.Y >= NDCs.MinY && mouseNDCs.X <= NDCs.MaxX && mouseNDCs.Y <= NDCs.MaxY) { return true; }
            else { return false; }
        }

        public virtual void SetNDCs(float oyaWidth, float oyaHeight, NDC oyaNDCs)
        {
            if (Anchor != eAnchor.None)
            {
                Position = GetAnchoredDimensions(oyaWidth, oyaHeight);
            }

            // map anchored coordinates
            NDCs.MaxX = ((Position.MaxX / oyaWidth) * (oyaNDCs.MaxX - oyaNDCs.MinX) + oyaNDCs.MinX);
            NDCs.MinX = ((Position.MinX / oyaWidth) * (oyaNDCs.MaxX - oyaNDCs.MinX) + oyaNDCs.MinX);
            NDCs.MaxY = ((Position.MaxY / oyaHeight) * (oyaNDCs.MaxY - oyaNDCs.MinY) + oyaNDCs.MinY);
            NDCs.MinY = ((Position.MinY / oyaHeight) * (oyaNDCs.MaxY - oyaNDCs.MinY) + oyaNDCs.MinY);

            if (Children.Count > 0)
            {
                foreach (UI ui in Children.Values)
                {
                    ui.SetNDCs(Width, Height, NDCs);
                }
            }
        }

        /// <summary>
        /// Returns coordinates (0,0) to (1,1) relative to the parent container.
        /// </summary>
        /// <param name="OyaWidth"></param>
        /// <param name="OyaHeight"></param>
        /// <returns></returns>
        public Vector2 GetRelativeDimensions(float OyaWidth, float OyaHeight)
        {
            Vector2 relativeDimensions = new();
            float OyaAspectRatio = OyaWidth / OyaHeight;
                        
            if (OyaAspectRatio > AspectRatio) // koHeight is constraint
            {
                relativeDimensions.Y = OyaHeight * Scale;
                relativeDimensions.X = relativeDimensions.Y * AspectRatio;
            }
            else // koWidth is constraint
            {
                relativeDimensions.X = OyaWidth * Scale;
                relativeDimensions.Y = relativeDimensions.X / AspectRatio;
            }
            
            return relativeDimensions;
        }

        /// <summary>
        /// Returns coordinates (0, 0) to (1,1) anchored within the parent container.
        /// </summary>
        /// <param name="OyaWidth"></param>
        /// <param name="OyaHeight"></param>
        /// <returns></returns>
        public RPC GetAnchoredDimensions(float OyaWidth, float OyaHeight)
        {
            Vector2 relativeDimensions = GetRelativeDimensions(OyaWidth, OyaHeight);
            RPC anchoredDimensions = new();
            switch (this.Anchor)
            {
                case eAnchor.Middle:
                    anchoredDimensions.MaxX = (OyaWidth + relativeDimensions.X) / 2f;
                    anchoredDimensions.MaxY = (OyaHeight + relativeDimensions.Y) / 2f;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.Left:
                    anchoredDimensions.MaxX = relativeDimensions.X;
                    anchoredDimensions.MaxY = (OyaHeight + relativeDimensions.Y) / 2f;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.Top:
                    anchoredDimensions.MaxX = (OyaWidth + relativeDimensions.X) / 2f;
                    anchoredDimensions.MaxY = OyaHeight;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.Right:
                    anchoredDimensions.MaxX = OyaWidth;
                    anchoredDimensions.MaxY = (OyaHeight + relativeDimensions.Y) / 2f;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.Bottom:
                    anchoredDimensions.MaxX = (OyaWidth + relativeDimensions.X) / 2f;
                    anchoredDimensions.MaxY = relativeDimensions.Y;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.TopLeft:
                    anchoredDimensions.MaxX = relativeDimensions.X;
                    anchoredDimensions.MaxY = OyaHeight;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.TopRight:
                    anchoredDimensions.MaxX = OyaWidth;
                    anchoredDimensions.MaxY = OyaHeight;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.BottomRight:
                    anchoredDimensions.MaxX = OyaWidth;
                    anchoredDimensions.MaxY = relativeDimensions.Y;
                    anchoredDimensions.MinX = anchoredDimensions.MaxX - relativeDimensions.X;
                    anchoredDimensions.MinY = anchoredDimensions.MaxY - relativeDimensions.Y;
                    break;
                case eAnchor.BottomLeft:
                    anchoredDimensions.MaxX = relativeDimensions.X;
                    anchoredDimensions.MaxY = relativeDimensions.Y;
                    anchoredDimensions.MinX = 0f;
                    anchoredDimensions.MinY = 0f;
                    break;
                default:
                    break;
            }
                        
            return anchoredDimensions;
        }
    }
}
