using FeloxGame.Core.Rendering;
using FeloxGame.Rendering;
using FeloxGame.Utilities;
using FeloxGame.GameClasses;
using FeloxGame.World;
using FeloxGame.GUI;
using FeloxGame.Inventories;
using FeloxGame.Entities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

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
        private GameCamera _camera;
        
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
        private UI MasterUI { get; set; }
        int currentAnchor = 0; //debug

        protected override void OnLoad()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Arb.BlendFuncSeparate(0, BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.SrcAlpha, BlendingFactor.DstAlpha);

            _shader = new(Shader.ParseShader(@"../../../Resources/Shaders/TextureWithColorAndTextureSlotAndUniforms.glsl"));
            if (!_shader.CompileShader())
            {
                Console.WriteLine("Failed to compile shader.");
                return;
            }

            _UIShader = new(Shader.ParseShader(@"../../../Resources/Shaders/UIShader.glsl")); // todo: move to UI Load?
            if (!_UIShader.CompileShader())
            {
                Console.WriteLine("Failed to compile shader.");
                return;
            }

            // Textures & Assets
            AssetLibrary.TextureAtlasList.Add("Tile Atlas", new IndexedTextureAtlas(1024, 16, 8, "Tiles/1024 Tile Atlas x16.png", 0, true));
            AssetLibrary.TextureAtlasList.Add("Player Atlas", new TextureAtlas(1024, "Entities/Player.png", 1));
            AssetLibrary.TextureAtlasList.Add("Inventory Atlas", new PrecisionTextureAtlas(1024, "Inventories/1024 UI Atlas x16.png", 2, 1024, 1024));
            AssetLibrary.TextureAtlasList.Add("Item Atlas", new IndexedTextureAtlas(1024, 16, 8, "Items/1024 Item Atlas 16x.png", 3));
            
            AssetLibrary.InitialiseItemList();
            AssetLibrary.InitialiseTileList();

            // World (initialised before player as player will reference it)
            _config = new GameConfig(true, 2);
            _world = new WorldManager(1, _config);
            
            // Player (with reference to _world)
            _player = new PlayerEntity(eEntityType.Player, new Vector2(0, 0), new Vector2(1, 2), "Player Atlas", _world);

            // Entities
            _world.AddEntityToWorld(_player);

            // UI systems
            MasterUI = new(Size.X, Size.Y, eAnchor.Middle, 1.0f);
                MasterUI.Kodomo.Add("Inventory", new InventoryUI(196f, 110f, eAnchor.Middle, 0.5f, true, false, false, 5, 10, 16f, 16f, 9f, 6f, 2f, _player.Inventory));
                MasterUI.Kodomo["Inventory"].SetTextureCoords(0, 0, 196, 110); //todo: set on instantiation
                MasterUI.Kodomo.Add("Hotbar", new HotbarUI(188f, 26f, eAnchor.Bottom, 0.5f, true, true, false, 1, 10, 16f, 16f, 5f, 2f, _player.Inventory));
                MasterUI.Kodomo["Hotbar"].SetTextureCoords(0, 118, 188, 26); //todo: set on instantiation

            // Textures
            _shader.Use(); //todo: do I need this?

            // Camera
            _camera = new GameCamera(Vector3.UnitZ * 10, Size.X / (float)Size.Y);

            //GameCursor
            _cursor = new GameCursor();

            // Events
            ((InventoryUI)MasterUI.Kodomo["Inventory"]).SubscribeToInventory(_player.Inventory);
            ((HotbarUI)MasterUI.Kodomo["Hotbar"]).SubscribeToInventory(_player.Inventory);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            KeyboardState keyboardInput = KeyboardState;

            _world.Update(_player); // World has to be updated at least once before player is first updated

            _player.Update(args, keyboardInput);

            // keyboard

            if (keyboardInput.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (keyboardInput.IsKeyReleased(Keys.E))
            {
                toggleInventory = !toggleInventory; // todo: find where this should belong
                if (toggleInventory)
                {
                    MasterUI.Kodomo["Inventory"].ToggleDraw = true;
                    ((HotbarUI)MasterUI.Kodomo["Hotbar"]).ToggleScrolling = false;
                }
                else
                {
                    MasterUI.Kodomo["Inventory"].ToggleDraw = false;
                    ((HotbarUI)MasterUI.Kodomo["Hotbar"]).ToggleScrolling = true;
                }
            }

            // Test - changing anchor
            if (keyboardInput.IsKeyPressed(Keys.R))
            {
                // temporary code to ensure Anchors are working. Will leave for now as debug
                currentAnchor++;
                if (currentAnchor > 8) { currentAnchor = 0; }
                switch (currentAnchor)
                {
                    case 0:
                        MasterUI.Kodomo["Inventory"].Anchor = eAnchor.Middle;
                        break;
                    case 1:
                        MasterUI.Kodomo["Inventory"].Anchor = eAnchor.Left;
                        break;
                    case 2:
                        MasterUI.Kodomo["Inventory"].Anchor = eAnchor.Top;
                        break;
                    case 3:
                        MasterUI.Kodomo["Inventory"].Anchor = eAnchor.Right;
                        break;
                    case 4:
                        MasterUI.Kodomo["Inventory"].Anchor = eAnchor.Bottom;
                        break;
                    case 5:
                        MasterUI.Kodomo["Inventory"].Anchor = eAnchor.TopLeft;
                        break;
                    case 6:
                        MasterUI.Kodomo["Inventory"].Anchor = eAnchor.TopRight;
                        break;
                    case 7:
                        MasterUI.Kodomo["Inventory"].Anchor = eAnchor.BottomRight;
                        break;
                    case 8:
                        MasterUI.Kodomo["Inventory"].Anchor = eAnchor.BottomLeft;
                        break;
                }
                MasterUI.SetNDCs(Size.X, Size.Y, new NDC(-1f, -1f, 1f, 1f));
            }
            // TEST - add hoe to player inventory
            if (keyboardInput.IsKeyPressed(Keys.U))
            {
                _player.Inventory.AddToSlotIndex(new ItemStack("Stone Hoe", 1), 0);
            }

            // TEST - add persimmon to player inventory
            if (keyboardInput.IsKeyPressed(Keys.P))
            {
                _player.Inventory.AddToSlotIndex(new ItemStack("Persimmon", 1), 0);
            }

            // Test - add rice seeds to player inventory
            if (keyboardInput.IsKeyPressed(Keys.I))
            {
                _player.Inventory.AddToSlotIndex(new ItemStack("Wheat Seeds", 1), 1);
            }

            // TEST - count items in inventory
            if (keyboardInput.IsKeyPressed(Keys.O))
            {
                foreach (var item in _player.Inventory._itemStackList)
                {
                    if (item != null)
                    {
                        Console.WriteLine($"{item.ItemName}: {item.Amount}");
                    }
                }
            }

            // TEST - spawn entity
            if (keyboardInput.IsKeyPressed(Keys.L))
            {
                _world.AddEntityToWorld(new ItemEntity(eEntityType.ItemEntity, _player.Position, "Persimmon", 1));
            }

            // TEST - save chunk
            if (keyboardInput.IsKeyPressed(Keys.K))
            {
                _world.Save();
                Console.WriteLine("Debug: current world saved");
            }

            // Track player with camera
            Vector3 cameraMoveDirection = new Vector3(_player.Position.X - _camera.Position.X, _player.Position.Y - _camera.Position.Y, 0f);
            _camera.Position += cameraMoveDirection * 0.05f;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Enable(EnableCap.DepthTest | EnableCap.Blend);
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // ---------- WORLD & Entities ----------

            _shader.Use();

            // matrices for camera
            _shader.SetMatrix4("model", Matrix4.Identity);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _world.Draw();

            // ---------- UI ----------

            GL.Disable(EnableCap.DepthTest);
            _UIShader.Use();
            MasterUI.Draw();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            _camera.AspectRatio = (float)e.Width / e.Height;
            _camera.UpdateCameraDimensions();
            MasterUI.OnResize(e.Width, e.Height, new NDC(-1f, -1f, 1f, 1f));
        }

        // ===== INPUT =====

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            MasterUI.OnKeyDown(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            _cursor.UpdateWorldAndScreenPosition(MousePosition, _camera.Position, Size, _camera.Width, _camera.Height);

            MasterUI.OnMouseMove(_cursor.ScreenPosition);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            _cursor.UpdateWorldAndScreenPosition(MousePosition, _camera.Position, Size, _camera.Width, _camera.Height);
            float distanceFromPlayer = Vector2.Distance(_player.Position, _cursor.WorldPosition);
            // debug
            //Console.WriteLine($"{_cursor.Position.X} => {_cursor.Rounded(_cursor.Position.X)}, {_cursor.Position.Y} => {_cursor.Rounded(_cursor.Position.Y)}");
            //Console.WriteLine($"The cursor is {distanceFromPlayer} units from the player.");

            // if inventory open
            if (toggleInventory)
            {
                // left click
                if (e.Button == MouseButton.Left)
                {
                    MasterUI.OnLeftClick(_cursor.ScreenPosition, _world);
                }
            }

            // if inventory closed
            else
            {
                // left click
                if (e.Button == MouseButton.Left)
                {
                    //_world.SetTile(_cursor.Rounded(_cursor.WorldPosition.X), _cursor.Rounded(_cursor.WorldPosition.Y), "Grass");
                    MasterUI.Kodomo["Hotbar"].OnLeftClick(_cursor.WorldPosition, _world);
                }
                
                // right click
                if (e.Button == MouseButton.Right)
                {
                    MasterUI.Kodomo["Hotbar"].OnRightClick(_cursor.WorldPosition, _world);
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            MasterUI.OnMouseWheel(e);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
