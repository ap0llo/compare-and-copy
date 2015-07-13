UpdateTransferState Action
==================
Updates the list of locations a file being transferred can be found in by scanning a number of transfer-location and interim locations.

All files for which the state contains no locations will be removed from the sync-state


###Attributes
- enable: Specifies if the action is to be executed. If value is set to false,
	the action will be skipped
	- Allowed values:
		- true
		- false
- child elements
	- transferLocation: Specifies a transfer-location to be scanned (referenced by name)
	- interimLocation: Specifies a "interim location" (a path that is not a transfer-location but might contain
	  files being transferred, e.g. as temporary location for a copy tool) to be included in the sync state



###Example
	<updateTransferState enable="true">
		<transferLocation transferLocationName="foo" transferLocationSubPath="bar" />
		<interimLocation path="\\foo\bar"/>
	</updateTransferState>



###Versions
Supported in Version 1.5.0 and above
