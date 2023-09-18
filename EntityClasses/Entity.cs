using FeloxGame.Core.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using FeloxGame.Core.Management;

namespace FeloxGame
{
    /// <summary>
    /// All entities must have: pos, size,
    /// They should have: texture
    /// </summary>
    public class Entity
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        protected Texture2D TextureAtlas { get; set; }

        // <!----- Rendering ----->
        protected float[] vertices =
        {
            //Vertices          //texCoords //texColors       
            1.0f, 2.0f, 0.001f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //top right (1,1)
            1.0f, 0.0f, 0.001f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom right (1, 0)
            0.0f, 0.0f, 0.001f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom left (0, 0)
            0.0f, 2.0f, 0.001f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f  //top left (0, 1)
        };

        protected uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };
        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;

        public Entity(Vector2 position, Vector2 size, string textureAtlasName, int textureUnit)
        {
            this.Position = position;
            this.Size = size;
            this.TextureAtlas = ResourceManager.Instance.LoadTexture(textureAtlasName, textureUnit);
            OnLoad();
        }
        
        public void OnLoad()
        {
            _vertexArray = new();
            _vertexBuffer = new VertexBuffer(vertices);

            BufferLayout layout = new();
            layout.Add<float>(3); // Positions
            layout.Add<float>(2); // Texture Coords
            layout.Add<float>(3); // Texture Color

            _vertexArray.AddBuffer(_vertexBuffer, layout);
            _indexBuffer = new IndexBuffer(_indices);
        }

        public void Update(FrameEventArgs args) { }
        public void Draw()
        {
            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();
                        
            vertices[0] =  Position.X + Size.X / 2f; vertices[1] =  Position.Y + Size.Y;
            vertices[8] =  Position.X + Size.X / 2f; vertices[9] =  Position.Y;
            vertices[16] = Position.X - Size.X / 2f; vertices[17] = Position.Y;
            vertices[24] = Position.X - Size.X / 2f; vertices[25] = Position.Y + Size.Y;

            TextureAtlas.Use(); //GL.ActiveTexture() and GL.BindTexture()

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.DynamicDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
        }
    }
}
