using FeloxGame.Utilities;
using FeloxGame.Drawing;
using FeloxGame.Inventories;
using FeloxGame.Items;
using OpenTK.Mathematics;

namespace FeloxGame.GUI
{
    public class SlotUI : UI
    {
        protected int ItemSlotID;
        protected Inventory Inventory;
        protected IndexedTextureAtlasManager AtlasManager = (IndexedTextureAtlasManager)AssetLibrary.TextureAtlasManagerList["Item Atlas"];

        public SlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool isDrawable, bool toggleDraw, bool isClickable,
            int itemSlotID, Inventory inventory, RPC koPosition
        )
            : base(koWidth, koHeight, anchor, scale, isDrawable, toggleDraw, isClickable)
        {
            this.ItemSlotID = itemSlotID;
            this.Inventory = inventory;
            this.KoPosition = koPosition;
        }

        /// <summary>
        /// Updates information about the item at this item slot. 
        /// </summary>
        public void UpdateItem(ItemStack itemStack)
        {
            int textureIndex = 0;

            Item matchingItem = AssetLibrary.ItemList.FirstOrDefault(i => i.ItemName == itemStack.ItemName);

            if (matchingItem != null)
            {
                textureIndex = matchingItem.TextureIndex;
            }

            TextureCoordinates = AtlasManager.GetIndexedAtlasCoords(textureIndex);
        }
              
        public override void Draw()
        {
            // todo: find a way to draw more than one at a time
            if (IsDrawable && ToggleDraw)
            {
                Box2 rect = new Box2(KoNDCs.MinX, KoNDCs.MinY, KoNDCs.MaxX, KoNDCs.MaxY);
                AtlasManager.StartBatch();
                AtlasManager.AddQuadToBatch(rect, TextureCoordinates);
                AtlasManager.EndBatch();
            }

            if (Kodomo.Count != 0 && ToggleDraw)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.Draw();
                }
            }
        }
        
    }
}
