using FeloxGame.Rendering;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using SixLabors.ImageSharp;

namespace FeloxGame.UtilityClasses
{
    public static class TextureFactory
    {
        public static Texture2D Load(string textureName, int texUnit)
        {
            int handle = GL.GenTexture();

            if (texUnit > GL.GetInteger(GetPName.MaxTextureImageUnits))
            {
                throw new Exception($"Exceeded maximum texture slots OpenGL can natively support. Count {texUnit}");
            }
            TextureUnit textureUnit = ((TextureUnit)texUnit);

            GL.ActiveTexture(textureUnit); // Set the texture units
            GL.BindTexture(TextureTarget.Texture2D, handle); // Bind our texture
            using var image = Image.Load<Rgba32>(textureName);

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

            return new Texture2D(handle, image.Width, image.Height, textureUnit);
        }
    }
}
