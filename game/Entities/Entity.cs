using Isola.Drawing;
using OpenTK.Mathematics;
using Isola.Entities;
using System.Text.Json;
using Isola.Utilities;
using Isola.Saving;
using Isola.engine.graphics;

namespace Isola
{
    public abstract class Entity : IDrawable, ISaveable<EntitySaveDataObject>
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = new Vector2(1f, 1f);

        // Drawing
        public BatchRenderer ?BatchRenderer { get; protected set; }
        protected TexCoords ?TexCoords { get; set; }
                
        // Initialize Entity from save data
        public Entity(EntitySaveData saveData)
        {
            Position = new Vector2(saveData.Position[0], saveData.Position[1]);
            Size = new Vector2(saveData.Size[0], saveData.Size[1]);
        }

        // Default constructor
        public Entity(Vector2 position)
        {
            Position = position;
        }
        
        public virtual void Draw()
        {
            if (BatchRenderer == null || TexCoords == null)
            {
                Console.WriteLine($"[W] Warning: Attempted to draw Entity without setting BatchRenderer || TexCoords.");
            }
            else
            {
                Box2 rect = new Box2(Position.X - Size.X / 2f, Position.Y, Position.X + Size.X / 2f, Position.Y + Size.Y); //todo (Aug 2025): do I always need to update this?
                BatchRenderer.StartBatch(); // todo: render entities with the same spritebatch
                BatchRenderer.AddQuadToBatch(rect, TexCoords);
                BatchRenderer.EndBatch(); // todo: render entities with the same spritebatch
            }
        }

        // Export entity save data
        public virtual EntitySaveDataObject GetSaveData()
        {
            EntitySaveData data = new
                (
                    new float[] { Position.X, Position.Y },     // 0
                    new float[] { Size.X, Size.Y }              // 1
                );

            return new EntitySaveDataObject(eEntityType.Entity, JsonSerializer.Serialize(data));
        }
    }
}
