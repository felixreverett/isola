using FeloxGame.Rendering;
using OpenTK.Mathematics;
using FeloxGame.Entities;
using System.Text.Json;

namespace FeloxGame
{
    public class Entity : IDrawable
    {
        public eEntityType EntityType { get; set; } = eEntityType.Entity;
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = new Vector2(1f, 1f);
        protected TextureAtlas EntityTextureAtlas { get; set; }

        // Associated rendering
        public SpriteBatch Batch { get; private set; }
        protected TexCoords TexCoords { get; set; }
                
        // Initialize Entity from save data
        public Entity(EntitySaveData saveData, string textureAtlasName = "Item Atlas")
        {
            Console.WriteLine("Loading entity from save data");

            this.Position = new Vector2(saveData.Position[0], saveData.Position[1]);
            this.Size = new Vector2(saveData.Size[0], saveData.Size[1]);
            
            Batch = new SpriteBatch(textureAtlasName, 0.001f);
        }

        // Default constructor
        public Entity(eEntityType entityType, Vector2 position, string textureAtlasName)
        {
            this.EntityType = entityType;
            this.Position = position;

            Batch = new SpriteBatch(textureAtlasName, 0.001f);
        }
        
        public virtual void Draw()
        {
            Box2 rect = new Box2(Position.X - Size.X / 2f, Position.Y, Position.X + Size.X / 2f, Position.Y + Size.Y);
            
            Batch.DrawQuad(rect, TexCoords);
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
