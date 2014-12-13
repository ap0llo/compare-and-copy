﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSync.Core.State
{
    public class SyncStateWriter
    {

        #region Public Methods

        public void WriteSyncState(string fileName, SyncState state)
        {        
            XElement fileList = new XElement(XmlConstants.FileList, state.Files.Select(GetFileItemElement));

            var document = new XDocument(new XElement(fileList));

            var dir = Path.GetDirectoryName(fileName);
            IOHelper.EnsureDirectoryExists(dir);

            document.Save(fileName);
        }

        #endregion 


        #region Private Implementation

        private XElement GetFileItemElement(FileItem file)
        {
            return new XElement( XmlConstants.File, 
                                 new XAttribute(XmlConstants.Path, file.RelativePath), 
                                 new XAttribute(XmlConstants.CompareState, file.CompareState),
                                 new XAttribute(XmlConstants.TransferState, file.TransferState));
        }

        #endregion Private Implementation

    }
}