using FeloxGame.Core.Rendering;
using FeloxGame.Core.Management;
using FeloxGame.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using FeloxGame.World;

namespace FeloxGame
{
    public class Game1 : GameWindow
    {
        public Game1(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {
        }

        private readonly float[] _vertices =
        {   //Vertices        //texCoords //texColors       //texUnit
            1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, //top right (1,1)
            1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom right (1, 0)
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom left (0, 0)
            0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f  //top left (0, 1)
        };

        private uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };

        private readonly float[] _cubevertices =
        {
            //Positions         //texCoords //texColor        //texunit
             0.5f,  0.5f,  0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, //top right front      0
             0.5f, -0.5f,  0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom right front   1
            -0.5f, -0.5f,  0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom left front    2
            -0.5f,  0.5f,  0.5f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, //top left front       3
             0.5f,  0.5f, -0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, //top right back       4
             0.5f, -0.5f, -0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom right back    5
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, //bottom left back     6
            -0.5f,  0.5f, -0.5f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f  //top left back        7
        };

        private uint[] _cubeindices =
        {
            0, 1, 3,
            1, 2, 3, // front
            4, 5, 0,
            5, 1, 0, // right
            7, 6, 4,
            6, 5, 4, // back
            3, 2, 7,
            2, 6, 7, // left
            4, 0, 7,
            0, 3, 7, // top
            5, 1, 6,
            1, 2, 6  // bottom
        };

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;
        private Shader _shader;

        // Added from OpenTK/Learn
        float speed = 2.5f;
        private Camera _camera;
        private bool mouseEnabled = false;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        // world data
        private Chunk _testChunk;

        protected override void OnLoad()
        {
            GL.Enable(EnableCap.DepthTest);
            _shader = new(Shader.ParseShader("Resources/Shaders/TextureWithColorAndTextureSlotAndUniforms.glsl"));
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

            ResourceManager.Instance.LoadTexture("Resources/Textures/grass.png");
            ResourceManager.Instance.LoadTexture("Resources/Textures/awesomeface.png");

            // Camera setup
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            CursorState = CursorState.Grabbed;

            // World setup
            _testChunk = WorldGenerator.Instance.LoadChunk("Resources/World/worldTest.txt");
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            const float sensitivity = 0.2f;

            // Keyboard movement

            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * speed * (float)args.Time; //Forward 
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * speed * (float)args.Time; //Backwards
            }

            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * speed * (float)args.Time; //Left
            }

            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * speed * (float)args.Time; //Right
            }

            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * speed * (float)args.Time; //Up 
            }

            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * speed * (float)args.Time; //Down
            }

            if (mouseEnabled)
            {
                // Get the mouse state
                var mouse = MouseState;

                // Mouse movement

                if (_firstMove)
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    _camera.Yaw += deltaX * sensitivity;
                    _camera.Pitch -= deltaY * sensitivity;
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _shader.Use();
            _vertexArray.Bind();
            _indexBuffer.Bind();

            // matrices for camera
            var model = Matrix4.Identity;// * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time));

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            for (int y = 0; y < _testChunk.Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < _testChunk.Tiles.GetLength(0); x++)
                {
                    _vertices[0]  = x+1; _vertices[1]  = y+1; // top right (1, 1)
                    _vertices[9]  = x+1; _vertices[10] = y; // bottom right (1, 0)
                    _vertices[18] = x; _vertices[19] = y; // bottom left (0, 0)
                    _vertices[27] = x; _vertices[28] = y+1; // top left (0, 1)
                    //_vertexArray.AddBuffer(_vertexBuffer, layout);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * _vertices.Length, _vertices);
                    GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
                }
            }

            //GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

    }
}
