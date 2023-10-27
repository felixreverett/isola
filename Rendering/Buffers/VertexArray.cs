using FeloxGame.UtilityClasses;
using OpenTK.Graphics.OpenGL4;

namespace FeloxGame.Rendering
{
    public class VertexArray : IBuffer
    {
        public int BufferId { get; }

        public VertexArray()
        {
            BufferId = GL.GenVertexArray(); // Gen vertex array
        }

        ~VertexArray()
        {
            GL.DeleteVertexArray(BufferId);
        }

        public void AddBuffer(VertexBuffer vertexBuffer, BufferLayout bufferLayout) // Add the vertex buffer data
        {
            Bind();
            vertexBuffer.Bind();
            var elements = bufferLayout.GetBufferElements();
            int offset = 0;
            for (int i = 0; i < elements.Count(); i++)
            {
                var currentElement = elements[i];
                GL.EnableVertexAttribArray(i);
                GL.VertexAttribPointer(i, currentElement.Count, currentElement.Type, currentElement.Normalized, bufferLayout.GetStride(), offset);
                offset += currentElement.Count * Utilities.GetSizeOfVertexAttribPointerType(currentElement.Type);
            }
        }

        public void Bind()
        {
            GL.BindVertexArray(BufferId);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }
    }
}
