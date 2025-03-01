using SlyTools.Common.IO;
using System;

namespace SlyTools.SanUtility
{
    public struct ChunkProcessorInput
    {
        public BinaryStream Stream { get; set; }
        public long Position { get; set; }
        public uint Size { get; set; }
    }

    public abstract class ChunkProcessor
    {
        public abstract string ChunkType { get; }
        public abstract long ProcessChunk(in ChunkProcessorInput input);

        protected void Read<T>(in ChunkProcessorInput input, ref T value, string name) where T : unmanaged
        {
            if (!input.Stream.Read(ref value))
            {
                throw new Exception($"Unabled to read value '{name}' of chunk type '{ChunkType}'!");
            }
        }

        protected void Read(in ChunkProcessorInput input, ref string value, string name, long size = 32)
        {
            if (!input.Stream.ReadString(ref value, size))
            {
                throw new Exception($"Unable to read value '{name}' of chunk type '{ChunkType}'!");
            }
        }

        protected void Read(in ChunkProcessorInput input, ref byte[] value, string name)
        {
            if (!input.Stream.Read(ref value))
            {
                throw new Exception($"Unable to read value '{name}' of chunk type '{ChunkType}'!");
            }
        }

        protected long GetReadSize(in ChunkProcessorInput input)
        {
            return input.Stream.Position - input.Position;
        }

        protected long GetRemainingSize(in ChunkProcessorInput input)
        {
            return (input.Position + input.Size) - input.Stream.Position;
        }
    }
}
