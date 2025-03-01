using SlyTools.Common.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SlyTools.SanUtility
{
    public class ChunkReader
    {
        private Dictionary<string, ChunkProcessor> processors = new Dictionary<string, ChunkProcessor>();

        public ChunkReader()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(ChunkProcessor)))
                    {
                        ChunkProcessor processor = (ChunkProcessor)Activator.CreateInstance(type);

                        if (processors.ContainsKey(processor.ChunkType))
                        {
                            throw new Exception($"Multiple chunk processors found for '{processor.ChunkType}'!");
                        }

                        processors.Add(processor.ChunkType, processor);
                    }
                }
            }
        }

        public void ProcessStream(BinaryStream stream)
        {
            ChunkProcessorInput parameters = new ChunkProcessorInput();

            while (stream.CanRead())
            {
                string chunk_type = "";

                if (!stream.ReadString(ref chunk_type, 4))
                {
                    throw new Exception("Unable to read chunk type!");
                }

                using (BinaryStream chunk_stream = BinaryStream.CreateFromStreamOffset(stream, stream.Position))
                {
                    uint chunk_size = 0;

                    if (!chunk_stream.Read(ref chunk_size))
                    {
                        throw new Exception("Unable to read chunk size!");
                    }

                    if (processors.TryGetValue(chunk_type, out ChunkProcessor processor))
                    {
                        parameters.Size = chunk_size;
                        parameters.Position = stream.Position;
                        parameters.Stream = chunk_stream;
                        stream.Skip(processor.ProcessChunk(parameters));
                    }
                    else
                    {
                        Console.WriteLine($"Skipping unknown chunk type '{chunk_type}'.");
                        stream.Skip(chunk_size);
                    }
                }
            }
        }
    }
}
