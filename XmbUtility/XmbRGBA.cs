using SlyTools.Common.IO;
using System;
using System.Xml;

namespace SlyTools.XmbUtility.Types
{
    public class XmbRGBA : IXmlSerializable
    {
        private byte r;
        private byte g;
        private byte b;
        private byte a;

        public void Read(BinaryStream stream)
        {
            if (!stream.Read(ref r))
            {
                throw new Exception("Failed to read R!");
            }

            if (!stream.Read(ref g))
            {
                throw new Exception("Failed to read G!");
            }

            if (!stream.Read(ref b))
            {
                throw new Exception("Failed to read B!");
            }

            if (!stream.Read(ref a))
            {
                throw new Exception("Failed to read A!");
            }
        }

        public void Write(BinaryStream stream)
        {
            if (!stream.Write(r))
            {
                throw new Exception("Failed to write R!");
            }

            if (!stream.Write(g))
            {
                throw new Exception("Failed to write G!");
            }

            if (!stream.Write(b))
            {
                throw new Exception("Failed to write B!");
            }

            if (!stream.Write(a))
            {
                throw new Exception("Failed to write A!");
            }
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteElementString("r", r.ToString());
            writer.WriteElementString("g", g.ToString());
            writer.WriteElementString("b", b.ToString());
            writer.WriteElementString("a", a.ToString());
        }
    }
}
