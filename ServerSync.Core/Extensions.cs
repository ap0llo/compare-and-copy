using ServerSync.Core.Filters;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
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



        public static IEnumerable<FileItem> ApplyFilter(this IEnumerable<FileItem> items, Filter filter)
        {
            return items.Where(item => filter.IncludeRules.Any(rule => rule.IsMatch(item)))
                        .Where(item => ! filter.ExcludeRules.Any(rule => rule.IsMatch(item)));
        }


    }
}
