namespace Isola.Entities
{
    public class EntitySaveData
    {
        public float[] Position { get; set; }
        public float[] Size { get; set; }

        public EntitySaveData(float[] position, float[] size)
        {
            Position = position;
            Size = size;
        }
    }
}
