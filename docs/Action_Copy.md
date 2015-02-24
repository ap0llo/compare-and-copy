Copy Action
============
Directly copies files from one sync folder to another.

###Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
  the action will be skipped
	- Allowed values:
		- true
		- false
- inputFilter (optional): The name of the filter to be applied to the sync state
  before executing the action.
- syncFolder: The sync folder to copy from (source).
	- Allowed values:
		- Left
		- Right


###Example
	<copy enable="true" syncFolder="Left" inputFilter="_filterName" />


###Versions
Supported in Version 1.1.0 and above
