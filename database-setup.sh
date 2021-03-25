#!/bin/bash

#automated sets up a SQL Server 2019 docker container to test this application with.
#the conection string called "CustomerContext" in appsettings.Development.json will be updated with the variables in this script

DOCKER_CONTAINER_NAME=sql2

SERVER=localhost
DATABASE=mockbankapp
DB_UID=SA

(

echo "Enter new SA Password: (no quotes or special characters, NOT injection safe)"
read SA_PASSWORD

echo "You may now be prompted for your root password if your docker install requires it"

sudo docker pull mcr.microsoft.com/mssql/server:2019-latest

#setup the container
sudo docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=${SA_PASSWORD}" -p 1433:1433 --name ${DOCKER_CONTAINER_NAME} -h ${DOCKER_CONTAINER_NAME} -d mcr.microsoft.com/mssql/server:2019-latest

#stall until initialisation has finished, then send a query to create the database
#TODO; resolve this race condition
echo "Waiting for docker container to start..."
for t in {30..1} ; do
	printf "%s \r" $t
	sleep 1
done
echo "Database ready"

#run database creation query
sudo docker exec -it $DOCKER_CONTAINER_NAME /bin/bash -c "/opt/mssql-tools/bin/sqlcmd -t 120 -S localhost -U SA -P $SA_PASSWORD -Q \"CREATE DATABASE $DATABASE\""

#substitute connection string in appsettings.json
sed -i "" "s/\"CustomerContext\": \".*\"/\"CustomerContext\": \"server=${SERVER};database=${DATABASE};uid=${DB_UID};pwd=${SA_PASSWORD}\"/" MiBank_A3/appsettings.json

#create and apply migration
echo "Creating migration..."
dotnet tool update --global dotnet-ef
cd MiBank_A3
rm -r ./Migrations
dotnet ef migrations add MiBankMigration --context MiBankContext
dotnet ef migrations add MiBankIdentityMigration --context MiBank_A3IdentityDbContext
echo "Applying migration..."
dotnet ef database update --context MiBankContext
dotnet ef database update --context MiBank_A3IdentityDbContext

#database should now be successfully set up
echo "Database setup completed"
echo "Login details have been saved in appsettings.json"
) || echo "Database was not setup correctly"
