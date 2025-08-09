using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Isola.Entities;
using Isola.World;
using Isola.Utilities;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Isola.engine.graphics;

namespace Isola
{
    public class PlayerEntity : Entity
    {
        public PlayerInventory Inventory { get; set; }
        public float Reach { get; set; }
        public eFacing Facing { get; set; } = eFacing.South;
        public WorldManager CurrentWorld { get; protected set; } // todo: resolve how to set this on load
        protected float Speed { get; set; } = 5.5f; // Todo: move to constructor
        private EntityTextureAtlasManager AtlasManager { get; set; }

        public PlayerEntity(Vector2 startPos, Vector2 size, WorldManager currentWorld)
            : base (startPos)
        {
            Inventory = new PlayerInventory(5, 10);
            CurrentWorld = currentWorld;
            Reach = 5f;
            Size = size;
            AtlasManager = (EntityTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Entity Atlas"];
            BatchRenderer = AssetLibrary.BatchRendererList["Entity Atlas"];
            TexCoords = AtlasManager.GetAtlasCoords("Player");
        }

        public void UpdatePosition(Vector2 movement, float time)
        {
            // Calculate in sub-steps to minimise hovering
            int increments = 5;
            for (int i = 0; i < increments; i++)
            {
                Vector2 newPosition = Position + movement * (Speed/increments * time);
                Vector2 collisionPositionX = new Vector2(newPosition.X, Position.Y);
                Vector2 collisionPositionY = new Vector2(Position.X, newPosition.Y);

                ChunkTile currentTileX = CurrentWorld.GetTile(collisionPositionX.X, collisionPositionX.Y);
                ChunkTile currentTileY = CurrentWorld.GetTile(collisionPositionY.X, collisionPositionY.Y);

                bool collisionX = AssetLibrary.TileList
                    .Where(tile => tile.TileID == currentTileX.TileID)
                    .Select(tile => tile.IsCollidable)
                    .FirstOrDefault();

                bool collisionY = AssetLibrary.TileList
                    .Where(tile => tile.TileID == currentTileY.TileID)
                    .Select(tile => tile.IsCollidable)
                    .FirstOrDefault();

                // if X but not Y
                if (collisionX && !collisionY)
                {
                    Position = collisionPositionY;
                }
                // if Y but not X
                else if (!collisionX && collisionY)
                {
                    Position = collisionPositionX;
                }
                // if neither
                else if (!collisionX && !collisionY)
                {
                    Position = newPosition;
                }
            }
        }

        public void Update(FrameEventArgs args, KeyboardState keyboardInput)
        {
            HandleInput(args, keyboardInput);

            ResolveCollision();
        }

        public void HandleInput(FrameEventArgs args, KeyboardState keyboardInput) //todo: rename to OnKeyDown and rework
        {
            // Keyboard movement

            Vector2 movement = Vector2.Zero;

            if (keyboardInput.IsKeyDown(Keys.A) | keyboardInput.IsKeyDown(Keys.Left))
            {
                movement.X -= 1.0f;
            }

            if (keyboardInput.IsKeyDown(Keys.D) | keyboardInput.IsKeyDown(Keys.Right))
            {
                movement.X += 1.0f;
            }

            if (keyboardInput.IsKeyDown(Keys.W) | keyboardInput.IsKeyDown(Keys.Up))
            {
                movement.Y += 1.0f;
            }

            if (keyboardInput.IsKeyDown(Keys.S) | keyboardInput.IsKeyDown(Keys.Down))
            {
                movement.Y -= 1.0f;
            }

            if (movement.LengthSquared > 1.0f) { movement.Normalize(); }

            UpdatePosition(movement, (float)args.Time);
        }

        public void ResolveCollision() { }
    }
}
