using FeloxGame.Core.Rendering;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using SixLabors.ImageSharp;

namespace FeloxGame.Core.Management
{
    public static class TextureFactory
    {
        private static int _textureCursor = 0;

        public static Texture2D Load(string textureName)
        {
            int handle = GL.GenTexture();
            Enum.TryParse(typeof(TextureUnit), $"Texture{_textureCursor}", out var result);
            if (result == null)
            {
                throw new Exception($"Exceeded maximum texture slots OpenGL can natively support. Count {_textureCursor}");
            }
            TextureUnit textureUnit = ((TextureUnit)result);

            GL.ActiveTexture(textureUnit); // Set the texture units
            GL.BindTexture(TextureTarget.Texture2D, handle); // Bind our texture
            using var image = Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(textureName);

            image.Mutate(i => i.RotateFlip(RotateMode.Rotate180, FlipMode.Horizontal));

            var data = image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, memory.ToArray());

            // Make our pixels "nearest"
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);

            // Wrapping (SGIS algorithm prevents edge sampling from neighbouring textures in the atlas
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Auto-Generate Mipmaps (probably won't need for 2D game)
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Increment cursor
            _textureCursor++;

            return new Texture2D(handle, image.Width, image.Height, textureUnit);
        }
    }
}
