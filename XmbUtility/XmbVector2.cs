using SlyTools.Common.IO;
using System;
using System.Xml;

namespace SlyTools.XmbUtility.Types
{
    public class XmbVector2 : IXmlSerializable
    {
        private float x;
        private float y;

        public void Read(BinaryStream stream)
        {
            if (!stream.Read(ref x))
            {
                throw new Exception("Failed to read X!");
            }

            if (!stream.Read(ref y))
            {
                throw new Exception("Failed to read Y!");
            }
        }

        public void Write(BinaryStream stream)
        {
            if (!stream.Write(x))
            {
                throw new Exception("Failed to write X!");
            }

            if (!stream.Write(y))
            {
                throw new Exception("Failed to write Y!");
            }
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteElementString("x", x.ToString());
            writer.WriteElementString("y", y.ToString());
        }
    }
}
