using FeloxGame.Core.Rendering;
using FeloxGame.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using FeloxGame.WorldClasses;
using FeloxGame.Core.Management;

namespace FeloxGame
{
    public class Game1 : GameWindow
    {
        public Game1(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, NumberOfSamples = 24 })
        {
        }
        
        // Shaders
        private Shader _shader;
        private Shader _UIShader;
        
        // Camera
        float speed = 5.5f;
        private Camera _camera;

        // world data
        private World _world;
        private readonly string tileListFolderPath = @"../../../Resources/Tiles";
        private List<Tile> _tileList; // will contain all tiles

        // player data
        private Player _player;

        // Cursor data
        private GameCursor _cursor;

        // Gamestate data
        private bool toggleInventory = false;

        // Textures
        Texture2D tileAtlas;
        Texture2D playerSprites;
        Texture2D inventoryAtlas;

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

            _UIShader = new(Shader.ParseShader(@"../../../Resources/Shaders/UIShader.glsl"));
            if (!_UIShader.CompileShader())
            {
                Console.WriteLine("Failed to compile shader.");
                return;
            }

            // World
            _world = new World();

            // Player
            _player = new Player(new Vector2(0, 0), new Vector2(1, 2));

            // Textures
            _shader.Use();

            tileAtlas = ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/WorldTextures.png", 0);
            playerSprites = ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/Entities/Player.png", 1); ;
            inventoryAtlas = ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/Inventories/Inventory Atlas.png", 2);

            //var textureSampleUniformLocation = _shader.GetUniformLocation("u_Texture[0]"); // ??
            //int[] samplers = new int[3] { 0, 1, 2 }; // ??
            //GL.Uniform1(textureSampleUniformLocation, 2, samplers); // ??

            // Camera
            _camera = new Camera(Vector3.UnitZ * 10, Size.X / (float)Size.Y);

            //GameCursor
            _cursor = new GameCursor();

            // Resource loading
            _tileList = Loading.LoadAllObjects<Tile>(tileListFolderPath);

            //_shader.Debug();
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
            KeyboardState input = KeyboardState;

            Vector2 movement = Vector2.Zero;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (input.IsKeyReleased(Keys.E))
            {
                toggleInventory = !toggleInventory;
            }

            if (input.IsKeyDown(Keys.A) | input.IsKeyDown(Keys.Left))
            {
                movement.X -= 1.0f;
            }

            if (input.IsKeyDown(Keys.D) | input.IsKeyDown(Keys.Right))
            {
                movement.X += 1.0f;
            }

            if (input.IsKeyDown(Keys.W) | input.IsKeyDown(Keys.Up))
            {
                movement.Y += 1.0f;
            }

            if (input.IsKeyDown(Keys.S) | input.IsKeyDown(Keys.Down))
            {
                movement.Y -= 1.0f;
            }

            if (movement.LengthSquared > 1.0f) { movement.Normalize(); }

            _player.Position += movement * (speed * (float)args.Time);

            // Track player with camera
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

            //_shader.SetInt("myTextureUnit", 0);
            _world.Draw(_tileList);

            //_shader.SetInt("myTextureUnit", 1);
            _player.Draw();
            

            if (toggleInventory)
            {
                GL.Disable(EnableCap.DepthTest);
                _UIShader.Use();
                _UIShader.SetMatrix4("model", model);
                _UIShader.SetMatrix4("view", Matrix4.Identity);
                _UIShader.SetMatrix4("projection", Matrix4.Identity);
                //_shader.SetInt("myTextureUnit", 2);
                _player.inventory.Draw();
            }

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            _camera.AspectRatio = (float)e.Width / e.Height;
            _camera.UpdateCameraDimensions();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            _cursor.UpdatePosition(MousePosition, _camera.Position, Size, _camera.Width, _camera.Height);
            
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            _cursor.UpdatePosition(MousePosition, _camera.Position, Size, _camera.Width, _camera.Height);
            float distanceFromPlayer = Vector2.Distance(_player.Position, _cursor.Position);
            // debug
            Console.WriteLine($"{_cursor.Position.X} => {_cursor.Rounded(_cursor.Position.X)}, {_cursor.Position.Y} => {_cursor.Rounded(_cursor.Position.Y)}");
            Console.WriteLine($"The cursor is {distanceFromPlayer} units from the player.");
            _world.UpdateTile(_cursor.Rounded(_cursor.Position.X), _cursor.Rounded(_cursor.Position.Y));
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

    }
}
