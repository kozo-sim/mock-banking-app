Mock Banking App
================

A sample C# webapp created during my tertiary studies.

Database-setup.sh has been added more recently to provision a database, as I no longer have access to the one provided by the university.

Dependencies
------------

* An SQL Server instance, or
* Docker if using the UNIX setup script

Installation
------------

On unix-like systems, run database-setup.sh to provision a test environment.
This script will;
* Prompt for an admin password for the database (SA password)
* Create a SQL Server 2019 docker container
* Run a query to create a database with the name given in the script
* Create and apply a migration
* Update the connection string in appsettings.json

The web server can then be started from within Visual Studio.
