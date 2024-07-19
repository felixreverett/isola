using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using FeloxGame.Entities;
using FeloxGame.World;
using FeloxGame.Utilities;
using OpenTK.Windowing.GraphicsLibraryFramework;
using FeloxGame.Rendering;

namespace FeloxGame
{
    public class Player : Entity
    {
        // Properties
        public Inventory Inventory { get; set; }
        public float Reach { get; set; }
        public int RenderDistance { get; set; } = 2; // todo: move to game config
        public eFacing Facing { get; set; } = eFacing.South;
        public WorldManager CurrentWorld { get; protected set; } // todo: resolve how to set this on laod
        protected float Speed { get; set; } = 5.5f; // Todo: move to constructor

        public Player(eEntityType entityType, Vector2 startPos, Vector2 size, string textureAtlasName, WorldManager currentWorld)
            : base (entityType, startPos, textureAtlasName)
        {
            this.Inventory = new Inventory(5, 10, this);
            this.CurrentWorld = currentWorld;
            Reach = 5f;
            this.EntityType = eEntityType.Player;
            this.Size = size;
            TexCoords = new TexCoords(0f, 0f, 1f, 1f);
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
