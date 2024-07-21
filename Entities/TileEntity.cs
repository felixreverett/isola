using FeloxGame.Rendering;
using OpenTK.Mathematics;
using FeloxGame.Entities;
using System.Text.Json;

namespace FeloxGame
{
    public class TileEntity : Entity
    {
        /* Base class:
         * EntityType; Position; Size; EntityTextureAtlas; Batch; TexCoords
         */

        private Vector2 DrawPositionOffset;

        // Initialize TileEntity from save data
        public TileEntity(TileEntitySaveData saveData, string textureAtlasName = "TileEntity Atlas")
            : base(saveData)
        {
            Position = new Vector2((float)Math.Floor(saveData.Position[0]), (float)Math.Floor(saveData.Position[1]));
            Size = new Vector2(saveData.Size[0], saveData.Size[1]);
            Batch = new SpriteBatch(textureAtlasName, 0.001f);
        }

        // Default constructor
        public TileEntity(eEntityType entityType, Vector2 position, string textureAtlasName)
        {
            EntityType = entityType;
            Position = new Vector2((float)Math.Floor(position.X), (float)Math.Floor(position.Y));
            Batch = new SpriteBatch(textureAtlasName, 0.001f);
        }

        // TileEntities use integer position but can have an offset for drawing
        public override void Draw()
        {
            Box2 rect = new Box2(Position.X + DrawPositionOffset.X, Position.Y + DrawPositionOffset.Y, Position.X + DrawPositionOffset.X + Size.X, Position.Y + DrawPositionOffset.Y + Size.Y);

            Batch.DrawQuad(rect, TexCoords);
        }

        // Export entity save data
        public virtual EntitySaveDataObject GetSaveData()
        {
            TileEntitySaveData data = new
                (
                    new float[] { Position.X, Position.Y },                     // 0
                    new float[] { Size.X, Size.Y },                             // 1
                    new float[] { DrawPositionOffset.X, DrawPositionOffset.Y }  // 2
                );

            return new EntitySaveDataObject(EntityType, JsonSerializer.Serialize(data));
        }
    }
}
