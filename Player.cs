using OpenTK.Mathematics;
using FeloxGame.Core.Rendering;
using RectangleF = System.Drawing.RectangleF;
using FeloxGame.Core.Management;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace FeloxGame
{
    public class Player //: Entity // ICollidable, 
    {
        public Vector2 Position { get; set; }
        private Vector2 Size;
        private Texture2D playerSprite;
        public Inventory inventory;
        public int RenderDistance { get; set; } = 2;
        public RectangleF ColRec
        {
            get
            {
                return new System.Drawing.RectangleF(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Size.X, Size.Y);
            }
        } // collision rectangle
        public RectangleF DrawRec
        {
            get
            {
                RectangleF colRec = ColRec;
                colRec.X = colRec.X - 5;
                colRec.Width = colRec.Width + 10;
                return colRec;
            }
        }// draw rectangle

        // Rendering
        private float[] vertices =
        {
            //Vertices        //texCoords //texColors       //texUnit
            1.0f, 2.0f, 0.001f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.5f, //top right (1,1)
            1.0f, 0.0f, 0.001f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.5f, //bottom right (1, 0)
            0.0f, 0.0f, 0.001f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.5f, //bottom left (0, 0)
            0.0f, 2.0f, 0.001f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.5f  //top left (0, 1)
        };

        private uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };
        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;

        // Properties
        public float Reach { get; set; }



        public Player(Vector2 startPos, Vector2 size)
        {
            this.Position = startPos;
            this.Size = size;
            this.playerSprite = ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/Entities/Player.png", 1);
            this.inventory = new Inventory();
            Reach = 5f;
            OnLoad();
        }

        public void Update(FrameEventArgs args)
        {
            HandleInput(args);

            ResolveCollision();
        }

        public void HandleInput(FrameEventArgs args) 
        {
        }

        public void ResolveCollision() { }

        public void OnLoad()
        {
            _vertexArray = new();
            _vertexBuffer = new VertexBuffer(vertices);

            BufferLayout layout = new();
            layout.Add<float>(3); // Positions
            layout.Add<float>(2); // Texture Coords
            layout.Add<float>(3); // Texture Color
            layout.Add<float>(1); // Texture Slot

            _vertexArray.AddBuffer(_vertexBuffer, layout);
            _indexBuffer = new IndexBuffer(_indices);
        }

        public void Draw()
        {
            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();

            vertices[0] = Position.X + Size.X;  vertices[1] = Position.Y + Size.Y;
            vertices[9] = Position.X + Size.X;  vertices[10] = Position.Y;
            vertices[18] = Position.X;          vertices[19] = Position.Y;
            vertices[27] = Position.X;          vertices[28] = Position.Y + Size.Y;

            playerSprite.Use(); //GL.ActiveTexture() and GL.BindTexture()
            
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.DynamicDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements

            /*if (climbing)
            {
                Spritebatch.Draw(playerSprite, this.Position, new Vector2(DrawRec.Width / playerSprite.Width, DrawRec.Height / playerSprite.Height), Color.White, new Vector2(playerSprite.Width / 4f);
            }*/
        }

        public void GetChunkPos()
        {

        }
    }
}
