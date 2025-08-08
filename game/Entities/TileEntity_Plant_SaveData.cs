namespace Isola.Entities
{
    public class TileEntity_Plant_SaveData : TileEntitySaveData
    {
        public int GrowthStage { get; set; }

        public TileEntity_Plant_SaveData(float[] position, float[] size, float[] drawPositionOffset, int growthStage)
            : base(position, size, drawPositionOffset)
        {
            GrowthStage = growthStage;
        }
    }
}
