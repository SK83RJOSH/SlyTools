using SlyTools.Common.IO;
using System;
using System.Xml;

namespace SlyTools.XmbUtility.Types
{
    public class XmbString : IXmlSerializable
    {
        private uint hash;
        private string value;

        public void Read(BinaryStream stream)
        {
            if (!stream.Read(ref hash))
            {
                throw new Exception("Failed to read string hash!");
            }

            if (!stream.ReadString(ref value))
            {
                throw new Exception("Failed to read string value!");
            }
        }

        public void Write(BinaryStream stream)
        {
            if (!stream.Write(hash))
            {
                throw new Exception("Failed to write string hash!");
            }

            //if (!file.WriteString(value))
            {
                throw new Exception("Failed to write string value!");
            }
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteElementString("value", value.ToString());
        }
    }
}
