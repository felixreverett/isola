using Isola.Core.Rendering;
using Isola.Drawing;
using Isola.Utilities;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Isola.engine.graphics {
    /// <summary>
    /// Handles the batching of sprites into a buffer to reduce total draw calls per frame. One per textureUnit.
    /// Compare "AtlasManager" classes, which are solely responsible for getting the texture coordinates of sprites on an Atlas
    /// These are separated to facilitate different implementations of Atlas Managers
    /// </summary>
    public class BatchRenderer {
        private readonly ILogger<BatchRenderer> _logger;

        // Sprite Batching
        private const int MaxQuads = 8192;
        private const int MaxVertices = MaxQuads * 4;
        private const int MaxIndices = MaxQuads * 6;
        private const int FloatsPerVertex = 9;

        private float[] _vertices;
        private uint[] _indices;

        private int _vertexCount;
        private int _indexCount;
        private int _quadCount;

        private readonly float _zDepthLayer;

        private VertexBuffer _vertexBuffer;
        private VertexArray _vertexArray;
        private IndexBuffer _indexBuffer;

        // Atlas Information
        public Texture2D Texture { get; private set; }
        private Shader _shader;

        public BatchRenderer(Shader shader, int textureUnit, string atlasFileName, ILogger<BatchRenderer> logger, float zdepth = 0.0f) {
            _shader = shader;
            _zDepthLayer = zdepth;
            _logger = logger;
            
            Texture = ResourceManager.Instance.LoadTextureAtlas(atlasFileName, textureUnit);

            _vertices = new float[MaxVertices * FloatsPerVertex];
            _indices = new uint[MaxIndices];

            uint offset = 0;
            for (int i = 0; i < MaxIndices; i += 6) {
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

            _logger.LogDebug("BatchRenderer initialized for {Atlas} on Unit {Unit}", atlasFileName, textureUnit);
        }

        public void StartBatch() {
            _quadCount = 0;
            _vertexCount = 0;
            _indexCount = 0;
        }

        public void EndBatch() {
            if (_indexCount == 0) return;

            _shader.Use();
            Texture.Use();

            _vertexArray.Bind();
            _vertexBuffer.Bind();

            int totalFloats = _vertexCount * FloatsPerVertex;

            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, totalFloats * sizeof(float), _vertices);
            CheckGLError("After GL.BufferSubData");

            _indexBuffer.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
            CheckGLError("After GL.DrawElements");
        }

        // Note Aug 2025: I could pass in the texture unit now if I wanted to to sample different ones within the same batch.
        public void AddQuadToBatch(Box2 rect, TexCoords texCoords) {
            if (_quadCount >= MaxQuads) {
                EndBatch();
                StartBatch();
            }

            int i = _vertexCount * FloatsPerVertex;
            float texIndex = (int)Texture.TextureSlot - (int)TextureUnit.Texture0;

            // Set position vertices
            _vertices[i + 0] = rect.Max.X;
            _vertices[i + 1] = rect.Max.Y;
            _vertices[i + 2] = _zDepthLayer;
            _vertices[i + 3] = texCoords.MaxX;
            _vertices[i + 4] = texCoords.MaxY;
            _vertices[i + 5] = 1.0f;
            _vertices[i + 6] = 1.0f;
            _vertices[i + 7] = 1.0f;
            _vertices[i + 8] = texIndex;

            _vertices[i + 9] = rect.Max.X;
            _vertices[i + 10] = rect.Min.Y;
            _vertices[i + 11] = _zDepthLayer;
            _vertices[i + 12] = texCoords.MaxX;
            _vertices[i + 13] = texCoords.MinY;
            _vertices[i + 14] = 1.0f;
            _vertices[i + 15] = 1.0f;
            _vertices[i + 16] = 1.0f;
            _vertices[i + 17] = texIndex;

            _vertices[i + 18] = rect.Min.X;
            _vertices[i + 19] = rect.Min.Y;
            _vertices[i + 20] = _zDepthLayer;
            _vertices[i + 21] = texCoords.MinX;
            _vertices[i + 22] = texCoords.MinY;
            _vertices[i + 23] = 1.0f;
            _vertices[i + 24] = 1.0f;
            _vertices[i + 25] = 1.0f;
            _vertices[i + 26] = texIndex;

            _vertices[i + 27] = rect.Min.X;
            _vertices[i + 28] = rect.Max.Y;
            _vertices[i + 29] = _zDepthLayer;
            _vertices[i + 30] = texCoords.MinX;
            _vertices[i + 31] = texCoords.MaxY;
            _vertices[i + 32] = 1.0f;
            _vertices[i + 33] = 1.0f;
            _vertices[i + 34] = 1.0f;
            _vertices[i + 35] = texIndex;

            _vertexCount += 4;
            _indexCount += 6;
            _quadCount++;
        }

        private void CheckGLError(string location) {
            ErrorCode err = GL.GetError();
            if (err != ErrorCode.NoError) {
                _logger.LogError("OpenGL Error at {Location}: {ErrorCode}", location, err);
            }
            return;
        }
    }
}
