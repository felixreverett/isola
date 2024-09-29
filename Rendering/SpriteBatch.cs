using FeloxGame.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FeloxGame.Rendering
{
    public class SpriteBatch
    {
        private const int MaxQuads = 256;
        private const int MaxVertices = MaxQuads * 4;
        private const int MaxIndices = MaxQuads * 6;

        private float[] _vertices;
        private uint[] _indices;
        private int _vertexCount;
        private int _indexCount;
        private int _quadCount;

        private float _zDepthLayer;

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;
        private TextureAtlas _textureAtlas;

        /*
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

        */

        public SpriteBatch(string textureAtlas, float zDepth) : this(textureAtlas)
        {
            _zDepthLayer = zDepth;
        }

        public SpriteBatch(string textureAtlas)
        {
            _textureAtlas = AssetLibrary.TextureAtlasList[textureAtlas];

            _vertices = new float[MaxVertices * 8];
            _indices = new uint[MaxIndices];

            uint offset = 0;
            for (int i = 0; i < MaxIndices; i+= 6)
            {
                _indices[i + 0] = offset + 0;
                _indices[i + 1] = offset + 1;
                _indices[i + 2] = offset + 3;
                _indices[i + 3] = offset + 1;
                _indices[i + 4] = offset + 2;
                _indices[i + 5] = offset + 3;
                offset += 4;
            }

            _vertexArray = new VertexArray();
            _vertexBuffer = new VertexBuffer(_vertices);
            _indexBuffer = new IndexBuffer(_indices);

            BufferLayout layout = new();
            layout.Add<float>(3); // Positions
            layout.Add<float>(2); // Texture Coords
            layout.Add<float>(3); // Texture Color
            _vertexArray.AddBuffer(_vertexBuffer, layout);
        }

        public void Begin()
        {
            _quadCount = 0;
            _vertexCount = 0;
            _indexCount = 0;
        }

        public void End()
        {
            _textureAtlas.Texture.Use();
            _vertexArray.Bind();

            _vertexBuffer.Bind();
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _vertexCount * sizeof(float), _vertices);

            _indexBuffer.Bind();

            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void DrawQuad(Box2 rect, TexCoords texCoords)
        {
            if (_quadCount >= MaxQuads)
            {
                End();
                Begin();
            }

            float zDepth = _zDepthLayer;

            // Set position vertices
            _vertices[_vertexCount + 0]  = rect.Max.X;
            _vertices[_vertexCount + 1]  = rect.Max.Y;
            _vertices[_vertexCount + 2]  = zDepth;
            _vertices[_vertexCount + 3]  = texCoords.MaxX;
            _vertices[_vertexCount + 4]  = texCoords.MaxY;

            _vertices[_vertexCount + 5]  = 1.0f;
            _vertices[_vertexCount + 6]  = 1.0f;
            _vertices[_vertexCount + 7]  = 1.0f;

            _vertices[_vertexCount + 8]  = rect.Max.X;
            _vertices[_vertexCount + 9]  = rect.Min.Y;
            _vertices[_vertexCount + 10] = zDepth;
            _vertices[_vertexCount + 11] = texCoords.MaxX;
            _vertices[_vertexCount + 12] = texCoords.MinY;

            _vertices[_vertexCount + 13] = 1.0f;
            _vertices[_vertexCount + 14] = 1.0f;
            _vertices[_vertexCount + 15] = 1.0f;

            _vertices[_vertexCount + 16] = rect.Min.X;
            _vertices[_vertexCount + 17] = rect.Min.Y;
            _vertices[_vertexCount + 18] = zDepth;
            _vertices[_vertexCount + 19] = texCoords.MinX;
            _vertices[_vertexCount + 20] = texCoords.MinY;

            _vertices[_vertexCount + 21] = 1.0f;
            _vertices[_vertexCount + 22] = 1.0f;
            _vertices[_vertexCount + 23] = 1.0f;

            _vertices[_vertexCount + 24] = rect.Min.X;
            _vertices[_vertexCount + 25] = rect.Max.Y;
            _vertices[_vertexCount + 26] = zDepth;
            _vertices[_vertexCount + 27] = texCoords.MinX;
            _vertices[_vertexCount + 28] = texCoords.MaxY;

            _vertices[_vertexCount + 29] = 1.0f;
            _vertices[_vertexCount + 30] = 1.0f;
            _vertices[_vertexCount + 31] = 1.0f;

            _vertexCount += 32; // 8 floats per vertex * 4 vertices

            _indexCount += 6; // 6 indices per quad
            _quadCount++;

            /*
            _textureAtlas.Texture.Use();
            _vertexArray.Bind();
            _vertexBuffer.Bind();
            _indexBuffer.Bind();

            SetPositionVertices(rect);

            setTexCoordVertices(texCoords);

            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * _vertices.Length, _vertices);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            */
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
