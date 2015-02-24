Sleep Action
============
Suspends execution of actions for a specified amount of time.


###Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
  the action will be skipped
	- Allowed values:
		- true
		- false


###Child Elements
- timeout: The amount of time to wait. Time span is specified like the
  timeStampMargin element in the configuration file


###Example
Suspends execution for 2 minutes

	<sleep enable="true">
		<timeout m="2" />
	</sleep>


###Versions
Supported in Version 1.3.0 and above
