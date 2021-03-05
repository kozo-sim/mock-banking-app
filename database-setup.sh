#!/bin/bash

#automated sets up a SQL Server 2019 docker container to test this application with.
#the conection string called "CustomerContext" in appsettings.Development.json will be updated with the variables in this script

DOCKER_CONTAINER_NAME=sql2

SERVER=localhost
DATABASE=mockbankapp
DB_UID=mockbankapp
SA_PASSWORD=SamplePass123



#sudo docker pull mcr.microsoft.com/mssql/server:2019-latest

#sudo docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$SA_PASSWORD" -p 1433:1433 --name $DOCKER_CONTAINER_NAME -h sql1 -d mcr.microsoft.com/mssql/server:2019-latest


cat $0/../MiBank_A3/appsettings.json | sed "s/\"CustomerContext\"=.*/\"CustomerContext\"=server=$SERVER;database=$DATABASE;uid=$DB_UID;pwd=$SA_PASSWORD/g" | $0/../MiBank_A3/appsettings.json
