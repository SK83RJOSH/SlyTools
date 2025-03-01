namespace SlyTools.SanUtility.Chunks
{
    public class TexrChunkProcessor : ChunkProcessor
    {
        public override string ChunkType => "TEXR";

        public override long ProcessChunk(in ChunkProcessorInput input)
        {
            TextureManager.CreateTexture();
            return GetReadSize(input);
        }
    }

    public class TxrhChunkProcessor : ChunkProcessor
    {
        public override string ChunkType => "TXRH";

        public override long ProcessChunk(in ChunkProcessorInput input)
        {
            TextureHeader header = new TextureHeader();

            Read(input, ref header.Version, "Version");
            Read(input, ref header.NameHash, "NameHash");
            Read(input, ref header.Width, "Width");
            Read(input, ref header.Height, "Height");
            Read(input, ref header.Depth, "Depth");
            Read(input, ref header.BitsPerPixel, "BitsPerPixel");
            Read(input, ref header.Type, "Type");
            Read(input, ref header.Name, "Name");

            if (header.Version > 0)
            {
                Read(input, ref header.PackBufferOffset, "PackBufferOffset");
                Read(input, ref header.PackBufferSize, "PackBufferSize");
            }

            TextureManager.SetTextureHeader(header);
            return input.Size;
        }
    }

    public class TgxtChunkProcessor : ChunkProcessor
    {
        public override string ChunkType => "TGXT";

        public override long ProcessChunk(in ChunkProcessorInput input)
        {
            uint size = input.Size - 4;
            byte[] data = new byte[size];

            Read(input, ref data, "data");

            TextureManager.SetTextureData(data);
            return input.Size;
        }
    }
}
