using FeloxGame.Core.Management;
using OpenTK.Mathematics;

namespace FeloxGame.GUI
{
    public class UI
    {
        // Position
        protected eAnchor Anchor { get; set; }
        protected float KoWidth { get; set; }
        protected float KoHeight { get; set; }
        protected float AspectRatio { get; set; } //removed the getter
        protected float Scale { get; set; }
        protected TexCoords KoNDCs { get; set; }

        // Kodomo
        protected List<UI> Kodomo { get; set; }

        // Rendering
        protected float[] Vertices =
        {
            //Vertices        //texCoords //texColors       
            1.0f, 1.0f, 0.3f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //top right (1,1)
            1.0f, 0.0f, 0.3f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom right (1, 0)
            0.0f, 0.0f, 0.3f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom left (0, 0)
            0.0f, 1.0f, 0.3f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f  //top left (0, 1)
        };
        protected uint[] Indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };

        // Constructor
        public UI(float koWidth, float koHeight, eAnchor anchor, float scale)
        {
            this.KoWidth = koWidth;
            this.KoHeight = koHeight;
            this.AspectRatio = koWidth / koHeight;
            this.Anchor = anchor;
            this.Scale = Math.Clamp(scale, 0.0f, 1.0f);
            this.KoNDCs = new();
            this.Kodomo = new List<UI>();
        }



        // Methods
        // Todo: rename this to something more appropriate
        public void SetNDCs(float oyaWidth, float oyaHeight, TexCoords oyaNDCs)
        {
            Vector2 anchoredDimensions = GetAnchoredDimensions(oyaWidth, oyaHeight);
            KoNDCs.MaxX = anchoredDimensions.X / oyaWidth * oyaNDCs.MaxX;
            KoNDCs.MinX = oyaNDCs.MaxX - KoNDCs.MaxX;
            KoNDCs.MaxY = anchoredDimensions.Y / oyaHeight * oyaNDCs.MaxY;
            KoNDCs.MinY = oyaNDCs.MaxY - KoNDCs.MaxY;

            // Set screen position
            Vertices[0]  = KoNDCs.MaxX; Vertices[1]  = KoNDCs.MaxY; // (1, 1)
            Vertices[8]  = KoNDCs.MaxX; Vertices[9]  = KoNDCs.MinY; // (1, 0)
            Vertices[16] = KoNDCs.MinX; Vertices[17] = KoNDCs.MinY; // (0, 0)
            Vertices[24] = KoNDCs.MinX; Vertices[25] = KoNDCs.MaxY; // (0, 1)
        }

        public Vector2 GetRelativeDimensions(float OyaWidth, float OyaHeight)
        {
            Vector2 relativeDimensions = new();
            float OyaAspectRatio = OyaWidth / OyaHeight;
                        
            if (OyaAspectRatio > AspectRatio) //koHeight is constraint
            {
                relativeDimensions.Y = OyaHeight * Scale;
                relativeDimensions.X = relativeDimensions.Y * AspectRatio;
            }
            else //koWidth is constraint
            {
                relativeDimensions.X = OyaWidth * Scale;
                relativeDimensions.Y = relativeDimensions.X / AspectRatio;
            }
            
            return relativeDimensions;
        }

        public Vector2 GetAnchoredDimensions(float OyaWidth, float OyaHeight)
        {
            Vector2 relativeDimensions = GetRelativeDimensions(OyaWidth, OyaHeight);
            Vector2 anchoredDimensions = new();
            switch (this.Anchor)
            {
                case eAnchor.Middle:
                    anchoredDimensions.X = (OyaWidth + relativeDimensions.X) / 2f;
                    anchoredDimensions.Y = (OyaHeight + relativeDimensions.Y) / 2f;
                    break;
                case eAnchor.Left:
                    anchoredDimensions.X = relativeDimensions.X;
                    anchoredDimensions.Y = (OyaHeight + relativeDimensions.Y) / 2f;
                    break;
                case eAnchor.Top:
                    anchoredDimensions.X = (OyaWidth + relativeDimensions.X) / 2f;
                    anchoredDimensions.Y = OyaHeight;
                    break;
                case eAnchor.Right:
                    anchoredDimensions.X = OyaWidth;
                    anchoredDimensions.Y = (OyaHeight + relativeDimensions.Y) / 2f;
                    break;
                case eAnchor.Bottom:
                    anchoredDimensions.X = (OyaWidth + relativeDimensions.X) / 2f;
                    anchoredDimensions.Y = relativeDimensions.Y;
                    break;
                case eAnchor.TopLeft:
                    anchoredDimensions.X = relativeDimensions.X;
                    anchoredDimensions.Y = OyaHeight;
                    break;
                case eAnchor.TopRight:
                    anchoredDimensions.X = OyaWidth;
                    anchoredDimensions.Y = OyaHeight;
                    break;
                case eAnchor.BottomRight:
                    anchoredDimensions.X = OyaWidth;
                    anchoredDimensions.Y = relativeDimensions.Y;
                    break;
                case eAnchor.BottomLeft:
                    anchoredDimensions.X = relativeDimensions.X;
                    anchoredDimensions.Y = relativeDimensions.Y;
                    break;
                default:
                    break;
            }
            return anchoredDimensions;
        }

    }
}
