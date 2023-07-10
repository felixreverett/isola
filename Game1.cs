using FeloxGame.Core.Rendering;
using FeloxGame.Core.Management;
using FeloxGame.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using FeloxGame.WorldClasses;

namespace FeloxGame
{
    public class Game1 : GameWindow
    {
        public Game1(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, NumberOfSamples = 24 })
        {
        }

        private Shader _shader;

        // Added from OpenTK/Learn
        float speed = 5.5f;
        private Camera _camera;
        private bool mouseEnabled = false;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        // world data
        private World _world;
        private Chunk _testChunk;
        private readonly string tileListFolderPath = @"../../../Resources/Tiles";
        private List<Tile> _tileList; // will contain all tiles

        // player data
        private Player _player;

        protected override void OnLoad()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _shader = new(Shader.ParseShader(@"../../../Resources/Shaders/TextureWithColorAndTextureSlotAndUniforms.glsl"));
            if (!_shader.CompileShader())
            {
                Console.WriteLine("Failed to compile shader.");
                return;
            }

            // World
            _world = new World();

            // Player
            _player = new Player(new Vector2(0, 0), new Vector2(1, 2));

            _shader.Use();

            var textureSampleUniformLocation = _shader.GetUniformLocation("u_Texture[0]"); // ??
            int[] samplers = new int[2] { 0, 1 }; // ??
            GL.Uniform1(textureSampleUniformLocation, 2, samplers); // ??

            // Camera
            _camera = new Camera(Vector3.UnitZ * 10, Size.X / (float)Size.Y);

            //CursorState = CursorState.Grabbed;

            // Resource loading
            _tileList = Loading.LoadAllObjects<Tile>(tileListFolderPath);

            // World data loading
            _testChunk = WorldManager.Instance.LoadChunk(@"../../../Resources/World/worldTest.txt", 0, 0);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            _player.Update(args);

            _world.Update(_player);

            // Keyboard movement
            const float sensitivity = 0.2f;

            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            // Zoom for debug
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Front * speed * (float)args.Time; //Forward 
            }

            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Front * speed * (float)args.Time; //Backwards
            }

            if (input.IsKeyDown(Keys.A) | input.IsKeyDown(Keys.Left))
            {
                _player.Position -= new Vector2(speed * (float)args.Time, 0);
            }

            if (input.IsKeyDown(Keys.D) | input.IsKeyDown(Keys.Right))
            {
                _player.Position += new Vector2(speed * (float)args.Time, 0);
            }

            if (input.IsKeyDown(Keys.W) | input.IsKeyDown(Keys.Up))
            {
                _player.Position += new Vector2(0, speed * (float)args.Time);
            }

            if (input.IsKeyDown(Keys.S) | input.IsKeyDown(Keys.Down))
            {
                _player.Position -= new Vector2(0, speed * (float)args.Time);
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

            Vector3 cameraMoveDirection = new Vector3(_player.Position.X - _camera.Position.X, _player.Position.Y - _camera.Position.Y, 0f);
            _camera.Position += cameraMoveDirection * 0.05f;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // matrices for camera
            var model = Matrix4.Identity;

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _world.Draw(_tileList);

            _player.Draw();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            _camera.AspectRatio = (float)e.Width / (float)e.Height;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

    }
}
