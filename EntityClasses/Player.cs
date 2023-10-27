using OpenTK.Mathematics;
using RectangleF = System.Drawing.RectangleF;
using OpenTK.Windowing.Common;
using FeloxGame.EntityClasses;
using FeloxGame.WorldClasses;
using FeloxGame.UtilityClasses;

namespace FeloxGame
{
    public class Player : Entity // ICollidable, 
    {
        public RectangleF ColRec
        {
            get
            {
                return new System.Drawing.RectangleF(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Size.X, Size.Y);
            }
        } // collision rectangle
        public RectangleF DrawRec
        {
            get
            {
                RectangleF colRec = ColRec;
                colRec.X = colRec.X - 5;
                colRec.Width = colRec.Width + 10;
                return colRec;
            }
        }// draw rectangle

        // Properties
        public Inventory Inventory { get; set; }
        public float Reach { get; set; }
        public int RenderDistance { get; set; } = 2;
        public eFacing Facing { get; set; } = eFacing.South;
        public World CurrentWorld { get; protected set; }
        protected float Speed { get; set; } = 5.5f; // Todo: move to constructor

        public Player(Vector2 startPos, Vector2 size, string textureAtlasName, World currentWorld)
            : base (startPos, size, textureAtlasName)
        {
            this.Inventory = new Inventory(5, 10, this);
            this.CurrentWorld = currentWorld;
            Reach = 5f;
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

                ChunkTile currentTileX = CurrentWorld.GetTile((int)Math.Floor(collisionPositionX.X), (int)Math.Floor(collisionPositionX.Y));
                ChunkTile currentTileY = CurrentWorld.GetTile((int)Math.Floor(collisionPositionY.X), (int)Math.Floor(collisionPositionY.Y));

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

        public void Update(FrameEventArgs args)
        {
            HandleInput(args);

            ResolveCollision();
        }

        public void HandleInput(FrameEventArgs args) 
        {
        }

        public void ResolveCollision() { }
    }
}
