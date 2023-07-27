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
            this.AspectRatio = koWidth / koHeight;
            this.Anchor = anchor;
            this.Scale = scale;
        }

        // Methods
        public Vector2 GetRelativeDimensions(float OyaWidth, float OyaHeight)
        {
            Vector2 relativeDimensions = new();
            float OyaAspectRatio = OyaWidth / OyaHeight;

            /*float limitingDimension = OyaWidth > OyaHeight ? OyaWidth * Scale : OyaHeight * Scale;
            
            if (OyaWidth > OyaHeight)
            {
                relativeDimensions.X = limitingDimension;
                relativeDimensions.Y = limitingDimension / AspectRatio;
            }
            else
            {
                relativeDimensions.Y = limitingDimension;
                relativeDimensions.X = limitingDimension / AspectRatio;
            }*/

            if (OyaAspectRatio > AspectRatio) //koHeight is constrain
            {
                relativeDimensions.Y = OyaHeight * Scale;
                relativeDimensions.X = relativeDimensions.Y * AspectRatio;
            }
            else //koWidth is constrain
            {
                relativeDimensions.X = OyaWidth * Scale;
                relativeDimensions.Y = relativeDimensions.X / AspectRatio;
            }
            
            return relativeDimensions;

            /// Pseudocode solution:
            /// if OyaAspect (W/H) > KoAspect (W/H), then:
            ///   Oya is wider than Ko, so Ko's Height is the constraining dimension
            ///   All other calculations to follow Ko's height
            /// if KoHeight constraining dimension:
            ///   rel.Y = OyaHeight * scale (0 to 1f)
            ///   rel.X = rel.Y * (old) AspectRatio (change AspectRatio to be set once?)
            /// else (KoWidth constraining dimension):
            ///   rel.X = OyaWidth * scale (0 to 1f)
            ///   rel.Y = rel.X / AspectRatio (should it be "/" ?)

        }


    }
}
