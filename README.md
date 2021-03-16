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
* Pull an SQL Server 2019 docker container
* Configure this database with the SA username and password in the script
* Update the connection string in appsettings.json

The web server can then be started from within Visual Studio.
