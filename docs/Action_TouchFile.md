TouchFile Action
================

Sets the last write time of th specified file to the current time.
If the file does not exist, it will be created


### Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
  the action will be skipped
	- Allowed values:
		- true
		- false
- path: The path of the file to be touched



### Example
Set the last write time for the file "C:\foo"

	<touchFile enable="true" path="C:\foo" />	


### Versions
Supported in Version 1.8.0 and above
