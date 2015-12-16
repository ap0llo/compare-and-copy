ReadSyncState Action
====================
Reads a serialized sync state from a file and replaces the application's current
state with the state read from file.  
If the file does not exist, execution of the action will result in an empty sync
state.


###Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
  the action will be skipped
	- Allowed values:
		- true
		- false
- fileName: The relative or absolute path of the file to read the sync state
  from.


###Example
	<readSyncState enable="true" fileName="C:\SyncState.xml" />


###Versions
Supported in Version 1.1.0 and above
