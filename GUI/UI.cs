using OpenTK.Mathematics;

namespace FeloxGame.GUI
{
    public class UI
    {
        // Position
        protected eAnchor Anchor { get; set; }
        protected float KoWidth { get; set; }
        protected float KoHeight { get; set; }
        protected float AspectRatio
        {
            get => KoWidth / KoHeight;
        }
        // You are literally the only field in anything I code.
        // I hope you are proud of yourself.
        protected float _scale;
        protected float Scale
        {
            get => _scale;
            set => _scale = Math.Clamp(value, 0.0f, 1.0f);
        }

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
            this.Anchor = anchor;
            this.Scale = scale;
        }

        // Methods
        public Vector2 GetRelativeDimensions(float OyaWidth, float OyaHeight)
        {
            Vector2 relativeDimensions = new();

            float limitingDimension = OyaWidth > OyaHeight ? OyaWidth * Scale : OyaHeight * Scale;
            
            if (OyaWidth > OyaHeight)
            {
                relativeDimensions.X = limitingDimension;
                relativeDimensions.Y = limitingDimension / AspectRatio;
            }
            else
            {
                relativeDimensions.Y = limitingDimension;
                relativeDimensions.X = limitingDimension / AspectRatio;
            }
            
            return relativeDimensions;
        }


    }
}
