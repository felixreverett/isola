using FeloxGame.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using FeloxGame.UtilityClasses;
using System.Text.Json.Serialization;
using FeloxGame.EntityClasses;

namespace FeloxGame
{
    /// <summary>
    /// All entities must have: pos, size,
    /// They should have: texture
    /// </summary>
    [JsonDerivedType(typeof(ItemEntity))]
    [JsonDerivedType(typeof(Player))]
    public class Entity
    {
        [JsonInclude] public Vector2 Position { get; set; }
        [JsonInclude] public Vector2 Size { get; set; } = new Vector2(1f, 1f);
        public eEntityType EntityType { get; set; }
        [JsonIgnore] protected TextureAtlas EntityTextureAtlas { get; set; }
        
        // <!----- Rendering ----->
        [JsonIgnore] protected float[] vertices =
        {
            //Vertices          //texCoords //texColors       
            1.0f, 2.0f, 0.001f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //top right (1,1)
            1.0f, 0.0f, 0.001f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom right (1, 0)
            0.0f, 0.0f, 0.001f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom left (0, 0)
            0.0f, 2.0f, 0.001f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f  //top left (0, 1)
        };
        [JsonIgnore] protected uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };
        [JsonIgnore] private VertexBuffer _vertexBuffer;
        [JsonIgnore] private VertexArray _vertexArray;
        [JsonIgnore] private IndexBuffer _indexBuffer;

        // <!----- Constructors ----->
        public Entity()
        {
            string textureAtlasName = "Item Atlas";
            this.EntityTextureAtlas = AssetLibrary.TextureAtlasList[textureAtlasName];
        }

        public Entity(Vector2 position, Vector2 size, string textureAtlasName) : this(position, textureAtlasName)
        {
            this.Size = size;
        }

        public Entity(Vector2 position, string textureAtlasName)
        {
            this.Position = position;
            this.EntityTextureAtlas = AssetLibrary.TextureAtlasList[textureAtlasName];
            OnLoad();
        }

        // <!----- Methods ----->
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

        public virtual void LoadData(EntitySaveData entitySaveData)
        {
            this.Position = (Vector2)entitySaveData.Data[0];
            this.Size = (Vector2)entitySaveData.Data[1];
        }

        // prepares entity save data
        public virtual EntitySaveData GetSaveData()
        {
            List<object> Data = new()
            {
                Position,
                Size,
            };

            return new EntitySaveData(EntityType, Data);
        }
    }
}
