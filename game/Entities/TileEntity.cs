using Isola.Drawing;
using Isola.engine.graphics;
using Isola.Entities;
using Isola.Utilities;
using OpenTK.Mathematics;
using System.Text.Json;

namespace Isola
{
    public abstract class TileEntity : Entity
    {
        internal Vector2 DrawPositionOffset { get; set; } = new Vector2(0f, 0f);
        private EntityTextureAtlasManager AtlasManager { get; set; }

        // Initialize TileEntity from save data
        public TileEntity(TileEntitySaveData saveData)
            : base(saveData)
        {
            DrawPositionOffset = new Vector2(saveData.DrawPositionOffset[0], saveData.DrawPositionOffset[1]);
            AlignPosition();
            Size = new Vector2(saveData.Size[0], saveData.Size[1]);
            AtlasManager = (EntityTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Entity Atlas"];
            BatchRenderer = AssetLibrary.BatchRendererList["Entity Atlas"];
        }

        // Default constructor
        public TileEntity(Vector2 position, Vector2 drawPositionOffset)
            : base(position)
        {
            AlignPosition();
            AtlasManager = (EntityTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Entity Atlas"];
            BatchRenderer = AssetLibrary.BatchRendererList["Entity Atlas"];
        }

        // TileEntities can have an offset for drawing
        public override void Draw()
        {
            Box2 rect = new Box2(
                Position.X - Size.X / 2f + DrawPositionOffset.X, Position.Y + DrawPositionOffset.Y,
                Position.X + Size.X / 2f + DrawPositionOffset.X, Position.Y + Size.Y + DrawPositionOffset.Y
                );

            BatchRenderer.AddQuadToBatch(rect, TexCoords);
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

            return new EntitySaveDataObject(eEntityType.TileEntity, JsonSerializer.Serialize(data));
        }

        private void AlignPosition()
        {
            Position = new Vector2((float)Math.Floor(Position.X) + 0.5f, (float)Math.Floor(Position.Y));
        }
    }
}
