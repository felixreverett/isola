using FeloxGame.InventoryClasses;
using System.Linq;

namespace FeloxGame
{
    // Inventory items will only be accessed through methods like .Add() and .Remove()
    public class Inventory
    {
        private ItemStack[]? _items;
        private int _rows;
        private int _cols;

        public Inventory(int rows, int cols)
        {
            this._rows = rows;
            this._cols = cols;
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
            var matchingItemStack = _items.FirstOrDefault(i => i.Item.Name == itemStack.Item.Name);

            if ( matchingItemStack is null)
            {
                throw new ArgumentException("Error. Attempted to remove an item not in the inventory");
            }
            else if (matchingItemStack.Amount > itemStack.Amount)
            {
                matchingItemStack.Amount -= itemStack.Amount;
            }
            else if (matchingItemStack.Amount == itemStack.Amount)
            {
                int index = Array.IndexOf(_items, matchingItemStack);
                _items[index] = null;
            }
            else if (matchingItemStack.Amount < itemStack.Amount)
            {
                throw new ArgumentException("Error. Tried to remove more items than in inventory");
            }
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

        public void Sort(eSortType sortType = eSortType.Alphabetical)
        {
            ItemStack[] sortedItems;
            switch (sortType)
            {
                case eSortType.Alphabetical:
                    sortedItems = _items.OrderBy(i => i.Item.Name).ToArray();
                    _items = sortedItems;
                    break;
                case eSortType.Amount:
                    sortedItems = _items.OrderBy(i => i.Amount).ThenBy(i => i.Item.Name).ToArray();
                    break;
                case eSortType.Category:
                    // not yet implemented
                    break;
            }
        }
    }
}
