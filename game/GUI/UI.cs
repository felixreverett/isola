﻿using FeloxGame.Utilities;
using FeloxGame.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using FeloxGame.World;

namespace FeloxGame.GUI
{
    public class UI : IDrawable
    {
        // Position
        public eAnchor Anchor { get; set; }
        protected float KoWidth { get; set; }
        protected float KoHeight { get; set; }
        protected float AspectRatio { get; set; }
        protected float Scale { get; set; }
        protected RPC KoPosition { get; set; }
        protected NDC KoNDCs { get; set; }
        protected TexCoords TextureCoordinates { get; set; }

        // Kodomo
        public Dictionary<string, UI> Kodomo { get; set; }

        // Rendering
        protected virtual TextureAtlasManager AtlasManager { get; set; }

        protected bool IsDrawable { get; set; }
        public bool ToggleDraw { get; set; }

        protected bool IsClickable { get; set; }

        // Constructor
        public UI(float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable = false, bool toggleDraw = true, bool isClickable = false)
        {
            this.KoWidth = koWidth;
            this.KoHeight = koHeight;
            this.AspectRatio = koWidth / koHeight;
            this.Anchor = anchor;
            this.Scale = Math.Clamp(scale, 0.0f, 1.0f);
            this.KoPosition = new();
            this.KoNDCs = new();
            this.Kodomo = new Dictionary<string, UI>();
            this.IsDrawable = isDrawable;
            this.ToggleDraw = toggleDraw;
            this.IsClickable = isClickable;
            OnLoad();
        }

        public void OnLoad()
        {
            if (IsDrawable)
            {
                if (AtlasManager is null)
                {
                    AtlasManager = AssetLibrary.TextureAtlasManagerList["Inventory Atlas"];
                }
            }
        }

        public virtual void Draw()
        {
            if (IsDrawable && ToggleDraw)
            {
                Box2 rect = new Box2(KoNDCs.MinX, KoNDCs.MinY, KoNDCs.MaxX, KoNDCs.MaxY);
                AtlasManager.StartBatch();
                AtlasManager.AddQuadToBatch(rect, TextureCoordinates);
                AtlasManager.EndBatch();
            }
            
            if (Kodomo.Count != 0 && ToggleDraw)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.Draw();
                }
            }
        }

        public virtual void Update()
        {
            if (Kodomo.Count != 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.Update();
                }
            }
        }

        public virtual void SetTextureCoords(float x, float y, float textureWidth, float textureHeight)
        {
            TextureCoordinates = AtlasManager.GetPrecisionAtlasCoords(x, y, textureWidth, textureHeight);
        }

        public void OnResize(float oyaWidth, float oyaHeight, NDC oyaNDCs)
        {
            this.KoWidth = oyaWidth;
            this.KoHeight = oyaHeight;
            this.AspectRatio = oyaWidth / oyaHeight;
            SetNDCs(oyaWidth, oyaHeight, oyaNDCs);
        }

        public virtual void OnMouseMove(Vector2 mouseNDCs)
        {
            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
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
                if (Kodomo.Count > 0)
                {
                    foreach (UI ui in Kodomo.Values)
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
                if (Kodomo.Count > 0)
                {
                    foreach (UI ui in Kodomo.Values)
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
            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.OnMouseWheel(e);
                }
            }
        }

        public virtual void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.OnKeyDown(e);
                }
            }
        }

        public bool IsMouseInBounds(Vector2 mouseNDCs)
        {
            if (mouseNDCs.X >= KoNDCs.MinX && mouseNDCs.Y >= KoNDCs.MinY && mouseNDCs.X <= KoNDCs.MaxX && mouseNDCs.Y <= KoNDCs.MaxY) { return true; }
            else { return false; }
        }

        public virtual void SetNDCs(float oyaWidth, float oyaHeight, NDC oyaNDCs)
        {
            if (Anchor != eAnchor.None)
            {
                KoPosition = GetAnchoredDimensions(oyaWidth, oyaHeight);
            }

            // map anchored coordinates
            KoNDCs.MaxX = ((KoPosition.MaxX / oyaWidth) * (oyaNDCs.MaxX - oyaNDCs.MinX) + oyaNDCs.MinX);
            KoNDCs.MinX = ((KoPosition.MinX / oyaWidth) * (oyaNDCs.MaxX - oyaNDCs.MinX) + oyaNDCs.MinX);
            KoNDCs.MaxY = ((KoPosition.MaxY / oyaHeight) * (oyaNDCs.MaxY - oyaNDCs.MinY) + oyaNDCs.MinY);
            KoNDCs.MinY = ((KoPosition.MinY / oyaHeight) * (oyaNDCs.MaxY - oyaNDCs.MinY) + oyaNDCs.MinY);

            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.SetNDCs(KoWidth, KoHeight, KoNDCs);
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
