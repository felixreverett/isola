using FeloxGame.Core;
using FeloxGame.Core.Management;
using FeloxGame.Core.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FeloxGame
{
    internal class MyGame : Game
    {
        public MyGame(string windowTitle, int initialWindowWidth, int initialWindowHeight) : base(windowTitle, initialWindowWidth, initialWindowHeight)
        {
        }

        private readonly float[] _vertices = {
            //Positions         //texCoords //texColor        //texunit
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, //top right
             0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom left
            -0.5f,  0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f  //top left
        };

        private uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        //private int _vertexBufferObject;
        //private int _vertexArrayObject;
        private IndexBuffer _indexBuffer;

        private Shader _shader;

        // Added from OpenTK/Learn
        private Camera _camera;
        private MouseState mouse;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        protected override void Initialize()
        {

        }

        protected override void LoadContent()
        {
            _shader = new(Shader.ParseShader("Resources/Shaders/TextureWithColorAndTextureSlot.glsl"));
            if (!_shader.CompileShader())
            {
                Console.WriteLine("Failed to compile shader.");
                return;
            }

            _vertexArray = new();
            _vertexBuffer = new VertexBuffer(_vertices);

            BufferLayout layout = new();
            layout.Add<float>(3); // Positions
            layout.Add<float>(2); // Texture Coords
            layout.Add<float>(3); // Texture Color
            layout.Add<float>(1); // Texture Slot

            _vertexArray.AddBuffer(_vertexBuffer, layout);
            _shader.Use();
            _indexBuffer = new IndexBuffer(_indices);
            var textureSampleUniformLocation = _shader.GetUniformLocation("u_Texture[0]");
            int[] samplers = new int[2] { 0, 1 };
            GL.Uniform1(textureSampleUniformLocation, 2, samplers);

            ResourceManager.Instance.LoadTexture("Resources/Textures/wall.jpg");
            ResourceManager.Instance.LoadTexture("Resources/Textures/awesomeface.png");
            //_texture = ResourceManager.Instance.LoadTexture("Resources/Textures/wall.jpg");
            //_texture.Use(); // Only call once for 1 texture slot use
        }

        protected override void Update(GameTime gameTime)
        {

        }

        protected override void Render(GameTime gameTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color4.CornflowerBlue);
            _shader.Use();
            _vertexArray.Bind();
            _indexBuffer.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
        }

    }
}
