Compare Action
==============
Compares the left and right sync folders and thus creates a new sync state.
Files which are already present in the sync state and have a SyncState value
other than none will retain this value.


### Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
  the action will be skipped
	- Allowed values:
		- true
		- false
- inputFilter (optional): The name of the filter to be applied to the sync
  state before executing the action.

### Child Elements
- timeStampMargin (optional): Specifies a time-span by which two modification
  times of files may differ for the files still being considered equal.
  The value is specified as time span with the individuals components of the
  time span being defined as individual XML attributes.
  All attributes are optional. If not present a value of 0 will be assumed.
  The value of the time-span is the sum of all attribute values.
  The following attributes are available
  - *h*: Specifies a number of hours to be added to the time-span
  - *m*: Specifies a number of minutes to be added to the time-span
  - *s*: Specifies a number of seconds to be added to the time-span
  - *ms*: Specifies a number of milliseconds to be added to the time-span



### Examples

	<compare enable="true" />

	<compare enable="true">
    	<timeStampMargin ms="1000" />
  	</compare>

### Versions
Supported in version 1.1.0 and above.  
The timeStampMargin element is only supported in version 1.5 and above.
