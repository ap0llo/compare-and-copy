using Moq;
using ServerSync.Core.Configuration;
using ServerSync.Core.Copy;
using ServerSync.Core.Locking;
using ServerSync.Core.PathResolving;
using ServerSync.Core.State;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Xunit.Extensions;

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

        #region Filter

        /// <summary>
        /// Not purely a unit test. 
        /// Tests if the configuration can read a configuration file defining a filter and if this filter
        /// works correctly to ensure filter is not setup wrong by the configuration reader
        /// </summary>
        [Fact]
        public void Test_Filter_1()
        {
            //we need to mock path resolver
            var pathResolverMock = new Mock<IPathResolver>();

            //get the xml configuration for the test
            var input = LoadResource(s_Resource_TestFilter1);

            //initialize ConfigurationReader instance
            var configurationReader = new ConfigurationReader();

            //read the configuration
            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            //verify that path resolver was not used (no paths in config)
            pathResolverMock.Verify(r => r.GetAbsolutePath(It.IsAny<string>()), Times.Never());

            //Assert exactly one filter was read from the configuration
            Assert.Equal(1, config.Filters.Count());
            Assert.DoesNotThrow(new Assert.ThrowsDelegate(() => config.GetFilter(s_Filter1)));

            //test the filter
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
                new FileItem("file1.ext3") { CompareState = CompareState.Conflict }
                
            };

            var actualFilterOutput = filter.ApplyFilter(filterInput).ToList();

            Assert.Equal(1, actualFilterOutput.Count);
            Assert.Empty(actualFilterOutput.Except(expectedOutput));
        }

        #endregion


        #region Transfer Locations

        /// <summary>
        /// Tests reading of implicitly defined transfer locations 
        /// (without maximum size specified for the transfer location)
        /// </summary>
        [Fact]
        public void Test_ImplicitTransferLocation_1()
        {
            //set up path resolver mock (returns unchanged path for all inputs)
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);

            //get test input
            var input = LoadResource(s_Resource_ImplicitTransferLocation1);
            var configurationReader = new ConfigurationReader();

            //read configuration
            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);


            //check the configuration read by the configuration reader
            //config contains a single export action that implicitly defines a transfer location
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

        


        /// <summary>
        /// Tests reading of implicitly defined transfer locations 
        /// (Maximum size specified for the transfer location path)
        /// </summary>
        [Fact]
        public void Test_ImplicitTransferLocation_2()
        {
            //set up path resolver mock (returns unchanged path for all inputs)
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);

            //load test data
            var input = LoadResource(s_Resource_ImplicitTransferLocation2);
            var configurationReader = new ConfigurationReader();

            //read configuration
            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            //check configuration
            //export action specifies a maximum transfer size which must be the maximum size for the implicitly defined transfer location            
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

        /// <summary>
        /// Tests reading of implicitly defined transfer locations 
        /// (Maximum size specified for the specified path's parent directory)
        /// </summary>
        [Fact]
        public void Test_ImplicitTransferLocation_3()
        {
            //set up path resolver mock (returns unchanged path for all inputs)
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);
            
            //load test data
            var input = LoadResource(s_Resource_ImplicitTransferLocation3);
            var configurationReader = new ConfigurationReader();

            //read configuration
            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            //check configuration
            //export action specifies a maximum transfer size for the path's parent directory
            //This should be the maximum size of the transfer location
            //Because the maximum size applies to the parent directory, the transfer location's path should be the parent directory
            //to keep the path, the TransferLocationSubPath property of the action must be set accordingly
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
      
        [Fact]
        public void Test_ReadTransferLocation_1()
        {
            var transferLocationName = "transferLocation1";
            var transferLocationPath = @"\\path\to\transfer\location";
            var inputName = "ServerSync.Core.Test.Configuration.TestData.TransferLocation1.xml";

            //set up path resolver mock (returns unchanged path for all inputs)
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);


            var input = LoadResource(inputName);

            var configurationReader = new ConfigurationReader();

            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            //make sure path resolver was called to resolve the path of the transfer location
            pathResolverMock.Verify(r => r.GetAbsolutePath(It.IsAny<string>()), Times.Once());


            Assert.Equal(1, config.TransferLocations.Count());
            
            var transferLocation = config.GetTransferLocation(transferLocationName);
            Assert.NotNull(transferLocation);

            Assert.Equal(transferLocationName, transferLocation.Name);
            Assert.Equal(transferLocationPath, transferLocation.RootPath);
            Assert.Null(transferLocation.MaximumSize);
        }

        [Fact]
        public void Test_ReadTransferLocation_2()
        {
            var transferLocationName = "transferLocation1";
            var transferLocationPath = @"\\path\to\transfer\location";
            var inputName = "ServerSync.Core.Test.Configuration.TestData.TransferLocation2.xml";

            //set up path resolver mock (returns unchanged path for all inputs)
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);


            var input = LoadResource(inputName);

            var configurationReader = new ConfigurationReader();

            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            //make sure path resolver was called to resolve the path of the transfer location
            pathResolverMock.Verify(r => r.GetAbsolutePath(It.IsAny<string>()), Times.Once());


            Assert.Equal(1, config.TransferLocations.Count());

            var transferLocation = config.GetTransferLocation(transferLocationName);
            Assert.NotNull(transferLocation);

            Assert.Equal(transferLocationName, transferLocation.Name);
            Assert.Equal(transferLocationPath, transferLocation.RootPath);
            Assert.NotNull(transferLocation.MaximumSize);
            Assert.Equal(ByteSize.ByteSize.FromMegaBytes(23), transferLocation.MaximumSize);
        }


        [Fact]
        public void Test_ReadTransferLocation_3()
        {
            var transferLocationName = "transferLocation1";
            var transferLocationPath = @"path\to\transfer\location";
            var inputName = "ServerSync.Core.Test.Configuration.TestData.TransferLocation3.xml";
           
            var dir = Path.GetTempPath();

            var input = LoadResource(inputName);

            var configurationReader = new ConfigurationReader();

            var config = configurationReader.ReadConfiguration(input, new PathResolver(dir));

            Assert.Equal(1, config.TransferLocations.Count());

            var transferLocation = config.GetTransferLocation(transferLocationName);
            Assert.NotNull(transferLocation);

            Assert.Equal(transferLocationName, transferLocation.Name);
            Assert.True(Path.Combine(dir, transferLocationPath).Equals(transferLocation.RootPath, StringComparison.InvariantCultureIgnoreCase));
            Assert.NotNull(transferLocation.MaximumSize);
            Assert.Equal(ByteSize.ByteSize.FromMegaBytes(23), transferLocation.MaximumSize);
        }


        /// <summary>
        /// Tests if a ConfigurationException is thrown when a transfer location definition's name is empty or missing
        /// </summary>
        [Theory]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.TransferLocation4_1.xml")]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.TransferLocation4_2.xml")]
        public void Test_ReadTransferLocation_4(string resourceName)
        {       

            //set up path resolver mock (returns unchanged path for all inputs)
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);


            var configurationReader = new ConfigurationReader();

            var exception = Record.Exception( () => configurationReader.ReadConfiguration(LoadResource(resourceName), pathResolverMock.Object));
            Assert.True(typeof(ConfigurationException).IsAssignableFrom(exception.GetType()));            
        }

        #endregion


        #region Actions


        /// <summary>
        /// Ensures the expected number of actions is read from the configuration file.
        /// This must be the case whether the actions appear inside a <![CDATA[<action />]]> node or on the configuration file's root level.
        /// Mixing of actions inside and outside a action node is allowed, too
        /// </summary>
        [Theory]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.Actions1.xml", 10)]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.Actions2.xml", 10)]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.Actions3.xml", 10)]
        public void Test_ReadActions(string resourceName, int expectedActionCount)
        {

            //set up path resolver mock (returns unchanged path for all inputs)
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);

            var input = LoadResource(resourceName);

            var configurationReader = new ConfigurationReader();

            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            Assert.Equal(expectedActionCount, config.Actions.Count());            
        }


        #endregion


        #region AcquireLock Action

        [Theory]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.Action_AcquireLock_Success_1.xml", true, "file.lock", null)]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.Action_AcquireLock_Success_2.xml", true, "file.lock", 2)]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.Action_AcquireLock_Success_3.xml", false, "file.lock", null)]
        public void ReadAction_AcquireLock_Success(string resourceName, bool expectedEnable, string lockFileValue, int? timeoutInMinutes)
        {
            var resolvedPath = Guid.NewGuid().ToString();

            var mock = GetDefaultPathResolverMock();
            mock.Setup(m => m.GetAbsolutePath(lockFileValue)).Returns(resolvedPath);

            var configurationReader = new ConfigurationReader();          
            var config = configurationReader.ReadConfiguration(LoadResource(resourceName), mock.Object);

            Assert.Equal(1, config.Actions.Count());

            var action = config.Actions.First() as AcquireLockAction;
            Assert.NotNull(action);
            
            
            Assert.Equal(expectedEnable, action.IsEnabled);
            Assert.Equal(resolvedPath, action.LockFile);

            if (timeoutInMinutes.HasValue)
            {
                Assert.Equal(new TimeSpan(0, timeoutInMinutes.Value, 0), action.Timeout);
            }
            else
            {
                Assert.Null(action.Timeout);
            }
        }

        #endregion

        [Theory]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.Action_AcquireLock_Fail_1.xml")]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.Action_AcquireLock_Fail_2.xml")]
        [InlineData("ServerSync.Core.Test.Configuration.TestData.Action_AcquireLock_Fail_3.xml")]
        public void ReadAction_Fail(string resourceName)
        {

            var mock = GetDefaultPathResolverMock();            

            var configurationReader = new ConfigurationReader();

            var exception = Record.Exception(() => configurationReader.ReadConfiguration(LoadResource(resourceName), mock.Object));
            Assert.True(typeof(ConfigurationException).IsAssignableFrom(exception.GetType())); 
            
            
        }



        /// <summary>
        /// Tests if a configuration file in which no XML namespace is specified is read correctly
        /// </summary>
        [Fact]
        public void Test_ReadNoNamespace()
        {
            var inputName = "ServerSync.Core.Test.Configuration.TestData.NoNamespace.xml";

            //set up path resolver mock (returns unchanged path for all inputs)
            var pathResolverMock = new Mock<IPathResolver>();
            pathResolverMock.Setup(r => r.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);

            var input = LoadResource(inputName);

            var configurationReader = new ConfigurationReader();

            var config = configurationReader.ReadConfiguration(input, pathResolverMock.Object);

            Assert.Equal("left", config.Left.Name);
            Assert.Equal("right", config.Right.Name);
            Assert.Equal(1, config.Actions.Count());
            Assert.Equal(new TimeSpan(0, 0, 0, 0, 50), config.TimeStampMargin);
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

        Mock<IPathResolver> GetDefaultPathResolverMock()
        {
            var mock = new Mock<IPathResolver>();
            mock.Setup(m => m.GetAbsolutePath(It.IsAny<string>())).Returns((string s) => s);
            return mock;
        }

        #endregion

    }
}
