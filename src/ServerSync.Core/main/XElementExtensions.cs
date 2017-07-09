using System;
using System.Linq;
using System.Xml.Linq;

namespace ServerSync.Core
{
    /// <summary>
    /// Extension methods for XElement
    /// </summary>
    static class XElementExtensions
    {
         
        /// <summary>
        /// Changes the namespace of every element in the currentNamespace in the element tree to newNamespace
        /// </summary>
        public static void ReplaceNamespace(this XElement element, XNamespace currentNamespace, XNamespace newNamespace)
        {
            if(element.Name.Namespace == currentNamespace)
            {
                element.Name = newNamespace.GetName(element.Name.LocalName);
            }

            foreach (var childNode in element.Elements())//.Where(el => el.Name.Namespace == currentNamespace))
            {
                childNode.ReplaceNamespace(currentNamespace, newNamespace);
            }
        }      

    }
}
