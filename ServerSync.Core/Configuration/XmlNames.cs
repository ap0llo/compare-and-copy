using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSync.Core.Configuration
{
    class XmlNames
    {
        static readonly XNamespace s_Namespace = "http://grynwald.net/schemas/2014/ServerSync/v1/Configuration/";

        public static readonly XName Left = s_Namespace.GetName("left");
        public static readonly XName Right = s_Namespace.GetName("right");
        public static readonly XName TimeStampMargin = s_Namespace.GetName("timeStampMargin");
        public static readonly XName Regex = s_Namespace.GetName("regex");
        public static readonly XName Filter = s_Namespace.GetName("filter");
        public static readonly XName Include = s_Namespace.GetName("include");
        public static readonly XName Exclude = s_Namespace.GetName("exclude");
        public static readonly XName CompareState = s_Namespace.GetName("compareState");
        public static readonly XName TransferState = s_Namespace.GetName("transferState");
        public static readonly XName MaxTransferSize = s_Namespace.GetName("maxTransferSize");
        public static readonly XName MaxTransferSizeParent = s_Namespace.GetName("maxTransferSizeParent");
        public static readonly XName Compare = s_Namespace.GetName("compare");
        public static readonly XName Export = s_Namespace.GetName("export");
        public static readonly XName Import = s_Namespace.GetName("import");
        public static readonly XName ReadSyncState = s_Namespace.GetName("readSyncState");
        public static readonly XName WriteSyncState = s_Namespace.GetName("writeSyncState");
        public static readonly XName ApplyFilter = s_Namespace.GetName("applyFilter");
        public static readonly XName Copy = s_Namespace.GetName("copy");
        public static readonly XName MaximumSize = s_Namespace.GetName("maximumSize");
        public static readonly XName TransferLocation = s_Namespace.GetName("transferLocation");
        
        public static XNamespace GetNamespace()
        {
            return s_Namespace;
        }

    }
}
