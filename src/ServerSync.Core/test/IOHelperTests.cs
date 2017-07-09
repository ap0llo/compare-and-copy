using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ServerSync.Core.Test
{
    public class IOHelperTests
    {

        [Fact]
        public void Test_PathLeavesRoot_True()
        {
            Assert.True(IOHelper.PathLeavesRoot("", ".."));
            Assert.True(IOHelper.PathLeavesRoot("foo", ".."));
            Assert.True(IOHelper.PathLeavesRoot(@"C:\foo", ".."));
            Assert.True(IOHelper.PathLeavesRoot(@"C:\foo\bar", ".."));
            Assert.True(IOHelper.PathLeavesRoot(@"C:\foo", @"bar\..\..\"));
            Assert.True(IOHelper.PathLeavesRoot(@"C:\foo", @".\bar\..\..\"));
            Assert.True(IOHelper.PathLeavesRoot(@"C:\foo", @".\bar\..\.\..\"));

            Assert.True(IOHelper.PathLeavesRoot(@"C:\foo", @"C:\bar"));
        }


        [Fact]
        public void Test_PathLeavesRoot_False()
        {
            Assert.False(IOHelper.PathLeavesRoot("", "foo"));
            Assert.False(IOHelper.PathLeavesRoot("foo", "bar"));
            Assert.False(IOHelper.PathLeavesRoot("", "."));

            Assert.False(IOHelper.PathLeavesRoot("foo", @"..\foo"));
            Assert.False(IOHelper.PathLeavesRoot("foo", @"..\foo\..\foo"));

            Assert.False(IOHelper.PathLeavesRoot("foo", @"bar\.."));
            Assert.False(IOHelper.PathLeavesRoot("foo", @".\bar\.."));

            Assert.False(IOHelper.PathLeavesRoot(@"C:\foo", @"C:\foo\bar"));
        }

    }
}
