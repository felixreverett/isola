using Isola.Core.Rendering;
using Isola.Drawing;
using Isola.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Isola.engine.graphics
{
    /// <summary>
    /// Handles the batching of sprites into a buffer to reduce total draw calls per frame. One per textureUnit.
    /// Compare "AtlasManager" classes, which are solely responsible for getting the texture coordinates of sprites on an Atlas
    /// These are separated to facilitate different implementations of Atlas Managers
    /// </summary>
    public class BatchRenderer
    {
        // Sprite Batching
        private const int MaxQuads = 256;
        private const int MaxVertices = MaxQuads * 4;
        private const int MaxIndices = MaxQuads * 6;

        private float[] _vertices;
        private uint[] _indices;
        private int _vertexCount;
        private int _indexCount;
        private int _quadCount;

        private float _zDepthLayer = 0;

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;

        // Atlas Information
        public Texture2D Texture { get; private set; }
        private Shader _shader;

        public BatchRenderer(Shader shader, int textureUnit, string atlasFileName, float zdepth = 0.0f)
        {
            _shader = shader;
            _zDepthLayer = zdepth;
            Texture = ResourceManager.Instance.LoadTextureAtlas(atlasFileName, textureUnit);

            _vertices = new float[MaxVertices * 9];
            _indices = new uint[MaxIndices];

            uint offset = 0;
            for (int i = 0; i < MaxIndices; i += 6)
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
            layout.Add<float>(1); // Texture Index (new Aug 2025)
            _vertexArray.AddBuffer(_vertexBuffer, layout);
        }

        public void StartBatch()
        {
            _quadCount = 0;
            _vertexCount = 0;
            _indexCount = 0;
        }

        public void EndBatch()
        {
            // new2
            //Console.WriteLine($"EndBatch: Using shader program {_shader?.ProgramId}");
            _shader.Use(); //new

            //GL.ActiveTexture(Texture.TextureSlot); //new

            //Console.WriteLine($"EndBatch: Binding texture handle {Texture.Handle} at slot {(int)(Texture.TextureSlot - TextureUnit.Texture0)}");
            Texture.Use();
            var err = GL.GetError();
            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                Console.WriteLine("GL Error in EndBatch() after Texture.Use();: " + err);
            }

            //int textureUnitIndex = (int)Texture.TextureSlot - (int)TextureUnit.Texture0; //new
            //_shader.SetInt("myTextureUnit", textureUnitIndex); //new

            _vertexArray.Bind();
            err = GL.GetError();
            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                Console.WriteLine("GL Error in EndBatch() after _vertexArray.Bind();: " + err);
            }

            _vertexBuffer.Bind();
            err = GL.GetError();
            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                Console.WriteLine("GL Error in EndBatch() after _vertexBuffer.Bind();: " + err);
            }
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, _vertexCount * sizeof(float), _vertices);
            err = GL.GetError();
            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                Console.WriteLine("GL Error in EndBatch() after GL.BufferSubData(): " + err);
            }

            _indexBuffer.Bind();
            err = GL.GetError();
            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                Console.WriteLine("GL Error in EndBatch() after indexBuffer.Bind();: " + err);
            }

            int currentProgram;
            GL.GetInteger(GetPName.CurrentProgram, out currentProgram);
            //Console.WriteLine($"Current shader program before drawing: {currentProgram}");

            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
            err = GL.GetError();
            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                Console.WriteLine("GL Error in EndBatch() after GL.DrawElements();: " + err);
            }


        }

        // Note Aug 2025: I could pass in the texture unit now if I wanted to to sample different ones within the same batch.
        public void AddQuadToBatch(Box2 rect, TexCoords texCoords)
        {
            if (_quadCount >= MaxQuads)
            {
                EndBatch();
                StartBatch();
            }

            //Console.WriteLine($"> Adding quad with texture index: {Texture.TextureSlot}");

            float zDepth = _zDepthLayer;

            float texIndex = (int)Texture.TextureSlot - (int)TextureUnit.Texture0;

            // Set position vertices
            _vertices[_vertexCount + 0] = rect.Max.X;
            _vertices[_vertexCount + 1] = rect.Max.Y;
            _vertices[_vertexCount + 2] = zDepth;
            _vertices[_vertexCount + 3] = texCoords.MaxX;
            _vertices[_vertexCount + 4] = texCoords.MaxY;

            _vertices[_vertexCount + 5] = 1.0f;
            _vertices[_vertexCount + 6] = 1.0f;
            _vertices[_vertexCount + 7] = 1.0f;

            _vertices[_vertexCount + 8] = texIndex;

            _vertices[_vertexCount + 9] = rect.Max.X;
            _vertices[_vertexCount + 10] = rect.Min.Y;
            _vertices[_vertexCount + 11] = zDepth;
            _vertices[_vertexCount + 12] = texCoords.MaxX;
            _vertices[_vertexCount + 13] = texCoords.MinY;

            _vertices[_vertexCount + 14] = 1.0f;
            _vertices[_vertexCount + 15] = 1.0f;
            _vertices[_vertexCount + 16] = 1.0f;

            _vertices[_vertexCount + 17] = texIndex;

            _vertices[_vertexCount + 18] = rect.Min.X;
            _vertices[_vertexCount + 19] = rect.Min.Y;
            _vertices[_vertexCount + 20] = zDepth;
            _vertices[_vertexCount + 21] = texCoords.MinX;
            _vertices[_vertexCount + 22] = texCoords.MinY;

            _vertices[_vertexCount + 23] = 1.0f;
            _vertices[_vertexCount + 24] = 1.0f;
            _vertices[_vertexCount + 25] = 1.0f;

            _vertices[_vertexCount + 26] = texIndex;

            _vertices[_vertexCount + 27] = rect.Min.X;
            _vertices[_vertexCount + 28] = rect.Max.Y;
            _vertices[_vertexCount + 29] = zDepth;
            _vertices[_vertexCount + 30] = texCoords.MinX;
            _vertices[_vertexCount + 31] = texCoords.MaxY;

            _vertices[_vertexCount + 32] = 1.0f;
            _vertices[_vertexCount + 33] = 1.0f;
            _vertices[_vertexCount + 34] = 1.0f;

            _vertices[_vertexCount + 35] = texIndex;

            _vertexCount += 36; // 9 floats per vertex * 4 vertices

            _indexCount += 6; // 6 indices per quad
            _quadCount++;
        }
    }
}
