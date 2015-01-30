Export Action
=============


Inline definition of transfer location
--------------------------------------
###Attributes 
- enable: Specifies if the action is to be executed. If value is set to false, the action will be skipped
	- Allowed values: 
		- true
		- false
- inputFilter (optional): The name of the filter to be applied to the sync state before executing the action.
- syncFolder: The sync folder to copy from.
	- Allowed values:
		- left
		- right
- transferLocation: The root path of the transfer-location to copy the files to. The relative paths of the files within the transfer location will be the same as the relative path in the sync state.


###Child Elements
- maxTransferSize (optional): The maximum value for the combined size all files in the transfer location. If exporting a file would exceed this limit, it will be skipped,
- maxTransferSizeParent (optional): The maximum value for the combined size all files in the transfer location's parent directory. If exporting a file would exceed this limit, it will be skipped,


*Note: maxTransferSize and maxTransferSizeParent are mutually exclusive. At most one of the two elements may be used*


###Example

		<export enable="true" 				
				transferLocation="\\fileserver\Transfer\Exported" 
				syncFolder="left" 
				inputFilter="MissingRightFilter">
			<maxTransferSize gb="20" />
		</export>	


###Versions
Supported in Version 1.1.0 and above

**This feature is obsolete and will be removed in version 2.0**


Referenced Transfer location
----------------------------
###Attributes
- enable: Specifies if the action is to be executed. If value is set to false, the action will be skipped
	- Allowed values: 
		- true
		- false
- inputFilter (optional): The name of the filter to be applied to the sync state before executing the action.
- syncFolder: The sync folder to copy from.
	- Allowed values:
		- left
		- right
- transferLocationName: The name of the transfer location to copy files to. The transfer location referenced here must be defined in the configuration file.
- transferLocationSubPath (optional): The path that will be added to the referenced transfer locations root path. The result will be used as root directory for the action. The relative paths of the files within the transfer location will be the same as the relative path in the sync state. 


###Example

		<export enable="true" 
				transferLocationName="Filserver" 
				transferLocationSubPath="Exported" 
				syncFolder="left" 
				inputFilter="MissingRightFilter">		


###Versions
Supported in Version 1.3.0 and above