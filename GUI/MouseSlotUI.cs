using FeloxGame.Core.Management;
using FeloxGame.InventoryClasses;
using OpenTK.Mathematics;

namespace FeloxGame.GUI
{
    public class MouseSlotUI : SlotUI
    {
        public MouseSlotUI
        (
            float koWidth, float koHeight, eAnchor anchor, float scale, bool drawable, bool toggleDraw, bool isClickable,
            int itemSlotID, Inventory inventory, TexCoords koPosition
        )
            : base(koWidth, koHeight, anchor, scale, drawable, toggleDraw, isClickable,
                  itemSlotID, inventory, koPosition)
        {

        }

        public override void OnMouseMove(Vector2 mouseNDCs)
        {
            SetTextureCoords(mouseNDCs);

            if (Kodomo.Count > 0)
            {
                foreach (UI ui in Kodomo.Values)
                {
                    ui.OnMouseMove(mouseNDCs);
                }
            }
        }

        public void SetTextureCoords(Vector2 mouseNDCs)
        {
            Vertices[3]  = mouseNDCs.X + KoWidth / 2f; Vertices[4]  = mouseNDCs.Y + KoHeight / 2f; // (1, 1)
            Vertices[11] = mouseNDCs.X + KoWidth / 2f; Vertices[12] = mouseNDCs.Y - KoHeight / 2f; // (1, 0)
            Vertices[19] = mouseNDCs.X - KoWidth / 2f; Vertices[20] = mouseNDCs.Y - KoHeight / 2f; // (0, 0)
            Vertices[27] = mouseNDCs.X - KoWidth / 2f; Vertices[28] = mouseNDCs.Y + KoHeight / 2f; // (0, 1)
        }

    }
}
