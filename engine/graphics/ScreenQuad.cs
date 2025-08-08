using Isola.Drawing;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isola.engine.graphics
{
    public class ScreenQuad
    {
        private readonly float[] _vertices =
        {
            // Positions    // Texture Coordinates
            -1.0f,  1.0f,   0.0f, 1.0f, // top-left
            -1.0f, -1.0f,   0.0f, 0.0f, // bottom-left
             1.0f, -1.0f,   1.0f, 0.0f, // bottom-right
             1.0f,  1.0f,   1.0f, 1.0f  // top-right
        };

        private uint[] _indices =
        {
            0, 1, 2, // First triangle
            0, 2, 3  // Second triangle
        };

        private VertexArray _vao;
        private VertexBuffer _vbo;
        private IndexBuffer _ebo;

        public ScreenQuad()
        {
            _vao = new VertexArray();
            _vao.Bind();

            _vbo = new VertexBuffer(_vertices);
            _ebo = new IndexBuffer(_indices);

            var layout = new BufferLayout();
            layout.Add<float>(2);
            layout.Add<float>(2);
            _vao.AddBuffer(_vbo, layout);
            _ebo.Bind();

            _vao.Unbind();
        }

        public void Draw()
        {
            // Bind the vertex array and draw the quad
            _vao.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            _vao.Unbind();
        }
    }
}
