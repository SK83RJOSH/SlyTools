using SlyTools.Common.IO;
using SlyTools.Common.Hashing;
using System.Collections.Generic;
using System.Xml;

namespace SlyTools.XmbUtility.Types
{
    public class XmbNode : IXmlSerializable
    {
        private uint nameHash;
        private string value;
        private uint parentHash;
        private short nodeCount;
        private short attributeCount;
        private List<XmbNode> nodes = new List<XmbNode>();
        private List<XmbAttribute> attributes = new List<XmbAttribute>();

        public bool Read(BinaryStream stream)
        {
            if (!stream.Read(ref nameHash))
            {
                return false;
            }

            using (BinaryStream string_stream = BinaryStream.CreateFromStream<uint>(stream, true))
            {
                if (string_stream != null && !string_stream.ReadString(ref value))
                {
                    return false;
                }
            }

            using (BinaryStream hash_stream = BinaryStream.CreateFromStream<uint>(stream, true))
            {
                if (hash_stream != null && !hash_stream.Read(ref parentHash))
                {
                    return false;
                }
            }

            if (!stream.Read(ref nodeCount))
            {
                return false;
            }

            if (!stream.Read(ref attributeCount))
            {
                return false;
            }

            using BinaryStream nodes_stream = BinaryStream.CreateFromStream<uint>(stream);

            if (nodes_stream == null)
            {
                return false;
            }

            using BinaryStream attributes_stream = BinaryStream.CreateFromStream<uint>(stream);

            if (attributes_stream == null)
            {
                return false;
            }

            int[] attribute_type_array = new int[(attributeCount / 8) + (attributeCount % 8 == 0 ? 0 : 1)];

            for (int i = 0; i < attribute_type_array.Length; ++i)
            {
                if (!stream.Read(ref attribute_type_array[i]))
                {
                    return false;
                }
            }

            nodes.Capacity = nodeCount;

            for (ushort i = 0; i < nodeCount; ++i)
            {
                using (BinaryStream node_stream = BinaryStream.CreateFromStream<uint>(nodes_stream))
                {
                    XmbNode node = new XmbNode();

                    if (!node.Read(node_stream))
                    {
                        return false;
                    }

                    nodes.Add(node);
                }
            }

            for (ushort i = 0; i < attributeCount; ++i)
            {
                XmbAttributeType attribute_type = (XmbAttributeType)(((attribute_type_array[i >> 3]) >> (4 * (i & 7))) & 0xF);
                XmbAttribute attribute = new XmbAttribute(attribute_type);

                if (!attribute.Read(attributes_stream))
                {
                    return false;
                }

                attributes.Add(attribute);
            }

            return true;
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("node");

            if (HashDatabase.LookUp(nameHash, out string name))
            {
                writer.WriteAttributeString("name", name);
            }
            else
            {
                writer.WriteAttributeString("name-hash", $"0x{nameHash.ToString("X8")}");
            }

            if (parentHash != 0)
            {
                if (HashDatabase.LookUp(parentHash, out string value))
                {
                    writer.WriteAttributeString("parent", value);
                }
                else
                {
                    writer.WriteAttributeString("parent-hash", $"0x{parentHash.ToString("X8")}");
                }
            }

            if (value != null)
            {
                writer.WriteElementString("value", value.ToString());
            }

            foreach (XmbAttribute attribute in attributes)
            {
                attribute.Write(writer);
            }

            foreach (XmbNode node in nodes)
            {
                node.Write(writer);
            }

            writer.WriteEndElement();
        }
    }
}
