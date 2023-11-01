using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace FeloxGame.EntityClasses
{
    public class EntitySaveData
    {
        [JsonInclude] public Vector2 Position { get; set; }
        [JsonInclude] public Vector2 Size { get; set; }

        public EntitySaveData(Vector2 position, Vector2 size)
        {
            this.Position = position;
            this.Size = size;
        }
    }
}
