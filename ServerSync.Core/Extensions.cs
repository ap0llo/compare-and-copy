using ServerSync.Core.Filters;
using ServerSync.Core.State;
using ServerSync.Model;
using ServerSync.Model.Filtering;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace ServerSync.Core
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

            if(attribute == null)
            {
                return 0;
            }
            else
            {
                return long.Parse(attribute.Value);
            }

        }



        public static IEnumerable<IFileItem> ApplyFilter(this IEnumerable<IFileItem> items, IFilter filter)
        {
            return filter.ApplyFilter(items);
        }



        public static ByteSize.ByteSize GetByteSize(this FileInfo fileInfo)
        {
            return ByteSize.ByteSize.FromBytes(fileInfo.Length);
        }

    }
}
