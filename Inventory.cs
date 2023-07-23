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
            //Vertices        //texCoords //texColors       
            1.0f, 1.0f, 0.3f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //top right (1,1)
            1.0f, 0.0f, 0.3f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom right (1, 0)
            0.0f, 0.0f, 0.3f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom left (0, 0)
            0.0f, 1.0f, 0.3f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f  //top left (0, 1)
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
            this.inventoryAtlas = ResourceManager.Instance.LoadTexture(@"../../../Resources/Textures/Inventories/Inventory Atlas.png", 2);
            this.inventoryCoords = WorldManager.Instance.GetTexCoordFromAtlas(4, 840, 346, 180, 1024, 1024);
            UpdateTextureCoords();
            UpdateScreenCoords(346, 180);
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
            _indexBuffer = new IndexBuffer(_indices);
        }

        public void UpdateTextureCoords()
        {
            // Set texCoords of atlas
            vertices[3] = inventoryCoords.MaxX; vertices[4] = inventoryCoords.MaxY;   // (1, 1)
            vertices[11] = inventoryCoords.MaxX; vertices[12] = inventoryCoords.MinY; // (1, 0)
            vertices[19] = inventoryCoords.MinX; vertices[20] = inventoryCoords.MinY; // (0, 0)
            vertices[27] = inventoryCoords.MinX; vertices[28] = inventoryCoords.MaxY; // (0, 1)
        }

        public void UpdateScreenCoords(int width, int height)
        {
            float screenPercentage = 1f;
            //int grande = Math.Max(width, height);
            //int pequeño = Math.Min(width, height);
            TexCoords screenCoords = new TexCoords();
            float maxX = ((float)width / width) * screenPercentage;
            float maxY = ((float)height / width) * screenPercentage;
            Console.WriteLine(maxY);
            screenCoords.MaxX = maxX; screenCoords.MinX = -maxX;
            screenCoords.MaxY = maxY; screenCoords.MinY = -maxY;
            Console.WriteLine($"{screenCoords.MaxX}");
            Console.WriteLine($"{screenCoords.MinX}");
            Console.WriteLine(screenCoords.MaxY);
            Console.WriteLine(screenCoords.MinY);

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
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements

        }
    }
}
