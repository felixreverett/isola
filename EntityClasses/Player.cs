using OpenTK.Mathematics;
using RectangleF = System.Drawing.RectangleF;
using OpenTK.Windowing.Common;
using FeloxGame.EntityClasses;
using FeloxGame.WorldClasses;
using FeloxGame.Core;

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

        public Player(Vector2 startPos, Vector2 size, string textureAtlasName, int textureUnit, World currentWorld)
            : base (startPos, size, textureAtlasName, textureUnit)
        {
            this.Inventory = new Inventory(5, 10, this);
            this.CurrentWorld = currentWorld;
            Reach = 5f;
        }

        public void UpdatePosition(Vector2 movement, float time)
        {
            Vector2 newPosition = this.Position + movement * (Speed * time);

            //Todo: try setting new pos together without separate x and y components?
            
            // if collision detected with tilemap
            string currentTile = CurrentWorld.GetTile((int)Math.Floor(newPosition.X), (int)Math.Floor(newPosition.Y));
            
            if (AssetLibrary.TileList
                .Where(tile => tile.Name == currentTile)
                .Select(tile => tile.IsCollidable)
                .FirstOrDefault())
            {
                //Console.WriteLine(currentTile);
                //Console.WriteLine("Collision detected"); //debug
            }
            else
            {
                this.Position = newPosition;
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
