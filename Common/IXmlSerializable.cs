using System.Xml;

namespace SlyTools.Common.IO
{
    public interface IXmlSerializable
    {
        //public void Read(XmlReader reader);
        public void Write(XmlWriter writer);
    }
}
