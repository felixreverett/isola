using FeloxGame.Core;
using FeloxGame.Core.Management;
using FeloxGame.Core.Rendering;
using FeloxGame.InventoryClasses;
using OpenTK.Graphics.OpenGL4;

namespace FeloxGame.GUI
{
    public class SlotUI : UI
    {
        protected int ItemSlotID;
        protected Inventory Inventory;
        protected IndexedTextureAtlas ItemAtlas = (IndexedTextureAtlas)AssetLibrary.TextureAtlasList["Item Atlas"]; //Todo: redesign this whole system.

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

            TexCoords texCoords = ItemAtlas.GetIndexedAtlasCoords(textureIndex);
            //TextureManager.Instance.GetIndexedAtlasCoords(index, 16, 1024, 8);

            SetTextureCoords(texCoords);
        }

        public void SetTextureCoords(TexCoords texCoords)
        {
            // Set texCoords of atlas
            Vertices[3] = texCoords.MaxX; Vertices[4] = texCoords.MaxY; // (1, 1)
            Vertices[11] = texCoords.MaxX; Vertices[12] = texCoords.MinY; // (1, 0)
            Vertices[19] = texCoords.MinX; Vertices[20] = texCoords.MinY; // (0, 0)
            Vertices[27] = texCoords.MinX; Vertices[28] = texCoords.MaxY; // (0, 1)
        }

        public override void Draw()
        {
            if (IsDrawable && ToggleDraw)
            {
                ItemAtlas.Texture.Use();

                _vertexArray.Bind();
                _vertexBuffer.Bind();
                _indexBuffer.Bind();

                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Vertices.Length, Vertices, BufferUsageHint.DynamicDraw);
                GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
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
