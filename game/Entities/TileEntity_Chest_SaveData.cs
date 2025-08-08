namespace Isola.Entities
{
    public class TileEntity_Chest_SaveData : TileEntitySaveData
    {
        public Inventory Inventory { get; set; }

        public TileEntity_Chest_SaveData(float[] position, float[] size, float[] drawPositionOffset, Inventory inventory)
            : base (position, size, drawPositionOffset)
        {
            Inventory = inventory;
        }
    }
}
