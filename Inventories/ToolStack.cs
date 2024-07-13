namespace FeloxGame.Inventories
{
    internal class ToolStack : ItemStack
    {
        public ToolStack(string itemName, int amount) : base(itemName, amount)
        {
            
        }

        public override void Use()
        {
            // Reduce durability
            // possibly change from override to use base functionality
        }
    }
}
