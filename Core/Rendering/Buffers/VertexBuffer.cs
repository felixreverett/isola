using OpenTK.Graphics.OpenGL4;

namespace FeloxGame.Core.Rendering
{
    public class VertexBuffer : IBuffer
    {
        public int BufferId { get; }

        public VertexBuffer(float[] vertices)
        {
            BufferId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw); // Give data

        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferId);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
