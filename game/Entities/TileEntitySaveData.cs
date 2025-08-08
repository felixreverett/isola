namespace Isola.Entities
{
    public class TileEntitySaveData : EntitySaveData
    {
        public float[] DrawPositionOffset { get; set; }

        public TileEntitySaveData(float[] position, float[] size, float[] drawPositionOffset)
            : base(position, size)
        {
            DrawPositionOffset = drawPositionOffset;
        }
    }
}
