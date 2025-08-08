namespace Isola.Entities
{
    public class ItemEntitySaveData : EntitySaveData
    {
        public string ItemName { get; set; }
        public int Amount { get; set; }

        public ItemEntitySaveData(float[] position, float[] size, string itemName, int amount)
            : base(position, size)
        {
            ItemName = itemName;
            Amount = amount;
        }
    }
}