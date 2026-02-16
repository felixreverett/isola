using Isola.Drawing;
using Isola.engine.graphics;
using Isola.engine.graphics.Buffers;
using Isola.engine.ui;
using Isola.Entities;
using Isola.game.GUI;
using Isola.GameClasses;
using Isola.Inventories;
using Isola.ui;
using Isola.Utilities;
using Isola.World;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Isola {
    public class Game1 : GameWindow {
        private string _version;

        public Game1(int width, int height, string version)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = version, NumberOfSamples = 24 })
        {
            _version = version;
        }
        
        // Shaders & Camera
        private ScreenQuad _screenQuad;
        
        private GameCamera _camera;
        private FrameBuffer _fbo;

        private const int VIRTUAL_WIDTH = 640; // 1920 or 640
        private const int VIRTUAL_HEIGHT = 360;// 1080 or 360
        
        // world data & config
        private WorldManager _world;
        private GameConfig _config;
        
        // player data
        private PlayerEntity _player;
        
        // Cursor data
        private GameCursor _cursor;
        
        // GameState data
        private bool toggleInventory = false;
        
        // UISystem
        private MasterUI MasterUI { get; set; }
        int currentAnchor = 0; //debug

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            AssetLibrary.Initialise();

            _config = new GameConfig(true, 10);
            _world = new WorldManager(1, _config); // World (initialised before player as player will reference it)
            _player = new PlayerEntity(new Vector2(0, 0), new Vector2(1, 2), _world);
            _world.AddEntityToWorld(_player);

            // UI systems
            MasterUI = new MasterUI(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, eAnchor.Middle, 1.0f);
                MasterUI.Children.Add("Hotbar", new HotbarUI(188f, 26f, eAnchor.Bottom, 1.0f, true, true, false, 1, 10, 16f, 16f, 5f, 2f, _player.Inventory, _player, "Inventory Atlas"));
                MasterUI.Children.Add("Inventory", new PlayerInvUI(196f, 110f, eAnchor.Middle, 1.0f, true, false, false, _player.Inventory, _player, "Inventory Atlas"));
                MasterUI.Children.Add("Build Version", new TextUI(VIRTUAL_WIDTH, 12f, eAnchor.BottomLeft, 1f, true, true, false, _version, 12, "Font Atlas"));

            // Camera
            _camera = new GameCamera(new Vector3(0, 0, 0), ClientSize.X / (float)ClientSize.Y);
            _fbo = new FrameBuffer(VIRTUAL_WIDTH, VIRTUAL_HEIGHT); // determines the native resolution of the game
            _screenQuad = new ScreenQuad();            
            
            //GameCursor
            _cursor = new GameCursor();
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            if (!IsFocused) return;

            KeyboardState keyboardInput = KeyboardState;

            _world.Update(_player); // World has to be updated at least once before player is first updated
            _player.Update(args, keyboardInput);
            MasterUI.Update();

            if (keyboardInput.IsKeyReleased(Keys.E)) {
                toggleInventory = !toggleInventory; // todo: find where this should belong
                if (toggleInventory) {
                    MasterUI.Children["Inventory"].ToggleDraw = true;
                    ((HotbarUI)MasterUI.Children["Hotbar"]).AllowScrolling = false;
                } else {
                    MasterUI.Children["Inventory"].ToggleDraw = false;
                    ((HotbarUI)MasterUI.Children["Hotbar"]).AllowScrolling = true;
                }
            }

            if (keyboardInput.IsKeyPressed(Keys.R)) {
                // temporary code to ensure Anchors are working. Will leave for now as debug
                currentAnchor++;
                if (currentAnchor > 8) { currentAnchor = 0; }
                switch (currentAnchor) {
                    case 0:
                        MasterUI.Children["Inventory"].Anchor = eAnchor.Middle;
                        break;
                    case 1:
                        MasterUI.Children["Inventory"].Anchor = eAnchor.Left;
                        break;
                    case 2:
                        MasterUI.Children["Inventory"].Anchor = eAnchor.Top;
                        break;
                    case 3:
                        MasterUI.Children["Inventory"].Anchor = eAnchor.Right;
                        break;
                    case 4:
                        MasterUI.Children["Inventory"].Anchor = eAnchor.Bottom;
                        break;
                    case 5:
                        MasterUI.Children["Inventory"].Anchor = eAnchor.TopLeft;
                        break;
                    case 6:
                        MasterUI.Children["Inventory"].Anchor = eAnchor.TopRight;
                        break;
                    case 7:
                        MasterUI.Children["Inventory"].Anchor = eAnchor.BottomRight;
                        break;
                    case 8:
                        MasterUI.Children["Inventory"].Anchor = eAnchor.BottomLeft;
                        break;
                }
                MasterUI.OnResize(VIRTUAL_WIDTH, VIRTUAL_HEIGHT);
            }
            // TEST - add hoe to player inventory
            if (keyboardInput.IsKeyPressed(Keys.U)) _player.Inventory.AddToSlotIndex(new ItemStack("Stone Hoe", 1), 0);

            // TEST - add persimmon to player inventory
            if (keyboardInput.IsKeyPressed(Keys.P)) _player.Inventory.AddToSlotIndex(new ItemStack("Persimmon", 1), 0);

            // Test - add rice seeds to player inventory
            if (keyboardInput.IsKeyPressed(Keys.I)) _player.Inventory.AddToSlotIndex(new ItemStack("Wheat Seeds", 1), 1);

            // Test - add chest to player inventory
            if (keyboardInput.IsKeyPressed(Keys.RightBracket)) _player.Inventory.AddToSlotIndex(new ItemStack("Wood Chest", 1), 2);

            // TEST - count items in inventory
            if (keyboardInput.IsKeyPressed(Keys.O)) {
                foreach (var item in _player.Inventory.ItemStackList) {
                    if (!item.Equals(default(ItemStack))) Console.WriteLine($"{item.ItemName}: {item.Amount}");
                }
            }

            // TEST - spawn entity
            if (keyboardInput.IsKeyPressed(Keys.L)) _world.AddEntityToWorld(new ItemEntity(_player.Position, "Persimmon", 1));

            // TEST - save chunk
            if (keyboardInput.IsKeyPressed(Keys.K)) {
                _world.Save();
                Console.WriteLine("Debug: current world saved");
            }

            // Track player with camera
            Vector3 cameraMoveDirection = new Vector3(_player.Position.X - _camera.Position.X, _player.Position.Y - _camera.Position.Y, 0f);
            _camera.Position += (cameraMoveDirection * 0.05f);
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);
            // --- Render to Frame Buffer Object ---
            _fbo.Use();
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Blend);

            // ---------- WORLD & Entities ----------
            AssetLibrary.ShaderList["World Shader"].Use();
            AssetLibrary.ShaderList["World Shader"].SetMatrix4("model", Matrix4.Identity);
            AssetLibrary.ShaderList["World Shader"].SetMatrix4("view", _camera.GetViewMatrix());
            AssetLibrary.ShaderList["World Shader"].SetMatrix4("projection", _camera.GetProjectionMatrix());

            _world.Draw();

            AssetLibrary.ShaderList["UI Shader"].Use();
            MasterUI.Draw();

            // --- Render FBO to screen ---
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Disable(EnableCap.DepthTest); // no longer depth testing
            AssetLibrary.ShaderList["Screen Quad Shader"].Use();
            AssetLibrary.ShaderList["Screen Quad Shader"].SetInt("u_Texture", 0);
            _fbo.BindTexture(TextureUnit.Texture0);
            _screenQuad.Draw();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);

            if (_camera != null) {
                _camera.AspectRatio = (float)e.Width / e.Height;
                _camera.UpdateCameraDimensions();
                MasterUI.OnResize(VIRTUAL_WIDTH, VIRTUAL_HEIGHT);
            }
        }

        // ===== INPUT =====
        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);
            MasterUI.OnKeyDown(e);
        }

        private Vector2 GetVirtualMousePosition() {
            float scaleX = (float)VIRTUAL_WIDTH / ClientSize.X;
            float scaleY = (float)VIRTUAL_HEIGHT / ClientSize.Y;
            float x = MousePosition.X * scaleX;
            float y = VIRTUAL_HEIGHT - (MousePosition.Y * scaleY);
            return new Vector2(x, y);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            base.OnMouseMove(e);
            _cursor.UpdateWorldAndScreenPosition(MousePosition, _camera.Position, ClientSize, _camera.Width, _camera.Height);
            MasterUI.OnMouseMove(GetVirtualMousePosition());
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);

            _cursor.UpdateWorldAndScreenPosition(MousePosition, _camera.Position, ClientSize, _camera.Width, _camera.Height);
            float distanceFromPlayer = Vector2.Distance(_player.Position, _cursor.WorldPosition);
            // debug
            //Console.WriteLine($"{_cursor.Position.X} => {_cursor.Rounded(_cursor.Position.X)}, {_cursor.Position.Y} => {_cursor.Rounded(_cursor.Position.Y)}");
            //Console.WriteLine($"The cursor is {distanceFromPlayer} units from the player.");

            // if inventory open
            if (toggleInventory) {
                if (e.Button == MouseButton.Left) MasterUI.OnLeftClick(GetVirtualMousePosition(), _world);
            } else {
                if (e.Button == MouseButton.Left) MasterUI.Children["Hotbar"].OnLeftClick(GetVirtualMousePosition(), _world);
                
                if (e.Button == MouseButton.Right) MasterUI.Children["Hotbar"].OnRightClick(GetVirtualMousePosition(), _world);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);
            MasterUI.OnMouseWheel(e);
        }

        protected override void OnUnload() {
            base.OnUnload();
        }
    }
}
