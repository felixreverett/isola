using FeloxGame.Drawing;
using OpenTK.Mathematics;
using FeloxGame.Entities;
using System.Text.Json;

namespace FeloxGame
{
    public class TileEntity : Entity
    {
        internal Vector2 DrawPositionOffset { get; set; } = new Vector2(0f, 0f);

        // Initialize TileEntity from save data
        public TileEntity(TileEntitySaveData saveData)
            : base(saveData)
        {
            DrawPositionOffset = new Vector2(saveData.DrawPositionOffset[0], saveData.DrawPositionOffset[1]);
            AlignPosition();
            Size = new Vector2(saveData.Size[0], saveData.Size[1]);
        }

        // Default constructor
        public TileEntity(eEntityType entityType, Vector2 position)
            : base(entityType, position)
        {
            AlignPosition();
        }

        // TileEntities can have an offset for drawing
        public override void Draw()
        {
            Box2 rect = new Box2(
                Position.X - Size.X / 2f + DrawPositionOffset.X, Position.Y + DrawPositionOffset.Y,
                Position.X + Size.X / 2f + DrawPositionOffset.X, Position.Y + Size.Y + DrawPositionOffset.Y
                );

            Batch.AddQuadToBatch(rect, TexCoords);
        }

        // Export entity save data
        public override EntitySaveDataObject GetSaveData()
        {
            TileEntitySaveData data = new
                (
                    new float[] { Position.X, Position.Y },                     // 0
                    new float[] { Size.X, Size.Y },                             // 1
                    new float[] { DrawPositionOffset.X, DrawPositionOffset.Y }  // 2
                );

            return new EntitySaveDataObject(EntityType, JsonSerializer.Serialize(data));
        }

        private void AlignPosition()
        {
            Position = new Vector2((float)Math.Floor(Position.X) + 0.5f, (float)Math.Floor(Position.Y));
        }
    }
}
