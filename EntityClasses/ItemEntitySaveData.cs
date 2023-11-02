using FeloxGame.InventoryClasses;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace FeloxGame.EntityClasses
{
    public class ItemEntitySaveData : EntitySaveData
    {
        public ItemStack ItemStack { get; set; }

        public ItemEntitySaveData(Vector2 position, Vector2 size, ItemStack itemStack) : base(position, size)
        {
            ItemStack = itemStack;
        }
    }
}
