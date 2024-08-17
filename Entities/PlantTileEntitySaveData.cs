namespace FeloxGame.Entities
{
    public class PlantTileEntitySaveData : TileEntitySaveData
    {
        public int GrowthStage { get; set; }

        public PlantTileEntitySaveData(float[] position, float[] size, float[] drawPositionOffset, int growthStage)
            : base(position, size, drawPositionOffset)
        {
            GrowthStage = growthStage;
        }
    }
}
