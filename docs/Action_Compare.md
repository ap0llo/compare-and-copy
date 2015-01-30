Compare Action
==============

Compares the left and right sync folders and thus creates a new sync state. Files which are already present in the sync state and have a SyncState value other than none will retain this value.


###Attributes

- enable: Specifies if the action is to be executed. If value is set to false, the action will be skipped
	- Allowed values: 
		- true
		- false
- inputFilter (optional): The name of the filter to be applied to the sync state before executing the action.


###Example
	
	<compare enable="true" />

###Versions
Supported in Version 1.1.0 and above