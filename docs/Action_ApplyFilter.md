ApplyFilter Action
==================

Applies the specified filter to the current sync state. All files not included by the filter will be removed from the sync state.


###Attributes
- enable: Specifies if the action is to be executed. If value is set to false, the action will be skipped
	- Allowed values: 
		- true
		- false
- inputFilter (optional): The name of the filter to apply to the current sync state


*Note: The attribute inputFilter is optional. Omitting it for this action however is not very useful. It will result in a action that will not affect the sync state in any way.*


###Example

	<applyFilter enable="true" inputFilter="_fileFilter"/>


###Versions
Supported in Version 1.1.0 and above