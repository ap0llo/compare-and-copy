ReleaseLock Action
==================

Releases the lock on the specified file if it has been acquired using the AcquireLock action.


###Attributes
- enable: Specifies if the action is to be executed. If value is set to false, the action will be skipped
	- Allowed values: 
		- true
		- false
- lockFile: The absolute or relative path of the file which's lock to release


###Example

	<releaseLock enable="true" lockFile="..\file.lock" />


###Versions
Supported in Version 1.3.0 and above