using Isola.Drawing;
using Isola.Inventories;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Isola.Utilities;
using Isola.World;
using Isola.Entities;

namespace Isola.GUI
{
    public class HotbarUI : UI
    {
        // fields
        private int _rows;
        private int _cols;
        private float _itemSlotHeight;
        private float _itemSlotWidth;
        private float _edgePadding;
        private float _itemSlotPadding;
        Inventory Inventory;
        private PlayerEntity OwnerPlayer;
        protected PrecisionTextureAtlasManager AtlasManager;

        public bool ToggleScrolling { get; set; } = true; //todo: improve this name

        /// <summary>
        /// Creates a HotbarUI class to display an associated inventory
        /// </summary>
        /// <param name="width">The total width of the UI element</param>
        /// <param name="height">The total height of the UI element</param>
        /// <param name="anchor">The eAnchor type</param>
        /// <param name="scale">The scale of the UI element</param>
        /// <param name="isDrawable">Whether the UI element will use a texture atlas</param>
        /// <param name="toggleDraw">Whether the UI element is currently drawn</param>
        /// <param name="isClickable">Whether the UI element subscribes to Mouse Click events</param>
        /// <param name="rows">The number of inventory rows</param>
        /// <param name="cols">the number of inventory columns</param>
        /// <param name="itemSlotHeight">The height of each item slot</param>
        /// <param name="itemSlotWidth">The width of each item slot</param>
        /// <param name="edgePadding">The amount of padding between the edge of the itemslots and the edge of the UI element</param>
        /// <param name="itemSlotPadding">The amount of padding between item slots</param>
        /// <param name="inventory">The associated inventory of the UI element</param>
        public HotbarUI
        (
            float width, float height, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            int rows, int cols, float itemSlotHeight, float itemSlotWidth, float edgePadding, float itemSlotPadding, Inventory inventory, PlayerEntity ownerPlayer, string atlasName
        )
            : base(width, height, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            _rows = rows;
            _cols = cols;
            _itemSlotHeight = itemSlotHeight;
            _itemSlotWidth = itemSlotWidth;
            _edgePadding = edgePadding;
            _itemSlotPadding = itemSlotPadding;
            Inventory = inventory;
            OwnerPlayer = ownerPlayer;
            AtlasManager = (PrecisionTextureAtlasManager)AssetLibrary.TextureAtlasManagerList[atlasName];
            BatchRenderer = AssetLibrary.BatchRendererList[atlasName];
            SetTextureCoords(0, 118, 188, 26);
            GenerateChildren();
        }

        public void GenerateChildren()
        {
            int slotIndex = 0;

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    RPC childPosition = new();

                    childPosition.MinX = _edgePadding + col * (_itemSlotWidth + _itemSlotPadding);

                    if (row == 0)
                    {
                        childPosition.MinY = _edgePadding;
                    }
                    else
                    {
                        childPosition.MinY = Height - _edgePadding - (row * _itemSlotHeight) - (row - 1) * _itemSlotPadding;
                    }

                    childPosition.MaxX = childPosition.MinX + _itemSlotWidth;
                    childPosition.MaxY = childPosition.MinY + _itemSlotHeight;

                    Children.Add($"{slotIndex}", new SlotUI(_itemSlotWidth, _itemSlotHeight, eAnchor.None, 1f, true, false, false, slotIndex, Inventory, childPosition, OwnerPlayer, "Item Atlas"));

                    slotIndex++;
                }
            }

            // Add "activeHotbarSlot" kodomo
            RPC basePosition = new();
            basePosition.MinX = _edgePadding - 1f;
            basePosition.MinY = _edgePadding - 1f;
            basePosition.MaxX = basePosition.MinX + _itemSlotWidth + 2f;
            basePosition.MaxY = basePosition.MinY + _itemSlotHeight + 2f;
            Children.Add("ActiveHotbarSlot", new ActiveHotbarSlotUI(_itemSlotHeight + 2f, _itemSlotWidth + 2f, eAnchor.None, 1f, true, true, false, 0, 152, 18, 18, basePosition, 0, (_cols * _rows - 1), 0));
        }

        public void SetTextureCoords(float x, float y, float textureWidth, float textureHeight)
        {
            TexCoords = AtlasManager.GetPrecisionAtlasCoords(x, y, textureWidth, textureHeight);
        }

        public override void OnMouseWheel(MouseWheelEventArgs e)
        {
            // Only run if ActiveHotbarSlotUI is accepting scrollwheel updates?
            if (ToggleScrolling)
            {
                Children["ActiveHotbarSlot"].OnMouseWheel(e);
                Children["ActiveHotbarSlot"].SetNDCs(Width, Height, NDCs);
            }
        }

        public override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (ToggleScrolling)
            {
                // drop one item into world if item exists at slot
                if (e.Key == Keys.Q)
                {
                    int index = ((ActiveHotbarSlotUI)Children["ActiveHotbarSlot"]).ActiveIndex;

                    ItemStack itemStack = Inventory.ItemStackList[index];

                    if (!itemStack.Equals(default(ItemStack)))
                    {
                        if (itemStack.Amount == 1)
                        {
                            Inventory.ItemStackList[index] = default(ItemStack);
                        }
                        else
                        {
                            Inventory.ItemStackList[index].Amount -= 1;
                        }

                        OwnerPlayer.CurrentWorld.AddEntityToWorld(new ItemEntity(OwnerPlayer.Position, itemStack.ItemName, itemStack.Amount));
                    }
                }
            }
        }

        public override void OnRightClick(Vector2 mouseNDCs, WorldManager world)
        {
            int index = ((ActiveHotbarSlotUI)Children["ActiveHotbarSlot"]).ActiveIndex;
            
            if (!Inventory.ItemStackList[index].Equals(default(ItemStack)))
            {
                if (AssetLibrary.GetItemFromItemName(Inventory.ItemStackList[index].ItemName, out var item))
                {
                    item!.OnRightClick(mouseNDCs, world);
                }
            }
        }

        public override void OnLeftClick(Vector2 mouseNDCs, WorldManager world)
        {
            int index = ((ActiveHotbarSlotUI)Children["ActiveHotbarSlot"]).ActiveIndex;

            if (!Inventory.ItemStackList[index].Equals(default(ItemStack)))
            {
                if (AssetLibrary.GetItemFromItemName(Inventory.ItemStackList[index].ItemName, out var item))
                {
                    item!.OnLeftClick(mouseNDCs, world);
                }
            }
        }
    }
}
