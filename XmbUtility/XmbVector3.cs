using SlyTools.Common.IO;
using System;
using System.Xml;

namespace SlyTools.XmbUtility.Types
{
    public class XmbVector3 : IXmlSerializable
    {
        private float x;
        private float y;
        private float z;

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

            if (!stream.Read(ref z))
            {
                throw new Exception("Failed to read Z!");
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

            if (!stream.Write(z))
            {
                throw new Exception("Failed to write Z!");
            }
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteElementString("x", x.ToString());
            writer.WriteElementString("y", y.ToString());
            writer.WriteElementString("z", z.ToString());
        }
    }
}
