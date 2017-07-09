﻿using System.Xml.Linq;

namespace ServerSync.Core.State
{
    class XmlNames
    {
        static readonly XNamespace s_Namespace = "http://grynwald.net/schemas/2014/ServerSync/v1/SyncState/";

        public static readonly XName FileList = s_Namespace.GetName("fileList");
        public static readonly XName File = s_Namespace.GetName("file");
        public static readonly XName Location = s_Namespace.GetName("location");

        public static XNamespace GetNamespace() => s_Namespace;
    }
}
