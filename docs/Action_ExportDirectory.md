ExportDirectory Action
======================
Copies all contents of an arbitrary directory to a transfer location.
The sync state is not taken into consideration for this action, the directory
being exported does not have to be part of sync

### Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
  the action will be skipped
	- Allowed values:
		- true
		- false
- path: The path of the directory to copy
- deleteSourceFiles (optional): If set to true, files in the source directory will be deleted
  once they have been copied (default: false)
- transferLocationName: The name of the transfer location to copy files to.
  The transfer location referenced here must be defined in the configuration
  file.
- transferLocationSubPath (optional): The path that will be added to the
  referenced transfer locations root path. The result will be used as root
	directory for the action. The relative paths of the files within the transfer
	location will be the same as the relative path in the sync state.


### Example
		<exportDirectory enable="true"
		        path="C:\tmp\Export"
				transferLocationName="Filserver"
				transferLocationSubPath="Exported"
				deleteSourceFiles="true"
				 />

### Versions
Supported in Version 1.6 and above
