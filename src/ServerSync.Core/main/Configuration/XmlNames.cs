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
        public static readonly XName Actions = s_Namespace.GetName("actions");
        public static readonly XName AcquireLock = s_Namespace.GetName("acquireLock");
        public static readonly XName ReleaseLock = s_Namespace.GetName("releaseLock");
        public static readonly XName Sleep = s_Namespace.GetName("sleep");
        public static readonly XName Timeout = s_Namespace.GetName("timeout");
        public static readonly XName MicroscopeQuery = s_Namespace.GetName("microscopeQuery");
        public static readonly XName And = s_Namespace.GetName("and");
        public static readonly XName Or = s_Namespace.GetName("or");
        public static readonly XName Not = s_Namespace.GetName("not");
		public static readonly XName RunSyncJob = s_Namespace.GetName("runSyncJob");
        public static readonly XName UpdateTransferState = s_Namespace.GetName("updateTransferState");
        public static readonly XName InterimLocation = s_Namespace.GetName("interimLocation");
        public static readonly XName ExportDirectory = s_Namespace.GetName("exportDirectory");

        public static XNamespace GetNamespace()
        {
            return s_Namespace;
        }
    }
}
