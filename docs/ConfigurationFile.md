ServerSync Configuration File
=============================
The configuration file controls all behavior of the ServerSync tool and consists
of both definitions of global properties and a list of actions to execute.

Basics
-------
- For all elements that are not actions or include elements, the order in which
  they appear in the document is not relevant.
- For paths both absolute and relative paths can be used. Relative paths are
  always resolved in relation to the path of the configuration file they are
	used in.


Global Elements
-----------------
###Folder definitions
Most of the actions that can be executed by ServerSync require the sync-folders
to be defined. There must be exactly one "left" and one "right" sync folder.

####Attributes
- *name* : Defines the name of the sync folder
- *rootPath*: The root path of the sync folder. ServerSync references all files
   using relative paths which are relative to the root paths of the sync folders.

####Example
	<left name="MusicLeft" rootPath="C:\Music" />
	<right name="MusicRight" rootPath="G:\Music" />

--------------------------------------------------------------------------------

###Time-Stamp margin
This optional element allows to specify a margin by which time stamps of files
may differ to still be considered equal. This property is only used by the
"Compare" action. The value is specified as time span with the individuals
components of the time span being defined as individual XML attributes.

####Attributes
- *h*: Specifies a number of hours to be added to the time-span
- *m*: Specifies a number of minutes to be added to the time-span
- *s*: Specifies a number of seconds to be added to the time-span
- *ms*: Specifies a number of milliseconds to be added to the time-span

All attributes are optional. If not present a value of 0 will be assumed.
The value of the time-span is the sum of all attribute values.

*Note: There is no limit on the size of the individual components, so two hours
 can be defined as 120 minutes, too. Fell free to mix and match, e.g. h="1" and
 m="113" would be valid and equivalent to h="2" m="53"*

***Note: Versions before v1.3.0 do only support the 'ms' attribute***

#####Example
	<timeStampMargin h="1" m="2" s="3" />

Defines the length of 1 hours, 2 minutes and 3 seconds.


--------------------------------------------------------------------------------

###Include
***Note: The include element is supported in versions 1.3.0 and higher***

Include enables referencing other configuration files so settings that are
common to several configuration files can be shared.  
The referenced configuration file is processed as if all it's contents would be
copied from the reference file into the referencing file at the location of the
include element.

####Attributes
- *path*: The relative or absolute path to the configuration file to be included

####Example
	<include path="..\_common\TransferLocations.xml" />

--------------------------------------------------------------------------------

###Transfer location definitions
Defines a transfer location to be used by the Export action.

***Note: Transfer location definitions are supported in versions 1.3.0 and
   later***

####Attributes
- *name*: The name of the transfer location. This named needs to be unique
   within an configuration file (an it's included files)
- *path*: The root path of the transfer location

####Child Elements
- *maximumSize* (optional): If defined limits the maximum size of the transfer
  location. Before copying a file, the combined size of all files in the
	transfer location will be calculated. If the file to be copied is larger than
	the difference between the maximum and current size, it will be skipped.

####Example
	<transferLocation name="transferLocation1" path="\\server\Transfer">
		<maximumSize gb="40" />
	</transferLocation>

--------------------------------------------------------------------------------
###Filter definitions
Defines a filter that can be applied to the sync state in various actions.

***Note: Definition of a filter as expression tree is only supported in version
   1.4 and higher. For filters in earlier versions see "Legacy Filters"***

A filter is a tree of expressions that are evaluated when the filter is applied.
The filter defines a single root node of the expression tree. Depending on the
type of a node it can have various child nodes.

The following nodes are supported:
- and: Realizes a logical 'and' over one or more child expressions. Evaluates
  to true if all of the child expressions evaluate to true.
- or: Realizes a logical 'or' over one or more child expression. Evaluates to
  true of at least one of the child expression evaluates to true
- not: Realizes a logical 'not' for a single child expression. Evaluates to true
  if the child expression evaluates to false.
- regex: Check's a file's relative path against a regular expression. Evaluates
  to true if the relative path matches the regualr expression.
- microscopeQuery: Check's a file's relative path using a microscope query
  ([see Microscope GitHub page](http://github.com/clotheshorse/microscope)).
	Evaluates to true if the query evaluates to true for the item's relative path.
- compareState: Checks a file's CompareState. Evaluates to true if the file's
  CompareState matches the CompareState defined in the expression
- syncState: Checks a file's SyncState. Evaluates to true if the file's
  SyncState matches the SyncState defined in the expression


####Legacy Filters
Versions before 1.4.0 do only support a more limited filtering functionality.
Filters in this version consits of only two elements

- include: One or more expressions files are evaluated against. All files for
  which at least one expression matches, is included in the (temporary) state
- exclude: After evaluating the include expressions, all elements for which any
  of the exclude expressions matches is removed from the temporary sync state
	which will result in the filtered sync-state.

Thus, a include/exclude filter is equivalent to the following expression tree
that can be defined using the new filtering system (In fact, this transformation
is performed internally if such a filter is encountered in a configuration file)

	<filter name="_MusicFilter">
		<and>
			<or>
				... include expressoins ...
			</or>
			<not>
				<or>
					... exclude expressions ...
				</or>
			</not>
		</and>
	</filer>

--------------------------------------------------------------------------------

###Actions
After parsing a configuration file ServerSync will execute all actions found in
the file in the order in which they appear.  
Actions appear inside a "action group". Action groups only exists to give
structure to the configuration file and have no influence on the execution
of the actions (for example it would make no difference if every action is
wrapped in it's own action group)

***Note: Action groups were introduced in version 1.3.0. In earlier versions
   actions were defined as top level element in the configuration file.
	 For backwards compatibility this is still supported but will be removed in
	 future versions***


####Available Actions
- Compare
- Export
- Import
- Copy
- ReadSyncState
- WriteSyncState
- ApplyFilter
- AcquireLock
- ReleaseLock
- Sleep

####Example
	<actions>
		<acquireLock enable="true" lockFile="locks\file.lock">
			<timeout m="2" />
		</acquireLock>
		<readSyncState enable="true" fileName="..\SyncState.xml" />
		<releaseLock enable="true" lockFile="locks\file.lock" />
	</actions>
