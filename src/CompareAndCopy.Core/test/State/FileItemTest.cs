using CompareAndCopy.Core.State;
using CompareAndCopy.Model.State;
using Xunit;

namespace CompareAndCopy.Core.Test.State
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

            var fileItem1 = new FileItem("relative/path", new TransferState(TransferDirection.None)) { CompareState = CompareState.Conflict };
            var fileItem2 = new FileItem("relative/Path", new TransferState(TransferDirection.None)) { CompareState = CompareState.Conflict };
            var fileItem3 = new FileItem("RELATIVE\\path", new TransferState(TransferDirection.None)) { CompareState = CompareState.MissingLeft };
            var fileItem4 = new FileItem("relative/path", new TransferState(TransferDirection.InTransferToLeft)) { CompareState = CompareState.Conflict, };
            var fileItem5 = new FileItem("RELATIVE\\path", new TransferState(TransferDirection.InTransferToLeft)) { CompareState = CompareState.Conflict };
            var fileItem6 = new FileItem("RELATIVE\\path", new TransferState(TransferDirection.InTransferToRight)) { CompareState = CompareState.Conflict };
            var fileItem7 = new FileItem("relative///path", new TransferState(TransferDirection.None)) { CompareState = CompareState.Conflict};


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
