Author: Dverhoeckx
Date: 19-02-2008

This document describes the directory sturcture requred to release databases with this tool.

0. PreDataScripts 
	// These will be execuate first. It is ment for data manipulation..
1. DatabaseCreate
	// The script that creates the database.
2. Tables
	//Scripts that create or alter the tables.
	//When creating a new table it it is recommended to use an IF EXISTS in the sql script.
	//Mutations can be done by using different scripts.
	//Note: the order is determind by the version number prefix.
3. Triggers
	//Scripts that create or modify triggers
	//When creating a new trigger it it is recommended to use an IF EXISTS in the sql script.
	//Mutations can be done by using different scripts.
	//Note: the order is determind by the version number prefix.
4. Functions
	//Scripts that create or modify functions
	//When creating a new function it it is recommended to use an IF EXISTS in the sql script.
	//Mutations can be done by using different scripts.
	//Note: the order is determind by the version number prefix.
5. StoredProcedures
	//Scripts that create or modify Stored Procedures 
	//When creating a new stored procedure it it is recommended to use an DROP CREATE statement
	//Note: the order is determind by the version number prefix.
6. Indexes
	//Scripts that create or modify indexes
	//When creating a new index it it is recommended to use an IF EXISTS in the sql script.
	//Note: the order is determind by the version number prefix.
7. Views
	//Scripts that create or modify views
	//When creating a new view it it is recommended to use an IF EXISTS in the sql script.
	//Mutations can be done by using different scripts.
	//Note: the order is determind by the version number prefix.
999. PostDataScripts
	//The sql scripts will be execute ast. it is ment for data manipulation.
Version
	//A sql script can be placed here that can modify the database version
	
* All files must have a .sql extention.
* All files must have a version number prefix. ie 1.0.0_001_CreateAllTables.sql.
** Exception is the 'CreateDatabase.sql' file, this file does not have a version prefix.

* All scripts within the given version range will ALWAYS be executed. Make sure scripts can execute multiple time.
** This also goed for the 'CreateDatabase.sql' file. Make sure you check if the database exists.

+ In the version folder you can place a (generated) sql file to update the database version. 

* The dummy.txt files are in the folder because of a version control not allowing empty folders.