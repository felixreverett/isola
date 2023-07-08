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
            : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, NumberOfSamples = 24 })
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

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;
        private Shader _shader;

        // Added from OpenTK/Learn
        float speed = 5.5f;
        private Camera _camera;
        private bool mouseEnabled = false;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        // world data
        private Chunk _testChunk;
        private Dictionary<string, Chunk> _loadedChunks;
        private readonly string worldFolderPath = @"../../../Resources/World/WorldFiles";
        private readonly string tileListFolderPath = @"../../../Resources/Tiles";
        private List<Tile> _tileList; // will contain all tiles

        // player data
        private Entity _player = new Entity(0f, 0f);

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

            // Load all textures (TODO: actually load them all not manually)
            ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/WorldTextures.png");

            // Camera setup
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            //CursorState = CursorState.Grabbed;

            // Resource loading
            _tileList = Loading.LoadAllObjects<Tile>(tileListFolderPath);

            // World loading
            _testChunk = WorldManager.Instance.LoadChunk("Resources/World/worldTest.txt", 0, 0);
            _loadedChunks = new Dictionary<string, Chunk>();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            _player.PosX = _camera.Position.X;
            _player.PosY = _camera.Position.Y;

            int renderDistance = 2;

            // load chunks around the player
            for (int x = (int)_player.PosX / 16 - renderDistance; x <= (int)_player.PosX / 16 + renderDistance; x++)
            {
                for (int y = (int)_player.PosY / 16 - renderDistance; y <= (int)_player.PosY / 16 + renderDistance; y++)
                {
                    string chunkID = $"x{x}y{y}";
                    if (!_loadedChunks.ContainsKey(chunkID))
                    {
                        Chunk newChunk = WorldManager.Instance.LoadOrGenerateChunk($"{worldFolderPath}/{chunkID}.txt", x, y); // load the chunk
                        _loadedChunks.Add(chunkID, newChunk);
                    }
                }
            }

            // unload chunks around the player
            foreach (Chunk chunk in _loadedChunks.Values)
            {
                if (Math.Abs(chunk.ChunkPosX - (int)_player.PosX / 16) > renderDistance || Math.Abs(chunk.ChunkPosY - (int)_player.PosY / 16) > renderDistance)
                {
                    _loadedChunks.Remove($"x{chunk.ChunkPosX}y{chunk.ChunkPosY}");
                }
            } 

            // Keyboard movement
            const float sensitivity = 0.2f;

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

            if (input.IsKeyDown(Keys.A) | input.IsKeyDown(Keys.Left))
            {
                _camera.Position -= _camera.Right * speed * (float)args.Time; //Left
            }

            if (input.IsKeyDown(Keys.D) | input.IsKeyDown(Keys.Right))
            {
                _camera.Position += _camera.Right * speed * (float)args.Time; //Right
            }

            if (input.IsKeyDown(Keys.Space) | input.IsKeyDown(Keys.Up))
            {
                _camera.Position += _camera.Up * speed * (float)args.Time; //Up 
            }

            if (input.IsKeyDown(Keys.LeftShift) | input.IsKeyDown(Keys.Down))
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
            var model = Matrix4.Identity;

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            foreach (Chunk loadedChunk in _loadedChunks.Values)
            {
                for (int y = 0; y < loadedChunk.Tiles.GetLength(1); y++)
                {
                    for (int x = 0; x < loadedChunk.Tiles.GetLength(0); x++)
                    {
                        _vertices[0] = loadedChunk.ChunkPosX * 16 + x + 1; _vertices[1] = loadedChunk.ChunkPosY * 16 + y + 1; // top right (1, 1)
                        _vertices[9] = loadedChunk.ChunkPosX * 16 + x + 1; _vertices[10] = loadedChunk.ChunkPosY * 16 + y; // bottom right (1, 0)
                        _vertices[18] = loadedChunk.ChunkPosX * 16 + x; _vertices[19] = loadedChunk.ChunkPosY * 16 + y; // bottom left (0, 0)
                        _vertices[27] = loadedChunk.ChunkPosX * 16 + x; _vertices[28] = loadedChunk.ChunkPosY * 16 + y + 1; // top left (0, 1)
                        string textureName = loadedChunk.Tiles[x, y];
                        int textureIndex = _tileList.Where(t => t.Name.ToLower() == textureName.ToLower()).FirstOrDefault().TextureIndex;
                        TexCoords texCoords = WorldManager.Instance.GetSubTextureCoordinates(textureIndex);
                        _vertices[3] = texCoords.MaxX; _vertices[4] = texCoords.MaxY;
                        _vertices[12] = texCoords.MinX; _vertices[13] = texCoords.MaxY;
                        _vertices[21] = texCoords.MinX; _vertices[22] = texCoords.MinY;
                        _vertices[30] = texCoords.MaxX; _vertices[31] = texCoords.MinY;
                        //_vertexArray.AddBuffer(_vertexBuffer, layout);
                        GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * _vertices.Length, _vertices);
                        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
                    }
                }
            }

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
