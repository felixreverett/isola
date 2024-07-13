using FeloxGame.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using FeloxGame.Utilities;
using System.Text.Json.Serialization;
using FeloxGame.Entities;
using System.Text.Json;

namespace FeloxGame
{
    /// <summary>
    /// All entities must have: pos, size,
    /// They should have: texture
    /// </summary>
    public class Entity
    {
        public eEntityType EntityType { get; set; } = eEntityType.Entity;
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = new Vector2(1f, 1f);
        protected TextureAtlas EntityTextureAtlas { get; set; }
        
        protected float[] vertices =
        {
            //Vertices          //texCoords //texColors       
            1.0f, 2.0f, 0.001f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //top right (1,1)
            1.0f, 0.0f, 0.001f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom right (1, 0)
            0.0f, 0.0f, 0.001f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom left (0, 0)
            0.0f, 2.0f, 0.001f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f  //top left (0, 1)
        };

        protected uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;

        // Initialize Entity from save data
        public Entity(EntitySaveData saveData)
        {
            Console.WriteLine("Loading gets this far 1");
            this.Position = new Vector2(saveData.Position[0], saveData.Position[1]);
            this.Size = new Vector2(saveData.Size[0], saveData.Size[1]);
            string textureAtlasName = "Item Atlas";
            this.EntityTextureAtlas = AssetLibrary.TextureAtlasList[textureAtlasName];
            OnLoad();
        }

        // Default constructor
        public Entity(eEntityType entityType, Vector2 position, string textureAtlasName)
        {
            this.EntityType = entityType;
            this.Position = position;
            this.EntityTextureAtlas = AssetLibrary.TextureAtlasList[textureAtlasName];
            OnLoad();
        }
                
        public void OnLoad()
        {
            SetPosition(this.Position);

            _vertexArray = new();
            _vertexBuffer = new VertexBuffer(vertices);

            BufferLayout layout = new();
            layout.Add<float>(3); // Positions
            layout.Add<float>(2); // Texture Coords
            layout.Add<float>(3); // Texture Color

            _vertexArray.AddBuffer(_vertexBuffer, layout);
            _indexBuffer = new IndexBuffer(_indices);
        }

        public void SetPosition(Vector2 newPosition)
        {
            vertices[0] = newPosition.X + Size.X / 2f; vertices[1] = newPosition.Y + Size.Y;
            vertices[8] = newPosition.X + Size.X / 2f; vertices[9] = newPosition.Y;
            vertices[16] = newPosition.X - Size.X / 2f; vertices[17] = newPosition.Y;
            vertices[24] = newPosition.X - Size.X / 2f; vertices[25] = newPosition.Y + Size.Y;
        }

        public void Update(FrameEventArgs args) { }
        public void Draw()
        {
            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();
            
            EntityTextureAtlas.Texture.Use(); //GL.ActiveTexture() and GL.BindTexture()

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.DynamicDraw);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0); // Used for drawing Elements
        }

        // Export entity save data
        public virtual EntitySaveDataObject GetSaveData()
        {
            EntitySaveData data = new
                (
                    new float[] { Position.X, Position.Y },     // 0
                    new float[] { Size.X, Size.Y }              // 1
                );

            return new EntitySaveDataObject(EntityType, JsonSerializer.Serialize(data));
        }
    }
}
