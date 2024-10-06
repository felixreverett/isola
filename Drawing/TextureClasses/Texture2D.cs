using OpenTK.Graphics.OpenGL4;

namespace FeloxGame.Drawing
{
    public class Texture2D : IDisposable
    {
        private bool _disposed;
        public int Handle { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TextureUnit TextureSlot { get; set; } = TextureUnit.Texture0;

        public Texture2D(int handle, int width, int height, TextureUnit textureSlot)
        {
            TextureSlot = textureSlot;
            Width = width;
            Height = height;
            Handle = handle;
        }        

        ~Texture2D()
        {
            Dispose(false);
        }
        
        public void Use()
        {
            GL.ActiveTexture(TextureSlot);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                GL.DeleteTexture(Handle);
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
