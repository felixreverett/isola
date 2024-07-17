using FeloxGame.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FeloxGame.Rendering
{
    public class SpriteBatch
    {
        public SpriteBatch(string textureAtlas)
        {
            _textureAtlas = AssetLibrary.TextureAtlasList[textureAtlas];

            _vertexArray = new VertexArray();
            _vertexBuffer = new VertexBuffer(_vertices);
            _indexBuffer = new IndexBuffer(_indices);

            BufferLayout layout = new();
            layout.Add<float>(3); // Positions
            layout.Add<float>(2); // Texture Coords
            layout.Add<float>(3); // Texture Color
            _vertexArray.AddBuffer(_vertexBuffer, layout);
        }

        // Defined four corners of the "quad"
        private readonly float[] _vertices =
        {   //Vertices        //texCoords //texColors
            1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, //top right (1,1)
            1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom right (1, 0)
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, //bottom left (0, 0)
            0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f  //top left (0, 1)
        };

        // Defined vertices of the two triangles of the quad
        private uint[] _indices =
        {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;
        private TextureAtlas _textureAtlas;

        public void DrawQuad(Box2 rect, TexCoords texCoords)
        {
            _textureAtlas.Texture.Use();
            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();

            SetPositionVertices(rect);

            setTexCoordVertices(texCoords);

            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * _vertices.Length, _vertices);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private void SetPositionVertices(Box2 rect)
        {
            _vertices[0]  = rect.Max.X; _vertices[1] =  rect.Max.Y; // top right (1, 1)
            _vertices[8]  = rect.Max.X; _vertices[9] =  rect.Min.Y; // bottom right (1, 0)
            _vertices[16] = rect.Min.X; _vertices[17] = rect.Min.Y; // bottom left (0, 0)
            _vertices[24] = rect.Min.X; _vertices[25] = rect.Max.Y; // top left (0, 1)
        }

        private void setTexCoordVertices(TexCoords texCoords)
        {
            _vertices[3]  = texCoords.MaxX; _vertices[4]  = texCoords.MaxY; // (1, 1)
            _vertices[11] = texCoords.MaxX; _vertices[12] = texCoords.MinY; // (1, 0)
            _vertices[19] = texCoords.MinX; _vertices[20] = texCoords.MinY; // (0, 0)
            _vertices[27] = texCoords.MinX; _vertices[28] = texCoords.MaxY; // (0, 1)
        }
    }
}
