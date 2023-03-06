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
* Download the file DataChecker.zip and either the DataCheckerPersistance.bacpac (for SqlServer) or DataCheckerPersistance.dump (for postgres) from the latest release here: https://github.com/Ed-Fi-Exchange-OSS/DataChecker/releases
* Unzip DataChecker.zip to a local folder
* Validate if you have all your prerequisites installed running the powershell script VerifyPreReqs.ps1
* Copy the folder DataChecker in the path: C:\inetpub\wwwroot\DataChecker or other suitable location for the web application
* Find that folder in IIS, right click and 'convert to application'
* If using MS Sql Server, Restore the dataCheckerPersitance.bacpac file.
* Otherwise if using Posgres, use pg_restore and the DataCheckerPersistance.dump to restore the database. .
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
* Ensure the solution builds: Right click the *"MSDF.DataChecker.WebApp"* project and click rebuild. If you get any errors it's probably because you are missing one of the dependencies or prerequisites listed above.
* Right Click on the WebApp folder and set it as the startup project.
* In the WebApp project, configure both "appsettings.json" and "appsettings.Production.json". Specify your database engine (postgres of SqlServer) and your database connection string.
* In visual studio open: Tools->Nuget Package Manager->Package Manager Console.
* In the package manager console change the project to *dataChecker.Persistance*
* If and only if this is an upgrade of an exisitng system, in the package manager console run: `Add-Migration MyMigration -context DatabaseContext` 
* Run`Update-Database -context DatabaseContext` to update the data base with the Migration files( DataChecker.Persistanc-> Migrations)
* Deploy the WebApp to a folder in the IIS root and convert it into an application in IIS.

## Docker Install ##
The data checker web app is available as a docker container. At this time there is not a containerized version of the database.
There is a sample docker-compose.yml file and both a postgres and SqlServer version of the database that can be found attached to the latest release in github.


## Securing the Data Checker Web App ##

At this time, data checker does not have an internal security (authentication and authorization) method built in.  Data checker has the potential of having access to protected student data, this means that the application needs to be installed in such a a way that the web application and the database are not available unauthorized access. We recommend the following strategies.
### Secure the database
The Data Checker database has validation results and should be subject to the same security practices as the ODS.
### Eliminate outside access to the data checker web app
Like other internal EdFI tools (Admin App, Data Import), the data checker should not be on a web site that is available from the public internet. 
### Enable windows authentication for the data checker web app
As an added level of security, windows authentication should be considered for the web app.  This is done with the following steps.

* Under server manger, go to the IIS role, open the security folder, and make sure that the optional feature 'Windows Authentication' is installed.
* Restart IIS
* In IIS, select the data checker web app and double click the 'authentication' icon on the right.  Change "Anonymous Authentication" to disabled and "Windows Authentication" to enabled. Close the authentication tab.
* Right click on the data checker web application, and choose "Edit Permissions..."
* Go to the security tab
* Click on the 'advanced' button
* Click 'Disable Inheritance' , and choose the option to convert the inherited permissions to explicit permissions on that object. 
* Delete the access for the 'Users' group
* Add a windows security group that includes the people that should be able to access the application and close this screen.
* Test access using incognito mode in your browser to make sure that authentication is required.

# Quick start guide #
* Set up an environment that connects to an ODS you want to examine. It is highly recommended that the credentials used to connect to that ODS have read-only access.
* Pull in an existing collections of rules from https://github.com/Ed-Fi-Exchange-OSS/DataChecker-Collections
* More detailed user instructions can be found at: [Data Checker Overview and User Guide](https://docs.google.com/document/d/17FkjSqg55-MOvFxpmbAZ06okIdxyjXhHoN4DnxgIC8A/)

# Known Issues #
## Why is Data Checker only showing a blank page?
The issue could be caused because we have different version of Angular Clic and Node in our environment.
Data checker needs:

Angular CLI: 14.1.0 

Node: you need a version compatible with Angular CLI: 14.1.0 , Node 16.16.0 works well.

Run the commands:

`ng version` : to know what angular version you have installed.
`node -v` : to know what version of Node you have installed.

If we have different versions, uninstall and Install the correct versions.

`npm i -g node@16.16.0`
`npm install -g @angular/cli@14.1.0`

 ### Before you run the application.
 1) Compile the the solution. This process will install all the packages that the application needs to run.

    if for some reason It failed to install a package, try to install manually.<br>
     a) go to ..\DataChecker\MSDF.DataChecker.WebApp\ClientApp directory and delete the package-lock.json file and the node_modules folder.<br>
     b) Open PowerShell and navigate to your directory :  `MSDF.DataChecker.WebApp\ClientApp `<br>
     c) Run the command npm install -force

 2) Create your Data Base running the Migrations files.<br>
    a) In your visual Studio , open a Package Manager Console.<br>
    b) Depending on your connection string,select `MSDF.DataChecker.Migrations.SqlServer` or `MSDF.DataChecker.Migrations.Postgres`        
    c) Run the command `Update-Database -context DatabaseContext`.<br>
    d) Open your Sql Server Management Studio and ensure your data base was created.<br>
    
    Note: if you get the error Your target project 'MSDF.DataChecker.Migrations' doesn't match your migrations assembly ....
    ![image](https://user-images.githubusercontent.com/85459544/223282457-b05150bd-5cea-4336-8650-1216132e23f0.png)
          Make sure your connection Engine match with your Migration Assembly.<br>
          "Engine": "Postgres" —>MSDF.DataChecker.Migrations.Postgres<br>
          "Engine": "SqlServer" —>MSDF.DataChecker.Migrations.SqlServer<br>
        ![image](https://user-images.githubusercontent.com/85459544/223282673-3543bc50-87f9-4cb8-a0a9-a1e4d86cd29e.png)

 3) Run Data checker<br>
    a) Change all your connections strings<br>
    b) If your are going to debug Data Checker, ensure your ASPNETCORE_ENVIRONMENT in `MSDF.DataChecker.WebApp\Properties\launchSettings.json` is configured as                   Development (`"ASPNETCORE_ENVIRONMENT": "Development"`)<br>
    c) Right Click on the `MSDF.DataChecker.WebApp` project and set it as the startup project.<br>
    d) Run the application, instead to run with IIS express, select` MSDF.DataChecker.WebApp`<br>
    
    If everything works well, you are going to see a console application like this.

![image](https://user-images.githubusercontent.com/85459544/221946710-f5e361e0-57e6-42b4-8284-3cb0fb92bd30.png)
Note: if for some reason, in the console application you see something like this and your browser never open DataChecker, just close the console application and run DataChecker again.
![image](https://user-images.githubusercontent.com/85459544/221947047-2f524334-2789-4fc2-82ca-916d76dd74f5.png)


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
