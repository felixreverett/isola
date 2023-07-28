using FeloxGame.Core.Management;
using FeloxGame.Core.Rendering;
using FeloxGame.GUI;
using FeloxGame.WorldClasses;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FeloxGame
{
    public class Inventory : UI
    {
        // Fields (inventory will only be accessed through methods like .Add() and .Remove()
        private Dictionary<Item, int> _items;

        // Rendering
        private float[] vertices;
        private uint[] indices;

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;
        private Texture2D inventoryAtlas;
        private TexCoords inventoryCoords;

        public Inventory(float koWidth, float koHeight, eAnchor anchor, float scale)
            : base(koWidth, koHeight, anchor, scale)
        {
            this.inventoryAtlas = ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/Inventories/Inventory Atlas.png", 2);
            this.inventoryCoords = WorldManager.Instance.GetTexCoordFromAtlas(4, 840, 346, 180, 1024, 1024);
            this.vertices = base.Vertices;
            this.indices = base.Indices;
            UpdateTextureCoords();
            UpdateScreenCoordsNew(346, 180);
            OnLoad();
        }

        public void OnLoad()
        {
            _items = new Dictionary<Item, int>();

            _vertexArray = new();
            _vertexBuffer = new VertexBuffer(vertices);

            BufferLayout layout = new();
            layout.Add<float>(3); // Positions
            layout.Add<float>(2); // Texture Coords
            layout.Add<float>(3); // Texture Color

            _vertexArray.AddBuffer(_vertexBuffer, layout);
            _indexBuffer = new IndexBuffer(indices);
        }

        public void UpdateTextureCoords()
        {
            // Set texCoords of atlas
            vertices[3] = inventoryCoords.MaxX; vertices[4] = inventoryCoords.MaxY;   // (1, 1)
            vertices[11] = inventoryCoords.MaxX; vertices[12] = inventoryCoords.MinY; // (1, 0)
            vertices[19] = inventoryCoords.MinX; vertices[20] = inventoryCoords.MinY; // (0, 0)
            vertices[27] = inventoryCoords.MinX; vertices[28] = inventoryCoords.MaxY; // (0, 1)
        }

        public void UpdateScreenCoordsNew(int windowWidth, int windowHeight)
        {
            Vector2 relativeDimensions = GetRelativeDimensions(windowWidth, windowHeight);

            // Calculate the NDC width and height based on the desired size
            float ndcWidth = (relativeDimensions.X / windowWidth);
            float ndcHeight = (relativeDimensions.Y / windowHeight);

            TexCoords screenCoords = new TexCoords();

            screenCoords.MaxX = ndcWidth; screenCoords.MinX = -ndcWidth;
            screenCoords.MaxY = ndcHeight; screenCoords.MinY = -ndcHeight;

            // Set screen position
            vertices[0] = screenCoords.MaxX; vertices[1] = screenCoords.MaxY;   // (1, 1)
            vertices[8] = screenCoords.MaxX; vertices[9] = screenCoords.MinY;   // (1, 0)
            vertices[16] = screenCoords.MinX; vertices[17] = screenCoords.MinY; // (0, 0)
            vertices[24] = screenCoords.MinX; vertices[25] = screenCoords.MaxY; // (0, 1)
        }

        public void Draw()
        {
            inventoryAtlas.Use(); //GL.ActiveTexture() and GL.BindTexture()

            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.DynamicDraw);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
        }
    }
}
