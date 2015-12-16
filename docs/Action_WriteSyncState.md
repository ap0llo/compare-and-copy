WriteSyncState Action
=====================
Serializes the current sync state to XML and writes it to a file.


###Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
  the action will be skipped
	- Allowed values:
		- true
		- false
- inputFilter (optional): The name of the filter to be applied to the current
  sync state before executing the action.
- fileName: The relative or absolute path of the file to write the sync
  state to.


###Example
	<writeSyncState enable="true" fileName="C:\SyncState.xml" />


###Versions
Supported in Version 1.1.0 and above
