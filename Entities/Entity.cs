using FeloxGame.Drawing;
using OpenTK.Mathematics;
using FeloxGame.Entities;
using System.Text.Json;
using FeloxGame.Utilities;

namespace FeloxGame
{
    public class Entity : IDrawable
    {
        public eEntityType EntityType { get; set; } = eEntityType.Entity;
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = new Vector2(1f, 1f);

        // Drawing
        public SpriteBatch Batch { get; private set; }
        public TextureAtlasManager AtlasManager { get; protected set; } = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Item Atlas"];
        protected TexCoords TexCoords { get; set; }
                
        // Initialize Entity from save data
        public Entity(EntitySaveData saveData)
        {
            Position = new Vector2(saveData.Position[0], saveData.Position[1]);
            Size = new Vector2(saveData.Size[0], saveData.Size[1]);
        }

        // Default constructor
        public Entity(eEntityType entityType, Vector2 position)
        {
            EntityType = entityType;
            Position = position;
        }
        
        public virtual void Draw()
        {
            Box2 rect = new Box2(Position.X - Size.X / 2f, Position.Y, Position.X + Size.X / 2f, Position.Y + Size.Y);
            AtlasManager.StartBatch(); // todo: render entities with the same spritebatch
            AtlasManager.AddQuadToBatch(rect, TexCoords);
            AtlasManager.EndBatch(); // todo: render entities with the same spritebatch
        }

        // Export entity save data
        public virtual EntitySaveDataObject GetSaveData()
        {
            EntitySaveData data = new
                (
                    new float[] { Position.X, Position.Y },     // 0
                    new float[] { Size.X, Size.Y }              // 1
                );

            return new EntitySaveDataObject(EntityType, JsonSerializer.Serialize(data));
        }
    }
}
