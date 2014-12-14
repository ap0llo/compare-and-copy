using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSync.Core.State
{
    class XmlNames
    {

        static readonly XNamespace s_Namespace = "http://grynwald.net/schemas/2014/ServerSync/v1/SyncState/";


        public static readonly XName FileList = s_Namespace.GetName("fileList");
        public static readonly XName File = s_Namespace.GetName("file");



        public static XNamespace GetNamespace()
        {
            return s_Namespace;
        }
    }
}
