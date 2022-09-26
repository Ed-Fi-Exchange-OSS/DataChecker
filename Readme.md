Ed-Fi ODS Data Checker
============

# Description #
Data Checker is an applicaiton that manages SQL scripts that will find level 2 data validation errors in an Ed-Fi ODS.

# Install Guides #
The following methods are supported for Data Checker install.
* Install from binaries
* Run in docker
* Install from source code

### Binary Install ###
This option will work well for teams that are installing data checker on a Windows Server. There is an option for Either a MS SQL or a Postgres backend, use whatever database technology you are using for your ODS.
####  Binary Install- Prerequisites ####
* Install IIS
* Install .Net Core (or other 3.x version) 3.1 Hosting Bundle (https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-3.1.20-windows-hosting-bundle-installer)
* SQL Server or Postgres DB Server
* SQL Server Management Studio (SSMS)  or PG Admin

#### Binary Install- Installation Steps ####
* Download the file WebApp.zip and either the DataCheckerPersistance.bacpac (for SqlServer) or DataCheckerPersistance.sql (for postgres) from the latest release here: https://github.com/Ed-Fi-Exchange-OSS/DataChecker/releases
* Unzip WebApp.zip to a local folder
* Validate if you have all your prerequisites installed running the powershell script VerifyPreReqs.ps1
* Copy the folder DataChecker in the path: C:\inetpub\wwwroot\DataChecker or other suitable location for the web application
* Find that folder in IIS, right click and 'convert to application'
* If using MS Sql Server, Restore the dataCheckerPersitance.bacpac file.
* Otherwise if using Posgres, Create a new Postgres database and run the DataCHeckerPersistance.SQL file in PG Admin or the psql command line to restore all of the objects to that database.
* Open the file C:\inetpub\wwwroot\DataChecker\appsettings.json
	* Make sure the 'Engine' value is set to the database engine of your choice
	* Make usre the connection string is valid for the database that you just restored. The database user will need write acces to the datachecker database. In order to use integrated security with MS SQL SErver there will need to be a new SQL Server login created or the user IIS_APPPOOL/<app pool name>. 
	* Change the value of "EncryptionKey" from "CHANGE_ME_PLEASE!" to a random text string. This will impact how ODS connection string passwords are stored in the dabase
	* (optinally) configure the location and level of the logging
	* Repeat these edits in the appsettings.production.json folder
* restart IIS


### Build from source with Visual Studio ###
This method is recommend for teams that may want to modify the source code and have sufficent knowledge of the .NET core development environment, visual studio, and IIS.
#### Source Code Install- Prerequisites ####
* Basic to mid skills on hosting application on Windows IIS Server
* Visual Studio 2019 Community edition. ([https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/)
* .Net core 3.1 hosting bundle (  [https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.3-windows-hosting-bundle-installer](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.3-windows-hosting-bundle-installer) )
* NodeJs LTS ([https://nodejs.org/en/](https://nodejs.org/en/))
* Angular CLI ([https://cli.angular.io/](https://cli.angular.io/)) npm install -g @angular/cli
* Acquiring the codebase: [https://github.com/Ed-Fi-Exchange-OSS/DataChecker/](https://github.com/Ed-Fi-Exchange-OSS/DataChecker/)
    

#### Soure Code Install - Installation Steps ####

* Clone this repository locally
* Open the solution (MSDF.DataChecker.sln) file located in the root directory of the repository in Visual Studeio
* Open the <project root./MSDF.DataChecker.WebApp/ClientApp folder in Powershell (or another shell and run:) <npm install -force>
* Ensure the solution builds: Right click the *“MSDF.DataChecker.WebApp”* project and click rebuild. If you get any errors it's probably because you are missing one of the dependencies or prerequisites listed above.
* Right Click on the WebApp folder and set it as the startup project.
* In the WebApp project, configure both “appsettings.json” and "appsettings.Production.json". Specify your database engine (postgres of SqlServer) and your database connection string.
* In visual studio open: Tools->Nuget Package Manager->Package Manager Console.
* In the package manager console change the project to *dataChecker.Persistance*
* In visual studeio solutions Explorer delate the DataChecker.Persistanc-> Migrations folder if it exists.
* In the package manager console run: Add-Migration MyMigration -context DatabaseContext
* Then run: Update-Database -context DatabaseContext
* Deploy the WebApp to a folder in the IIS root and convert it into an application in IIS

## Docker Install ##
The data checker web app is available as a docker container. At this time there is not a containerized version of the database.
There is a sample docker-compose.yml file and both a postgres and SqlServer version of the database that can be found attached to the latest release in github.

# Quick start guide #
* Set up an environment that connects to an ODS you want to examine. It is highly recommended that the credentials used to connect to that ODS have read-only access.
* Pull in an existing collections of rules from https://github.com/Ed-Fi-Exchange-OSS/DataChecker-Collections
* More detailed user instructions can be found at: [Data Checker Overview and User Guide](https://docs.google.com/document/d/17FkjSqg55-MOvFxpmbAZ06okIdxyjXhHoN4DnxgIC8A/)
# Support #
* For Questions,or  comments, please use the #users-datachecker channel on the Ed-Fi Alliance Slack workspace.
* For support requests or bugs please open a ticket with the Ed-Fi support group with Data Checker in the title.


## Legal Information

Copyright (c) 2021 Ed-Fi Alliance, LLC and contributors.

Licensed under the [Apache License, Version 2.0](LICENSE) (the "License").

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.

See [NOTICES](NOTICES.md) for additional copyright and license notifications.
