using SlyTools.Common.IO;
using System.Xml;

namespace SlyTools.XmbUtility.Types
{
    public class XmbVersion : IXmlSerializable
    {
        private ushort value = 0x0004;
        public ushort Value => value;
        public char Major => (char)(Value >> 0 & 0xF);
        public char Minor => (char)(Value >> 4 & 0xF);
        public char Revision => (char)(Value >> 8 & 0xF);
        public char Patch => (char)(Value >> 12 & 0xF);

        public bool Read(BinaryStream stream)
        {
            return stream.Read(ref value);
        }

        public bool Write(BinaryStream stream)
        {
            return stream.Write(value);
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("version");
            writer.WriteElementString("major", ((int)Major).ToString());
            writer.WriteElementString("minor", ((int)Minor).ToString());
            writer.WriteElementString("revision", ((int)Revision).ToString());
            writer.WriteElementString("patch", ((int)Patch).ToString());
            writer.WriteEndElement();
        }
    }
}
