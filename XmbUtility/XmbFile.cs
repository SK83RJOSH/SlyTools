using SlyTools.Common.IO;
using System.Collections.Generic;
using System.Xml;

namespace SlyTools.XmbUtility.Types
{
    public class XmbFile : IXmlSerializable
    {
        private string header = "XMLB";
        public XmbVersion Version = new XmbVersion();
        private string platform = "PSP2";
        private uint size;
        private ushort nodeCount;
        private List<XmbNode> nodes = new List<XmbNode>();

        public bool Read(BinaryStream stream)
        {
            if (!stream.ReadString(ref header, 4) || header != "XMLB")
            {
                return false;
            }

            if (!Version.Read(stream) || Version.Value != 0x0004)
            {
                return false;
            }

            if (!stream.Skip(2))
            {
                return false;
            }

            if (!stream.ReadString(ref platform, 4) || (platform != "PSP2" && platform != "PS3 "))
            {
                return false;
            }

            if (!stream.Read(ref size))
            {
                return false;
            }

            if (!stream.Read(ref nodeCount))
            {
                return false;
            }

            if (!stream.Skip(2))
            {
                return false;
            }

            nodes.Capacity = nodeCount;

            for (ushort i = 0; i < nodeCount; ++i)
            {
                using (BinaryStream node_stream = BinaryStream.CreateFromStream<uint>(stream))
                {
                    XmbNode node = new XmbNode();

                    if (!node.Read(node_stream))
                    {
                        return false;
                    }

                    nodes.Add(node);
                }
            }

            nodes.Capacity = nodeCount;

            return true;
        }

        public bool Write(BinaryStream stream)
        {
            return true;
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("xmb");
            Version.Write(writer);
            writer.WriteElementString("platform", platform);

            foreach (XmbNode node in nodes)
            {
                node.Write(writer);
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }
}
