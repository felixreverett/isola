using FeloxGame.InventoryClasses;
using System.Linq;

namespace FeloxGame
{
    // Inventory items will only be accessed through methods like .Add() and .Remove()
    public class Inventory
    {
        private ItemStack[] _items;

        public Inventory(int rows, int cols)
        {
            this._items = new ItemStack[rows * cols];
        }

        public void Add(ItemStack itemStack)
        {
            var matchingItemStack = _items.FirstOrDefault(i => i.Item.Name == itemStack.Item.Name);
            
            if (matchingItemStack is null)
            {
                if (FirstFreeIndex(out var index))
                {
                    _items[index] = itemStack;
                }
            }
            else
            {
                matchingItemStack.Amount += itemStack.Amount;
            }
        }

        public void Remove(ItemStack itemStack)
        {
            //if (amount == )
        }

        public bool FirstFreeIndex(out int index)
        {
            index = -1;
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] is not null)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
    }
}
