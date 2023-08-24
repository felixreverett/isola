using FeloxGame.Core.Management;
using FeloxGame.WorldClasses;

namespace FeloxGame.GUI
{
    public class ItemSlotUI : UI
    {
        public ItemSlotUI(float koWidth, float koHeight, eAnchor anchor, float scale, bool drawable, bool toggleDraw, TexCoords koPosition) 
            : base(koWidth, koHeight, anchor, scale, drawable, toggleDraw)
        {
            this.KoPosition = koPosition;
            
            SetTextureCoords(4, 840, 346, 180, 1024, 1024);
        }

        /// <summary>
        /// Updates information on the item contained within 
        /// </summary>
        public void UpdateItem(/*feed it the item info*/)
        {
            // if (no item anymore) { ToggleDraw = false; }
            // else
            //SetTextureCoords();
        }

        /*public override void SetTextureCoords()
        {
            TexCoords inventoryCoords = WorldManager.Instance.GetIndexedAtlasCoords();

            // Set texCoords of atlas
            Vertices[3] = inventoryCoords.MaxX; Vertices[4] = inventoryCoords.MaxY; // (1, 1)
            Vertices[11] = inventoryCoords.MaxX; Vertices[12] = inventoryCoords.MinY; // (1, 0)
            Vertices[19] = inventoryCoords.MinX; Vertices[20] = inventoryCoords.MinY; // (0, 0)
            Vertices[27] = inventoryCoords.MinX; Vertices[28] = inventoryCoords.MaxY; // (0, 1)
        }*/

    }
}
