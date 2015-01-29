ServerSync Introduction
=======================


ServerSync is a tool to compare the contents of two directory trees and execute actions based on the results it finds. The two directories being compared to are labeled "Left" and "Right". 

Core to understanding how ServerSync works is the concept of the "SyncState". The sync state is a list of files which are different between the left and the right directory. Each of these files has 3 properties


- **Relative Path**: The path of the file relative to the root directory of the folders being compared.
- **CompareState**: Specifies the result of the comparison. It has one of these values
	- **MissingLeft**: The file only exists in the right directory
	- **MissingRight**: The file only exists in the left directory
	- **Conflict**: The file exists in both directories but the two version differ in size and/or modification time-stamps
- **SyncState**: SeverSync can be used to copy files to a location from which they are transferred to he respectively other folder. Transfer itself is not handled within the tool, however it keeps track of which files are currently being transferred which is stored in this property. The possible values are
	- None: The file is not being transferred at the moment
	- InTransferToLeft: The file is currently being transferred from the right to the left directory
	- InTransferToRight: The file is currently being transferred from the left to the right directory.




based on this concept, ServerSync allows to execute so called "actions" which depending on the action change the current state.

Definitions of the directories to be synced and the actions to be executed are defined in the [Configuration file](ConfigurationFile)