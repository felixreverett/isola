namespace FeloxGame.GUI
{
    public class InventoryUI : UI
    {
        // ItemSlots
        private int _rows;
        private int _cols;
        private float _itemSlotHeight;
        private float _itemSlotWidth;

        // Hard coded for now
        private float _edgePadding = 4f;
        private float _hotbarPadding = 6f;
        private float _itemSlotPadding = 2f;

        public InventoryUI(float koWidth, float koHeight, eAnchor anchor, float scale, bool drawable, bool toggleDraw, int rows, int cols, float itemSlotHeight, float itemSlotWidth)
            : base(koWidth, koHeight, anchor, scale, drawable, toggleDraw)
        {
            this._rows = rows;
            this._cols = cols;
            this._itemSlotHeight = itemSlotHeight;
            this._itemSlotWidth = itemSlotWidth;
        }

        public void GenerateKodomo()
        {
            // use width, height, rows, cols, itemslotdimensions
            for (int i = 0; i < _rows * _cols; i++)
            {
                base.Kodomo.Add($"{i}", new ItemSlotUI());
                int currentRow = i / _cols;
                int currentCol = i % _cols;
                if (currentRow == 0)
                {

                }
            }
        }
    }
}
