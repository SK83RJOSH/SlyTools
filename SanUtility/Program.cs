using SlyTools.Common.IO;
using SlyTools.SanUtility;
using System.IO;
using System;

namespace SkyTools.SanUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Only one argument may be supplied.");
            }

            if (File.GetAttributes(args[0]).HasFlag(FileAttributes.Directory))
            {
                throw new Exception("Argument must be a file.");
            }

            if (!args[0].EndsWith(".san.cooked"))
            {
                throw new Exception("File must be of extension .san.cooked");
            }

            ChunkReader chunk_reader = new ChunkReader();

            using (BinaryStream stream = new BinaryStream(args[0], "r", FileEndian.Little))
            {
                chunk_reader.ProcessStream(stream);
            }
        }
    }
}
