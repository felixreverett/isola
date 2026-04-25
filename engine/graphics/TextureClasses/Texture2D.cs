using OpenTK.Graphics.OpenGL4;
using Microsoft.Extensions.Logging;

namespace Isola.Drawing {
    public class Texture2D : IDisposable {
        private bool _disposed;
        public int Handle { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TextureUnit TextureSlot { get; set; } = TextureUnit.Texture0;

        private readonly ILogger<Texture2D>? _logger;

        public Texture2D(int handle, int width, int height, TextureUnit textureSlot, ILogger<Texture2D>? logger = null) {
            TextureSlot = textureSlot;
            Width = width;
            Height = height;
            Handle = handle;
            _logger = logger;
            _logger?.LogDebug($"Created Texture2D (Handle: {Handle}, Slot: {TextureSlot}, Size: {Width}x{Height})");
        }        

        ~Texture2D() {
            Dispose(false);
        }
        
        public void Use() {

            if ((int)TextureSlot < (int)TextureUnit.Texture0 || (int)TextureSlot > (int)TextureUnit.Texture31) {
                _logger?.LogError($"Invalid TextureSlot value: {TextureSlot}");
                return;
            }

            GL.ActiveTexture(TextureSlot);

            var err = GL.GetError();

            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError) _logger?.LogError("GL Error in Texture2D.Use() after GL.ActivateTexture(): " + err);

            GL.BindTexture(TextureTarget.Texture2D, Handle);

            err = GL.GetError();

            if (err != OpenTK.Graphics.OpenGL4.ErrorCode.NoError) _logger?.LogError("GL Error in Texture2D.Use() after GL.BindTexture(): " + err);

            return;
        }

        public void Dispose(bool disposing) {
            if (!_disposed) {
                GL.DeleteTexture(Handle);
                _disposed = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
