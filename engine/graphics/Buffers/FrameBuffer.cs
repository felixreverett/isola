using OpenTK.Graphics.OpenGL4;

namespace Isola.engine.graphics.Buffers
{
    public class FrameBuffer
    {
        private int _fboHandle;
        private int _textureHandle;
        private int _renderbufferHandle;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public FrameBuffer(int width, int height)
        {
            Width = width;
            Height = height;

            _fboHandle = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboHandle);

            _textureHandle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _textureHandle, 0);

            // Check for completeness
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                // Handle error
                Console.WriteLine($"Error: Framebuffer is not complete. Status: {status}. (FrameBuffer.cs)");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Use()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboHandle);
            GL.Viewport(0, 0, Width, Height);
        }

        public void BindTexture(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
        }
    }
}
