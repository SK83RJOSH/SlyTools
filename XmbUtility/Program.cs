using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using SlyTools.Common.IO;
using SlyTools.Common.Hashing;
using SlyTools.XmbUtility.Types;

namespace SlyTools.XmbUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            HashDatabase.LoadFile(Path.Join(location, "strings.txt"));

            if (args.Length != 1)
            {
                throw new Exception("Only one argument may be supplied.");
            }

            if (!File.GetAttributes(args[0]).HasFlag(FileAttributes.Directory))
            {
                throw new Exception("Argument must be a directory.");
            }

            string root_directory = args[0];
            string output_directory = root_directory + "Dump";
            XmlWriterSettings writer_settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.ASCII };

            foreach (string input_path in Directory.EnumerateFiles(root_directory, "*.xmb", SearchOption.AllDirectories))
            {
                using (BinaryStream stream = new BinaryStream(input_path, "r", FileEndian.Little))
                {
                    XmbFile xmb = new XmbFile();

                    if (!xmb.Read(stream))
                    {
                        throw new Exception("Failed to read XMB file.");
                    }
                    else
                    {
                        string output_path = Path.Join(output_directory, input_path.Remove(0, root_directory.Length + 1).Replace(".xmb", ".xml"));

                        Directory.CreateDirectory(Path.GetDirectoryName(output_path));

                        using (XmlWriter writer = XmlWriter.Create(output_path, writer_settings))
                        {
                            xmb.Write(writer);
                        }
                    }
                }
            }

            HashDatabase.WriteDiagnostics(output_directory);
        }
    }
}
