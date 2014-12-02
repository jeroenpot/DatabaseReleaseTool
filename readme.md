#DatabaseReleaseTool#

- Runs SQL Scripts from a predefined directory structure in a Transaction
- The directory structure is fixed
- All scripts must be created to be able to run repeatedly
- All filenames must have a valid version number prefix
- Used for Continuous Database Integration
- Used for executing releases
- supported Databases: MSSQL, Oracle (client tools must be installed), MySQL.

##Usage##
Updates a database with the given scripts directory.

To update a database:

With config file

    Mirabeau.DatabaseReleaseTool.exe -d:[pathToDatabaseScripts] -c:[configurationfile] -vf:[fromdatabaseversion] -vt:[todatabaseversion]

Without config file:

    Mirabeau.DatabaseReleaseTool.exe -d:[pathToDatabaseScripts] -server:[databaseservername] -username:[databaseusername] -password:[databasepassword] -databasename:[databasename] (-databasetype:[Oracle||MsSql||MySql] optional)-vf:[fromdatabaseversion] -vt:[todatabaseversion]


**Hidden directories will be ignored.**

The default CommandTimeout per Sql script is 900 seconds.<br/>
The SQL Files will be read with Default Encoding.
The used Encoding can be changed to UTF8:

    Mirabeau.DatabaseReleaseTool.exe -utf8

To get more information:

	Mirabeau.DatabaseReleaseTool.exe -i

###Template for the directory structure:###
	0. PreDataScripts
	1. DatabaseCreate
	2. Tables
	3. Triggers
	4. Functions
	5. StoredProcedures
	6. Indexes
	7. Views
	999. PostDataScripts
	Version

####Directory names that will be ignored:####

	_Misc


##Build script integration##

Msbuild:

	 <Exec
      Command="$(MirabeauDatabaseReleaseTool) -d:$(ActualDatabaseDirectory) -c:$(ActualDatabaseDirectory)$(DatabaseConfigfilename) -vf:$(DatabaseVersionFrom) -vt:$(DatabaseVersionTo)"      
      
          />

The msbuild-by-convention project will be updated to make use of the databaserelease tool.
This will include:
- Automatic installation of local (test) databases
- Automatic creation of releases for defined databases
- Deployment execution

### Contribution guidelines ###
* Comments, methods and variables in english.
* Create unittests where possible.
* Try to stick to the existing coding style.
* Give a short description in the pull request what you're doing and why.