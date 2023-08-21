namespace FeloxGame.InventoryClasses
{
    public class ItemStack
    {
        public string ItemName { get; set; }
        public int Amount { get; set; }
        public ItemStack()
        {
            
        }

        public virtual void Use()
        {
            //
        }
    }
}
