﻿using ServerSync.Core.State;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ServerSync.Core.Test.State
{
    public class FileItemTest
    {


        [Fact]
        public void Test_GetHashCode()
        {
            var fileItem1 = new FileItem("relative/path");
            var fileItem2 = new FileItem("relative/Path");
            var fileItem3 = new FileItem("RELATIVE\\path");

            Assert.Equal(fileItem1.GetHashCode(), fileItem2.GetHashCode());
            Assert.Equal(fileItem1.GetHashCode(), fileItem3.GetHashCode());             
        }


        [Fact]
        public void Test_Equals()
        {

            var fileItem1 = new FileItem("relative/path") { CompareState = CompareState.Conflict, TransferState = TransferState.None };
            var fileItem2 = new FileItem("relative/Path") { CompareState = CompareState.Conflict, TransferState = TransferState.None };
            var fileItem3 = new FileItem("RELATIVE\\path") { CompareState = CompareState.MissingLeft, TransferState = TransferState.None };
            var fileItem4 = new FileItem("relative/path") { CompareState = CompareState.Conflict, TransferState = TransferState.InTransferToLeft };
            var fileItem5 = new FileItem("RELATIVE\\path") { CompareState = CompareState.Conflict, TransferState = TransferState.InTransferToLeft };
            var fileItem6 = new FileItem("RELATIVE\\path") { CompareState = CompareState.Conflict, TransferState = TransferState.InTransferToRight };
            var fileItem7 = new FileItem("relative///path") { CompareState = CompareState.Conflict, TransferState = TransferState.None };


            Assert.True(fileItem1.Equals(fileItem1));
            Assert.True(fileItem2.Equals(fileItem2));
            Assert.True(fileItem3.Equals(fileItem3));
            Assert.True(fileItem4.Equals(fileItem4));
            Assert.True(fileItem5.Equals(fileItem5));

            Assert.True(fileItem1.Equals(fileItem1));
            Assert.True(fileItem4.Equals(fileItem5));
            Assert.True(fileItem1.Equals(fileItem7));
            Assert.False(fileItem1.Equals(fileItem3));
            Assert.False(fileItem1.Equals(fileItem4));
            Assert.False(fileItem1.Equals(fileItem5));
            Assert.False(fileItem5.Equals(fileItem6));

            Assert.Equal(fileItem1.Equals(fileItem2), fileItem2.Equals(fileItem1));
            Assert.Equal(fileItem1.Equals(fileItem3), fileItem3.Equals(fileItem1));
            Assert.Equal(fileItem1.Equals(fileItem4), fileItem4.Equals(fileItem1));
            Assert.Equal(fileItem1.Equals(fileItem5), fileItem5.Equals(fileItem1));

            Assert.Equal(fileItem2.Equals(fileItem3), fileItem3.Equals(fileItem2));
            Assert.Equal(fileItem2.Equals(fileItem4), fileItem4.Equals(fileItem2));
            Assert.Equal(fileItem2.Equals(fileItem5), fileItem5.Equals(fileItem2));

            Assert.Equal(fileItem3.Equals(fileItem4), fileItem4.Equals(fileItem3));
            Assert.Equal(fileItem3.Equals(fileItem5), fileItem5.Equals(fileItem3));

        }


    }
}
