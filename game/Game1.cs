using FeloxGame.Core.Rendering;
using FeloxGame.Drawing;
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
using FeloxGame.engine.graphics.Buffers;

namespace FeloxGame
{
    public class Game1 : GameWindow
    {
        public Game1(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title, NumberOfSamples = 24 })
        {
        }
        
        // Shaders
        private Shader WorldShader;
        private Shader UIShader;
        private Shader ScreenQuadShader;

        private float[] _quadVertices =
        {
            // Positions    // Texture Coordinates
            -1.0f,  1.0f,   0.0f, 1.0f, // top-left
            -1.0f, -1.0f,   0.0f, 0.0f, // bottom-left
             1.0f, -1.0f,   1.0f, 0.0f, // bottom-right
             1.0f,  1.0f,   1.0f, 1.0f  // top-right
        };

        private uint[] _quadIndices =
        {
            0, 1, 2, // First triangle
            0, 2, 3  // Second triangle
        };

        private VertexArray _quadVAO;
        private VertexBuffer _quadVBO;
        private IndexBuffer _quadEBO;
        
        // Camera
        private GameCamera _camera;
        private FrameBuffer _fbo;
        private const int VIRTUAL_WIDTH = 1920;
        private const int VIRTUAL_HEIGHT = 1080;
        
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
            base.OnLoad();

            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Arb.BlendFuncSeparate(0, BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.SrcAlpha, BlendingFactor.DstAlpha);

            WorldShader = new(Shader.ParseShader(@"../../../Resources/Shaders/TextureWithColorAndTextureSlotAndUniforms.glsl"));
            if (!WorldShader.CompileShader())
            {
                Console.WriteLine("Failed to compile shader.");
                return;
            }

            UIShader = new(Shader.ParseShader(@"../../../Resources/Shaders/UIShader.glsl")); // todo: move to UI Load?
            if (!UIShader.CompileShader())
            {
                Console.WriteLine("Failed to compile shader.");
                return;
            }

            ScreenQuadShader = new(Shader.ParseShader(@"../../../Resources/Shaders/ScreenQuadShader.glsl"));
            if (!ScreenQuadShader.CompileShader())
            {
                Console.WriteLine("[!] Failed to compile shader.");
                return;
            }

            // Asset Loading

            AssetLibrary.InitialiseTextureAtlasManagerList();

            AssetLibrary.InitialiseItemList();

            AssetLibrary.InitialiseTileList();

            // World (initialised before player as player will reference it)
            _config = new GameConfig(true, 10);
            _world = new WorldManager(1, _config);
            
            // Player (with reference to _world)
            _player = new PlayerEntity(new Vector2(0, 0), new Vector2(1, 2), _world);

            // Entities
            _world.AddEntityToWorld(_player);

            // UI systems
            MasterUI = new(ClientSize.X, ClientSize.Y, eAnchor.Middle, 1.0f);
                MasterUI.Kodomo.Add("Hotbar", new HotbarUI(188f, 26f, eAnchor.Bottom, 0.5f, true, true, false, 1, 10, 16f, 16f, 5f, 2f, _player.Inventory, _player));
                MasterUI.Kodomo["Hotbar"].SetTextureCoords(0, 118, 188, 26); //todo: set on instantiation
                MasterUI.Kodomo.Add("Inventory", new PlayerInvUI(196f, 110f, eAnchor.Middle, 0.5f, true, false, false, _player.Inventory, _player));
                MasterUI.Kodomo["Inventory"].SetTextureCoords(0, 0, 196, 110); //todo: set on instantiation

            // Textures
            WorldShader.Use(); //todo: do I need this?

            // Camera
            _camera = new GameCamera(new Vector3(0, 0, 0), ClientSize.X / (float)ClientSize.Y);
            _fbo = new FrameBuffer(VIRTUAL_WIDTH, VIRTUAL_HEIGHT); // determines the native resolution of the game

            _quadVAO = new VertexArray();
            _quadVBO = new VertexBuffer(_quadVertices);
            _quadEBO = new IndexBuffer(_quadIndices);

            var quadLayout = new BufferLayout();
            quadLayout.Add<float>(2); // Position
            quadLayout.Add<float>(2); // Texture Coordinates
            _quadVAO.AddBuffer(_quadVBO, quadLayout);
            _quadEBO.Bind();

            _quadVAO.Unbind();

            //GameCursor
            _cursor = new GameCursor();
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

            MasterUI.Update();

            // keyboard
            if (keyboardInput.IsKeyDown(Keys.Escape))
            {
                //Close();
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
                MasterUI.SetNDCs(ClientSize.X, ClientSize.Y, new NDC(-1f, -1f, 1f, 1f));
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

            // Test - add chest to player inventory
            if (keyboardInput.IsKeyPressed(Keys.RightBracket))
            {
                _player.Inventory.AddToSlotIndex(new ItemStack("Wood Chest", 1), 2);
            }

            // TEST - count items in inventory
            if (keyboardInput.IsKeyPressed(Keys.O))
            {
                foreach (var item in _player.Inventory.ItemStackList)
                {
                    if (!item.Equals(default(ItemStack)))
                    {
                        Console.WriteLine($"{item.ItemName}: {item.Amount}");
                    }
                }
            }

            // TEST - spawn entity
            if (keyboardInput.IsKeyPressed(Keys.L))
            {
                _world.AddEntityToWorld(new ItemEntity(_player.Position, "Persimmon", 1));
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

            // --- Render to Frame Buffer Object ---
            _fbo.Use(); // render world to a frame buffer for fixed native res
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest | EnableCap.Blend);

            // ---------- WORLD & Entities ----------

            WorldShader.Use();
            WorldShader.SetMatrix4("model", Matrix4.Identity);

            WorldShader.SetMatrix4("view", _camera.GetViewMatrix());
            WorldShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _world.Draw();

            // --- Render FBO to screen ---
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.DepthTest);

            ScreenQuadShader.Use();
            ScreenQuadShader.SetInt("u_ScreenTexture", 0);
            _fbo.BindTexture(TextureUnit.Texture0);

            _quadVAO.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _quadIndices.Length, DrawElementsType.UnsignedInt, 0);
            _quadVAO.Unbind();

            // --- Render UI on top ---
            UIShader.Use();
            MasterUI.Draw();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);

            if (_camera != null)
            {
                _camera.AspectRatio = (float)e.Width / e.Height;
                _camera.UpdateCameraDimensions();
                MasterUI.OnResize(e.Width, e.Height, new NDC(-1f, -1f, 1f, 1f));
            }
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

            _cursor.UpdateWorldAndScreenPosition(MousePosition, _camera.Position, ClientSize, _camera.Width, _camera.Height);

            MasterUI.OnMouseMove(_cursor.ScreenPosition);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            _cursor.UpdateWorldAndScreenPosition(MousePosition, _camera.Position, ClientSize, _camera.Width, _camera.Height);
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

            GL.DeleteProgram(WorldShader.ProgramId);
            GL.DeleteProgram(UIShader.ProgramId);
            GL.DeleteProgram(ScreenQuadShader.ProgramId);
        }
    }
}
