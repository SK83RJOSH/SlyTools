using System;

namespace SlyTools.SanUtility
{
    class FormChunkProcessor : ChunkProcessor
    {
        public override string ChunkType => "FORM";

        public override long ProcessChunk(in ChunkProcessorInput input)
        {
            Console.WriteLine($"FORM: {input.Size}");
            return GetReadSize(input);
        }
    }

    class ProfChunkProcessor : ChunkProcessor
    {
        public override string ChunkType => "PROF";

        public override long ProcessChunk(in ChunkProcessorInput input)
        {
            return GetReadSize(input);
        }
    }

    class PrfnChunkProcessor : ChunkProcessor
    {
        public override string ChunkType => "PRFN";

        public override long ProcessChunk(in ChunkProcessorInput input)
        {
            string name = "";
            Read(input, ref name, "name", GetRemainingSize(input));
            Console.WriteLine($"PRFN: {name}");
            return GetReadSize(input);
        }
    }
}
