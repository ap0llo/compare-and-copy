using Moq;
using ServerSync.Core.Configuration;
using ServerSync.Core.Copy;
using ServerSync.Core.PathResolving;
using ServerSync.Core.State;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace ServerSync.Core.Test.Configuration
{
    public class ConfigurationReaderTest
    {

        #region Constants

        const string s_Filter1 = "Filter1";
        const string s_Resource_TestFilter1 = "ServerSync.Core.Test.Configuration.TestData.TestFilter1.xml";
        const string s_Resource_ImplicitTransferLocation1 = "ServerSync.Core.Test.Configuration.TestData.ImplicitTransferLocation1.xml";
        const string s_Resource_ImplicitTransferLocation2 = "ServerSync.Core.Test.Configuration.TestData.ImplicitTransferLocation2.xml";
        const string s_Resource_ImplicitTransferLocation3 = "ServerSync.Core.Test.Configuration.TestData.ImplicitTransferLocation3.xml";

        #endregion


        #region Test Methods

        /// <summary>
        /// Not purely a unit test. 
        /// Tests if the configuration can read a configuration file defining a filter and if this filter
        /// works correctly to ensure filter is not setup wrong by the configuration reader
        /// </summary>
        [Fact]
        public void Test_Filter_1()
        {
            var pathResolverMock = new Mock<IPathResolver>();

            var input = LoadResource(s_Resource_TestFilter1);
            var configurationReader = new ConfigurationReader();


            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            pathResolverMock.Verify(r => r.GetAbsolutePath(It.IsAny<string>()), Times.Never());

            Assert.Equal(1, config.Filters.Count());
            Assert.DoesNotThrow(new Assert.ThrowsDelegate(() => config.GetFilter(s_Filter1)));

            var filter = config.GetFilter(s_Filter1);
            Assert.NotNull(filter);


            var filterInput = new List<IFileItem>()
            {
                new FileItem("file1.ext1") { CompareState = CompareState.Conflict},
                new FileItem("file1.ext2") { CompareState = CompareState.Conflict},
                new FileItem("file1.ext3") { CompareState = CompareState.Conflict},
                new FileItem("file2.ext1") { CompareState = CompareState.Conflict},
                new FileItem("file3.ext1") { CompareState = CompareState.MissingLeft},
                new FileItem("file4.ext1") { CompareState = CompareState.MissingRight},
                new FileItem("file3.ext3") { CompareState = CompareState.MissingRight},
            };

            var expectedOutput = new List<IFileItem>()
            {   
                filterInput.Skip(2).First()
                
            };

            var actualFilterOutput = filter.ApplyFilter(filterInput).ToList();

            Assert.Equal(1, actualFilterOutput.Count);
            Assert.Empty(actualFilterOutput.Except(expectedOutput));

        }

        [Fact]
        public void Test_ImplicitTransferLocation_1()
        {
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);

            var input = LoadResource(s_Resource_ImplicitTransferLocation1);
            var configurationReader = new ConfigurationReader();

            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            Assert.NotNull(config);
            Assert.Equal(1, config.TransferLocations.Count());
            Assert.Equal(1, config.Actions.Count());


            var action = (ExportAction) config.Actions.First();
            var transferLocation = config.GetTransferLocation(action.TransferLocationName);

            Assert.NotNull(transferLocation);
            Assert.True(String.IsNullOrEmpty(action.TransferLocationSubPath));

            Assert.Equal(@"\\path\to\transfer\location", transferLocation.RootPath);
            Assert.Null(transferLocation.MaximumSize);
            Assert.False(String.IsNullOrWhiteSpace(transferLocation.Name));

        }


        [Fact]
        public void Test_ImplicitTransferLocation_2()
        {
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);

            var input = LoadResource(s_Resource_ImplicitTransferLocation2);
            var configurationReader = new ConfigurationReader();

            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            Assert.NotNull(config);
            Assert.Equal(1, config.TransferLocations.Count());
            Assert.Equal(1, config.Actions.Count());


            var action = (ExportAction)config.Actions.First();
            var transferLocation = config.GetTransferLocation(action.TransferLocationName);

            Assert.NotNull(transferLocation);
            Assert.True(String.IsNullOrEmpty(action.TransferLocationSubPath));

            Assert.Equal(@"\\path\to\transfer\location", transferLocation.RootPath);
            Assert.NotNull(transferLocation.MaximumSize);
            Assert.Equal(ByteSize.ByteSize.FromGigaBytes(20), transferLocation.MaximumSize.Value);
            Assert.False(String.IsNullOrWhiteSpace(transferLocation.Name));

        }

        [Fact]
        public void Test_ImplicitTransferLocation_3()
        {
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);

            var input = LoadResource(s_Resource_ImplicitTransferLocation3);
            var configurationReader = new ConfigurationReader();

            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            Assert.NotNull(config);
            Assert.Equal(1, config.TransferLocations.Count());
            Assert.Equal(1, config.Actions.Count());


            var action = (ExportAction)config.Actions.First();
            var transferLocation = config.GetTransferLocation(action.TransferLocationName);

            Assert.NotNull(transferLocation);
            Assert.Equal("location", action.TransferLocationSubPath);

            Assert.Equal(@"\\path\to\transfer", transferLocation.RootPath);
            Assert.NotNull(transferLocation.MaximumSize);
            Assert.Equal(ByteSize.ByteSize.FromGigaBytes(20), transferLocation.MaximumSize.Value);
            Assert.False(String.IsNullOrWhiteSpace(transferLocation.Name));

        }


        #endregion


        #region Helpers

        XDocument LoadResource(string resourceName)
        {
            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                return XDocument.Load(stream);
            }
        }

        #endregion

    }
}
