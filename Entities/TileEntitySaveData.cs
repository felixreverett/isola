namespace FeloxGame.Entities
{
    public class TileEntitySaveData : EntitySaveData
    {
        public float[] Position { get; set; }
        public float[] Size { get; set; }
        public float[] DrawPositionOffset { get; set; }

        public TileEntitySaveData(float[] position, float[] size, float[] drawPositionOffset)
            : base(position, size)
        {
            DrawPositionOffset = drawPositionOffset;
        }
    }
}
