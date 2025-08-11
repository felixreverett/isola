using Isola.Utilities;
using Isola.Drawing;
using Isola.Inventories;
using Isola.Items;
using OpenTK.Mathematics;
using Isola.World;
using Isola.game.GUI;

namespace Isola.ui
{
    public class SlotUI : UI
    {
        protected int ItemSlotID;
        protected Inventory Inventory;
        protected PlayerEntity OwnerPlayer;
        protected IndexedTextureAtlasManager AtlasManager;

        public SlotUI
        (
            float width, float height, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            int itemSlotID, Inventory inventory, RPC position, PlayerEntity ownerPlayer, string atlasName
        )
            : base(width, height, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            ItemSlotID = itemSlotID;
            Inventory = inventory;
            OwnerPlayer = ownerPlayer;
            Position = position;
            AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList[atlasName];
            BatchRenderer = AssetLibrary.BatchRendererList[atlasName];
            // Adding child text element
            Children.Add("Amount", new TextUI(12f, 12f, eAnchor.BottomRight, 1, true, false, false, "0", 8, false, "Font Atlas"));
        }

        public override void Update()
        {
            ItemStack itemStack = OwnerPlayer.Inventory.ItemStackList[ItemSlotID];
            TextUI textAmount = (TextUI)Children["Amount"];

            if (itemStack.Equals(default(ItemStack)))
            {
                ToggleDraw = false;
                textAmount.ToggleDraw = false;
            }
            else
            {
                int textureIndex = 0;
                Item matchingItem = AssetLibrary.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);
                if (matchingItem != null)
                {
                    textureIndex = matchingItem.TextureIndex;
                    TexCoords = AtlasManager.GetIndexedAtlasCoords(textureIndex);
                }
                else
                {
                    Console.WriteLine($"Error: No matching item found in library with name {itemStack.ItemName}. Using default texture 0.");
                }
                ToggleDraw = true;

                // Update the text element and its visibility
                if (itemStack.Amount > 1)
                {
                    textAmount.Text = itemStack.Amount.ToString();
                    textAmount.ToggleDraw = true;
                }
                else
                {
                    textAmount.ToggleDraw = false;
                }
            }
            base.Update();
        }

        public override void OnLeftClick(Vector2 mousePosition, WorldManager world)
        {
            // Swap slots
            if (IsMouseInBounds(mousePosition))
            {
                ItemStack itemStack = OwnerPlayer.Inventory.ItemStackList[ItemSlotID];

                if (itemStack.ItemName == OwnerPlayer.Inventory.MouseSlotItemStack.ItemName)
                {
                    OwnerPlayer.Inventory.ItemStackList[ItemSlotID].Amount += OwnerPlayer.Inventory.MouseSlotItemStack.Amount;
                    OwnerPlayer.Inventory.MouseSlotItemStack = default(ItemStack);
                }
                else
                {
                    OwnerPlayer.Inventory.ItemStackList[ItemSlotID] = OwnerPlayer.Inventory.MouseSlotItemStack;
                    OwnerPlayer.Inventory.MouseSlotItemStack = itemStack;
                }
            }
        }
    }
}
