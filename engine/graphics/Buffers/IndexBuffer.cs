using OpenTK.Graphics.OpenGL4;

namespace Isola.Drawing
{
    public class IndexBuffer : IBuffer
    {
        public int BufferId { get; }

        public IndexBuffer(uint[] indices)
        {
            BufferId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, BufferId); // Bind buffer
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw); // Give data
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, BufferId);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
