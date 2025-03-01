using System.Collections.Generic;

namespace SlyTools.SanUtility
{
    public class TextureHeader
    {
        public byte Version;
        public uint NameHash;
        public ushort Width;
        public ushort Height;
        public byte Depth;
        public byte BitsPerPixel;
        public byte Type;
        public string Name;
        public uint PackBufferOffset;
        public uint PackBufferSize;
    }

    public class Texture
    {
        public TextureHeader Header;
        public byte[] Data;
    }

    public static class TextureManager
    {
        private static Texture texture = null;
        private static List<Texture> textures = new List<Texture>();

        public static void CreateTexture()
        {
            texture = new Texture();
            textures.Add(texture);
        }

        public static void SetTextureHeader(TextureHeader header)
        {
            texture.Header = header;
        }

        public static void SetTextureData(byte[] data)
        {
            texture.Data = data;
        }

        public static IReadOnlyCollection<Texture> GetTextures()
        {
            return textures;
        }
    }
}
