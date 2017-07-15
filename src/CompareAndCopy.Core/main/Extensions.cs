using System;
using System.IO;
using System.Xml.Linq;
using ByteSizeLib;

namespace CompareAndCopy.Core
{
    static class Extensions
    {
        public static string RequireAttributeValue(this XElement element, XName attributeName)
        {
            var attribute = element.Attribute(attributeName);

            if(attribute == null)
            {
                throw new ArgumentException("Attribute not found " + attributeName);
            }

            if(String.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new ArgumentException("Empty value for attribute " + attributeName);
            }

            return attribute.Value;
        }

        public static long ReadLongAttributeValueOrDefault(this XElement element, XName attributeName)
        {
            var attribute = element.Attribute(attributeName);
            return attribute == null ? 0 : long.Parse(attribute.Value);
        }
        
        public static ByteSize GetByteSize(this FileInfo fileInfo)
        {
            return ByteSize.FromBytes(fileInfo.Length);
        }
    }
}
