﻿using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace CompareAndCopy.Core.Configuration.Migration
{
    class ConfigurationMigrator : IConfigurationMigrator
    {
        const string s_ConfigurationSchema = "CompareAndCopy.Core.Configuration.ConfigurationSchema.xsd";
        const string s_TransferLocation = "TransferLocation_";

        static readonly HashSet<XName> s_ActionNames = new HashSet<XName>() {
            XmlNames.Compare,
            XmlNames.Export,
            XmlNames.Import,
            XmlNames.ReadSyncState,
            XmlNames.WriteSyncState,
            XmlNames.ApplyFilter,
            XmlNames.Copy,
            XmlNames.AcquireLock,
            XmlNames.ReleaseLock,
            XmlNames.Sleep,
			XmlNames.RunSyncJob,
            XmlNames.UpdateTransferState,
            XmlNames.ExportDirectory,
            XmlNames.TouchFile
        };
        

        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();


        public XDocument UpgradeConfigurationFile(XDocument currentConfiguration)
        {
            m_Logger.Debug("Upgrading configuration file to latest configuration version");


            //validate input (against a backwards compatible schema that allows all constructs that will be removed during migration)
            currentConfiguration.Validate(GetConfigutationSchema(), (s, e) =>
                {
                    throw new MigrationException("Invalid configuration file: " + e.Message);
                });

            //migration is performed on a copy of the configuration
            var newConfiguration = new XDocument(currentConfiguration);

            //Perform migration steps
            UpgradeNamespace(newConfiguration);

            UpgradeActions(newConfiguration);

            UpgradeImportExportActions(newConfiguration);

            UpgradeFilters(newConfiguration);

            UpgradeTimeStampMargin(newConfiguration);

            return newConfiguration;
        }

        
        /// <summary>
        /// Changes the XML namespace of all elements that do not specify a namespace to the configuration namespace
        /// </summary>
        void UpgradeNamespace(XDocument document)
        {
            if (String.IsNullOrEmpty(document.Root.Name.NamespaceName))
            {
                m_Logger.Debug("Updating XML Namespace");
                document.Root.ReplaceNamespace("", XmlNames.GetNamespace());
            }
            else if (document.Root.Name.Namespace.Equals(XmlNames.GetLegacyNamespace()))
            {
                m_Logger.Debug("Updating XML Namespace");
                document.Root.ReplaceNamespace(XmlNames.GetLegacyNamespace(), XmlNames.GetNamespace());
            }
        }

        /// <summary>
        /// Wraps all action elements defined at the root level of a configuration file into <![CDATA[<action />]]> elements.
        /// Consecutive action elements are placed within the same action parent
        /// </summary>        
        void UpgradeActions(XDocument document)
        {
            //find all elements on the root level of the configuration file
            var rootElements = document.Root.Elements().ToArray();

            m_Logger.Debug("Upgrading action {0} elements", rootElements.Length);

            //iterate over all elements. We need to wrap all elements that are action into a <action /> element
            XElement currentActionList = null;
            foreach (var element in rootElements)
            {
                //current node is a action node
                if (IsActionElement(element))
                {
                    if (currentActionList == null)
                    {
                        m_Logger.Debug("Creating new action list");
                        currentActionList = new XElement(XmlNames.Actions);
                    }

                    m_Logger.Debug("Adding element '{0}' to action list", element.Name.LocalName);
                    element.Remove();
                    currentActionList.Add(element);
                }
                //current node is some other node
                else
                {
                    //if we removed earlier action nodes and added them to a action list,
                    //insert action list before the next non-action element 
                    if (currentActionList != null)
                    {
                        m_Logger.Debug("Inserting action list into configuration");
                        element.AddBeforeSelf(currentActionList);
                        currentActionList = null;
                    }
                }
            }

            //add actions to configuration file (covers the case that no non-action element came after the last action element)
            if (currentActionList != null)
            {
                document.Root.Add(currentActionList);
                currentActionList = null;
            }
        }

        /// <summary>
        /// Upgrades the xml specification of all "import" and "export" actions.
        /// Earlier version allowed a implicit definition of transfer locations. These implicit definitions are removed and replaced as explicitly 
        /// defined transfer locations to the xml
        /// </summary>        
        void UpgradeImportExportActions(XDocument document)
        {
            //get all import and export nodes
            var importExportActions = document.Descendants(XmlNames.Export)
                .Union(document.Descendants(XmlNames.Import))
                .ToList();

            foreach (var action in importExportActions)
            {

                //check if the action uses a implicit transfer location
                if (action.Attribute(XmlAttributeNames.TransferLocation) != null)
                {

                    m_Logger.Debug("Upgrading action'{0}'", action.Name.LocalName);

                    //make sure the configuration file hasn't new and old configuration mixed up (cannot be caught by backwards-compatible XML schema)
                    if (action.Attribute(XmlAttributeNames.TransferLocationName) != null ||
                        action.Attribute(XmlAttributeNames.TransferLocationSubPath) != null)
                    {
                        throw new MigrationException("If 'transferLocation' is specified, the attributes transferLocationName and transferLocationSubPath are not allowed");
                    }


                    //generate a random name for the transfer location
                    var transferLocationName = s_TransferLocation + Guid.NewGuid().ToString();
                    m_Logger.Debug("Transfer-location name is {0}", transferLocationName);

                    //the path of the transfer location is the path as specified by the action
                    var transferLocationPath = action.RequireAttributeValue(XmlAttributeNames.TransferLocation);
                    m_Logger.Debug("Transfer-location path is {0}", transferLocationPath);

                    //default "sub-path" is empty
                    var transferLocationSubPath = "";

                    //if the action specifies a maximum size, add it as maximum size for the transfer location
                    XElement maximumSizeElement = null;
                    if (action.Element(XmlNames.MaxTransferSize) != null)
                    {
                        m_Logger.Debug("Action specified a maximum size, migrating to maximum size of transfer location");

                        //create MaximumSize element
                        maximumSizeElement = new XElement(XmlNames.MaximumSize, action.Element(XmlNames.MaxTransferSize).Attributes());
                    }
                    else if (action.Element(XmlNames.MaxTransferSizeParent) != null)
                    {

                        m_Logger.Debug("Action specified a maximum size for the parent directory, migrating to maximum size of transfer location");

                        //action specifies a maximum size for the parent directory of the specified path
                        // => modify transfer location path to the parent's directory, adjust sub-path accordingly                       
                        transferLocationSubPath = Path.GetFileName(transferLocationPath);
                        transferLocationPath = Path.GetDirectoryName(transferLocationPath);

                        m_Logger.Debug("New path for transfer-location is {0}", transferLocationPath);
                        m_Logger.Debug("New transfer-location sub-path for the action is {0}", transferLocationSubPath);

                        //create MaximumSize element
                        maximumSizeElement = new XElement(XmlNames.MaximumSize, action.Element(XmlNames.MaxTransferSizeParent).Attributes());
                    }


                    //add transfer location definition to the configuration

                    m_Logger.Debug("Adding transfer-location to configuration");

                    var transferLocationDefinition = new XElement(XmlNames.TransferLocation,
                        new XAttribute(XmlAttributeNames.Name, transferLocationName),
                        new XAttribute(XmlAttributeNames.Path, transferLocationPath));

                    if (maximumSizeElement != null)
                    {
                        m_Logger.Debug("Maximum size for transfer location: {0}", maximumSizeElement.ToString());
                        transferLocationDefinition.Add(maximumSizeElement);
                    }
                    document.Root.Add(transferLocationDefinition);


                    //update the action to use the explicitly defined transfer location

                    m_Logger.Debug("Updating action element to use the new transfer-location");

                    if (action.Element(XmlNames.MaxTransferSize) != null)
                    {
                        action.Element(XmlNames.MaxTransferSize).Remove();
                    }
                    else if (action.Element(XmlNames.MaxTransferSizeParent) != null)
                    {
                        action.Element(XmlNames.MaxTransferSizeParent).Remove();
                    }
                    action.Attribute(XmlAttributeNames.TransferLocation).Remove();
                    action.Add(new XAttribute(XmlAttributeNames.TransferLocationName, transferLocationName));
                    action.Add(new XAttribute(XmlAttributeNames.TransferLocationSubPath, transferLocationSubPath));
                }
            }
        }

        /// <summary>
        /// Upgrades the definitions of filters to use a expression tree instead of include/exclude lists
        /// </summary>
        void UpgradeFilters(XDocument document)
        {
            //find all filter definitions
            var filters = document.Root.Elements(XmlNames.Filter).Where(IsLegacyFilter).ToList();

            foreach (var filter in filters)
            {
                var name = filter.RequireAttributeValue(XmlAttributeNames.Name);

                m_Logger.Debug("Upgrading legacy filter {0}", name);


                //get the "include" and "exclude" expressions
                var includeElements = Enumerable.Empty<XElement>().ToArray();
                if (filter.Element(XmlNames.Include) != null)
                {
                    includeElements = filter.Element(XmlNames.Include).Elements().ToArray();
                }
                m_Logger.Debug("Found {0} include expressions", includeElements.Length);

                var excludeElements = Enumerable.Empty<XElement>().ToArray();
                if (filter.Element(XmlNames.Exclude) != null)
                {
                    excludeElements = filter.Element(XmlNames.Exclude).Elements().ToArray();
                }
                m_Logger.Debug("Found {0} exclude expressions", excludeElements.Length);


                //the include / exclude filter definition can be expressed using a expression tree, too:
                /*
                 *   <and> 
                 *      <or>
                 *          ... include elements ...
                 *      </or>
                 *      <not>
                 *          <or>
                 *              ... exlcude elements ...
                 *          </or>
                 *      </not>
                 *   </and>
                 * 
                 */

                var newFilter = new XElement(XmlNames.Filter,
                    new XAttribute(XmlAttributeNames.Name, name),
                    new XElement(XmlNames.And,
                        new XElement(XmlNames.Or, includeElements),
                        new XElement(XmlNames.Not,
                            new XElement(XmlNames.Or, excludeElements))));


                m_Logger.Debug("Replacing filter definition in configuration");
                filter.AddAfterSelf(newFilter);
                filter.Remove();
            }
        }

        /// <summary>
        /// Upgrades the time span margin element of the Compare action.
        /// Before version 1.5, the time-stamp margin was specified as global element in the configuration file.
        /// As it is only relevant for the compare action, it was moved to the compare action element.
        /// 
        /// This method adds a copy of the global time-stamp margin element to all compare action elements 
        /// that do not specify an own timeSpanMargin element
        /// </summary>
        /// <param name="document"></param>
        void UpgradeTimeStampMargin(XDocument document)
        {
            var globalTimesStampMargin = document.Root.Element(XmlNames.TimeStampMargin);

            if (globalTimesStampMargin == null)
            {
                return;
            }

            globalTimesStampMargin.Remove();

            var compareActions = document.Descendants(XmlNames.Actions)
                .SelectMany(actionlist => actionlist.Elements(XmlNames.Compare));

            foreach (var compareAction in compareActions)
            {
                if (compareAction.Element(XmlNames.TimeStampMargin) == null)
                {
                    compareAction.Add(globalTimesStampMargin);
                }
            }
        }

        /// <summary>
        /// Determines if the specified element is an action element
        /// </summary>
        bool IsActionElement(XElement element) => s_ActionNames.Contains(element.Name);

        /// <summary>
        /// Determines if the specified filter is a legacy filter definition
        /// </summary>
        bool IsLegacyFilter(XElement filterNode)
        {
            return filterNode.Elements().Count() <= 2 &&
                (filterNode.Elements().Count(element => element.Name == XmlNames.Include) == 1 ||
                filterNode.Elements().Count(element => element.Name == XmlNames.Include) == 1);
        }

        XmlSchemaSet GetConfigutationSchema()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(s_ConfigurationSchema))
            {
                var schemaSet = new XmlSchemaSet();

                schemaSet.Add(null, XmlReader.Create(stream));

                return schemaSet;
            }
        }
    }
}
