using FeloxGame.Core.Management;
using FeloxGame.Core.Rendering;
using FeloxGame.WorldClasses;
using OpenTK.Graphics.OpenGL4;

namespace FeloxGame
{
    public class Inventory
    {
        // Fields (inventory will only be accessed through methods like .Add() and .Remove()
        private Dictionary<Item, int> _items;

        // Rendering

        private float[] vertices =
        {
            //Vertices          //texCoords //texColors       //texUnit
            1.0f, 2.0f, 0.003f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.5f, //top right (1,1)
            1.0f, 0.0f, 0.003f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 2.5f, //bottom right (1, 0)
            0.0f, 0.0f, 0.003f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 2.5f, //bottom left (0, 0)
            0.0f, 2.0f, 0.003f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.5f  //top left (0, 1)
        };

        private uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;
        private Texture2D inventoryAtlas;
        private TexCoords inventoryCoords;

        public Inventory()
        {
            this.inventoryAtlas = ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/Inventories/Inventory Atlas.png");
            this.inventoryCoords = WorldManager.Instance.GetTexCoordFromAtlas(4, 840, 346, 180, 1024, 1024);
            UpdateTextureCoords();
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
            layout.Add<float>(1); // Texture Slot

            _vertexArray.AddBuffer(_vertexBuffer, layout);
            _indexBuffer = new IndexBuffer(_indices);
        }

        public void UpdateTextureCoords()
        {
            vertices[3] = inventoryCoords.MaxX; vertices[4] = inventoryCoords.MaxY;   // (1, 1)
            vertices[12] = inventoryCoords.MaxX; vertices[13] = inventoryCoords.MinY; // (1, 0)
            vertices[21] = inventoryCoords.MinX; vertices[22] = inventoryCoords.MinY; // (0, 0)
            vertices[30] = inventoryCoords.MinX; vertices[31] = inventoryCoords.MaxY; // (0, 1)
        }

        public void Draw()
        {
            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();

            inventoryAtlas.Use(); //GL.ActiveTexture() and GL.BindTexture()
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.DynamicDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements

        }
    }
}
