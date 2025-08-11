using OpenTK.Graphics.OpenGL4;

namespace Isola.Drawing
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
            //Console.WriteLine($"===== Texture2D.Use() on unit {TextureSlot - TextureUnit.Texture0} with texture handle {Handle}");

            if ((int)TextureSlot < (int)TextureUnit.Texture0 || (int)TextureSlot > (int)TextureUnit.Texture31)
            {
                Console.WriteLine($"Invalid TextureSlot value: {TextureSlot}");
                return;
            }
            GL.ActiveTexture(TextureSlot);

            var err = GL.GetError();
            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                Console.WriteLine("GL Error in Use() after GL.ActivateTexture(): " + err);
            }

            GL.BindTexture(TextureTarget.Texture2D, Handle);

            err = GL.GetError();
            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                Console.WriteLine("GL Error in Use() after GL.BindTexture(): " + err);
            }
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
