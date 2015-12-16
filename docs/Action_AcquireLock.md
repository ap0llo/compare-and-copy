AcquireLock Action
==================
Locks the specified file. While the file is locked, it cannot be accessed or
changed by any application, including ServerSync itslef. Thus you should never
lock files that are processed by ServerSync (like configuration or syncstate
files).

By locking files which's sole purpose is being locked ("lock files") execution
of multiple ServerSync instances can be synchronized to avoid conflicts that
would normally occur when several instances modify the same sync state file.

If the file is already locked, ServerSync will wait until a lock on the file
can be acquired.


###Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
  the action will be skipped
	- Allowed values:
		- true
		- false
- lockFile: The relative or absolute path of the file that will be locked.


###Child Elements
- timeout (optional): Specifies a maximum amout of time to wait for the file
  to be locked. If the file cannot be locked within the specified duration,
	execution of the actions in the current configuration file will be aborted.  

	Time span is specified like the timeStampMargin element in the configuration
	file.


###Examples
Acquire lock with timeout

	<acquireLock enable="true" lockFile="..\file.lock">
			<timeout m="2" />
	</acquireLock>

Acquire lock without timeout

	<acquireLock enable="true" lockFile="..\file.lock" />


###Versions
Supported in Version 1.3.0 and above
