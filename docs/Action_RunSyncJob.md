RunSyncJob Action
==============
Runs the specified sync configuration in the current instance of CompareAndCopy

### Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
  the action will be skipped
	- Allowed values:
		- true
		- false
- path: The path of the sync configuration to execute. If the value is a
  relative path, the path will be interpreted as being relative to the
  directory the current sync configuration file is located in


### Example

	<runSyncJob enable="true" path="..\OtherConfiguration.xml" />

### Versions
Supported in version 1.5 and above.  
