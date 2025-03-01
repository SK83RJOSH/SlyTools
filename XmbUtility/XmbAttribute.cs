using SlyTools.Common.IO;
using SlyTools.Common.Hashing;
using System;
using System.Xml;

namespace SlyTools.XmbUtility.Types
{
    public enum XmbAttributeType
    {
        String,
        Integer,
        Float,
        Vector2,
        Vector3,
        RGBA,
        Custom,
        Invalid
    }

    public class XmbAttribute : IXmlSerializable
    {
        private XmbAttributeType type;
        private uint nameHash;
        private object value;

        public XmbAttribute(XmbAttributeType type)
        {
            this.type = type;
        }

        public bool Read(BinaryStream stream)
        {
            if (!stream.Read(ref nameHash))
            {
                return false;
            }

            using (BinaryStream attribute_stream = BinaryStream.CreateFromStream<uint>(stream))
            {
                switch (type)
                {
                    case XmbAttributeType.String:
                        {
                            XmbString string_value = new XmbString();
                            string_value.Read(attribute_stream);
                            value = string_value;
                            break;
                        }
                    case XmbAttributeType.Integer:
                        {
                            int integer_value = 0;

                            if (!attribute_stream.Read(ref integer_value))
                            {
                                return false;
                            }

                            value = integer_value;
                            break;
                        }
                    case XmbAttributeType.Float:
                        {
                            float float_value = 0;

                            if (!attribute_stream.Read(ref float_value))
                            {
                                return false;
                            }

                            value = float_value;
                            break;
                        }
                    case XmbAttributeType.Vector2:
                        {
                            XmbVector2 vector_value = new XmbVector2();
                            vector_value.Read(attribute_stream);
                            value = vector_value;
                            break;
                        }
                    case XmbAttributeType.Vector3:
                        {
                            XmbVector3 vector_value = new XmbVector3();
                            vector_value.Read(attribute_stream);
                            value = vector_value;
                            break;
                        }
                    case XmbAttributeType.Custom:
                        {
                            uint custom_value = 0;

                            if (!attribute_stream.Read(ref custom_value))
                            {
                                return false;
                            }

                            value = custom_value;
                            break;
                        }
                    case XmbAttributeType.RGBA:
                        {
                            XmbRGBA rgba_value = new XmbRGBA();
                            rgba_value.Read(attribute_stream);
                            value = rgba_value;
                            break;
                        }
                    default:
                        return false;
                }
            }

            return true;
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("attribute");
            writer.WriteAttributeString("type", type.ToString());

            if (HashDatabase.LookUp(nameHash, out string name))
            {
                writer.WriteAttributeString("name", name);
            }
            else
            {
                writer.WriteAttributeString("name-hash", $"0x{nameHash.ToString("X8")}");
            }

            switch (type)
            {
                case XmbAttributeType.String:
                case XmbAttributeType.Vector2:
                case XmbAttributeType.Vector3:
                case XmbAttributeType.RGBA:
                    ((IXmlSerializable)value).Write(writer);
                    break;
                case XmbAttributeType.Float:
                case XmbAttributeType.Integer:
                case XmbAttributeType.Custom:
                    writer.WriteElementString("value", value.ToString());
                    break;
                default:
                    throw new Exception("Failed to write attibute value!");
            }

            writer.WriteEndElement();
        }
    }
}
