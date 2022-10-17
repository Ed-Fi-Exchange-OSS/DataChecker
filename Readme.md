﻿Ed-Fi ODS Data Checker
============

Description
------------
A very simple data checker that will have a set of files defining SQL statements to do level 2 validations on the ODS database

Binary Installs and upgrades
------------

### Prerequisites ###

* Install the IIS Features to enable the IIS Server:  
![](https://drive.google.com/uc?export=view&id=1QIiweGLsmqEqRTRRY0X_u0P_ohRnHK1I)
* Install .Net Core 2.2.5 Hosting Bundle (https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-2.2.5-windows-hosting-bundle-installer)
* Install .Net Core 3.1 Hosting Bundle (https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-3.1.20-windows-hosting-bundle-installer)
* SQL Server (https://www.microsoft.com/sql-server/sql-server-downloads)
* SQL Server Management Studio (SSMS) (https://aka.ms/ssmsfullsetup)

### Fresh Installation from binaries ###
* Download the file Binaries.zip from the latest release here: https://github.com/Ed-Fi-Exchange-OSS/DataChecker/releases
* Unzip Binaries.zip to a local folder
* Validate if you have all your prerequisites installed running the powershell script VerifyPreReqs.ps1
* Copy the folder DataChecker in the path: C:\inetpub\wwwroot\DataChecker
* Restore the database structure bacpac file in MSQL Server
* Open the file C:\inetpub\wwwroot\DataChecker\appsettings.json
	* Change the value "DataCheckerStore" to a valid connection string for the datachecker database
	* Change the value of "EncryptionKey" from "CHANGE_ME_PLEASE!" to a random text string. This will impact how ODS connection string passwords are st
* Open IIS and convert the folder DataChecker from the path C:\inetpub\wwwroot\DataChecker to a WebApplication.
* If you have an error regarding the login of the IIS APPPOOL\DefaultAppPool you could do the following steps:
    * In SQL Server Management Studio, look for the Security folder (the security folder at the same level as the Databases, Server Objects, etc. folders...not the security folder within each individual database)
    * Right click logins and select "New Login"
    * In the Login name field, type IIS APPPOOL\DefaultAppPool - do not click search
    * Fill whatever other values you like (i.e., authentication type, default database, etc.)
    * Click OK

### Upgrade from version 1.0 with Binaries ###
* Download the file Binaries.zip from the latest release here: https://github.com/Ed-Fi-Exchange-OSS/DataChecker/releases
* Unzip Binaries.zip to a local folder
* Validate if you have all your prerequisites installed running the powershell script VerifyPreReqs.ps1
* Copy the folder DataChecker in the path: C:\inetpub\wwwroot\DataChecker (presumedly replacing the old version)
* Run the following command in the data checker database:

    alter table datachecker.containers add DateUpdated datetime2;
    alter table datachecker.databaseEnvironments add TimeoutInMinutes int;
    alter table datachecker.Rules add DateUpdated datetime2;

* Update the connection string of the file C:\inetpub\wwwroot\DataChecker\appsettings.json
* Change the value of "EncryptionKey" from "CHANGE_ME_PLEASE!" to a random text string. This will impact how ODS connection string passwords are st
* Open IIS and convert the folder DataChecker from the path C:\inetpub\wwwroot\DataChecker to a WebApplication.
* If you have an error regarding the login of the IIS APPPOOL\DefaultAppPool you could do the following steps:
    * In SQL Server Management Studio, look for the Security folder (the security folder at the same level as the Databases, Server Objects, etc. folders...not the security folder within each individual database)
    * Right click logins and select "New Login"
    * In the Login name field, type IIS APPPOOL\DefaultAppPool - do not click search
    * Fill whatever other values you like (i.e., authentication type, default database, etc.)
    * Click OK

### Quick start guide ###
* Set up an environment that connects to an ODS you want to manage. It is highly recommended that the credentials used to connect to that ODS have read-only access.
* Pull in an existing collections of rules from https://github.com/Ed-Fi-Exchange-OSS/DataChecker-Collections
* More detailed user instructions can be found at: [Data Checker Overview and User Guide](https://docs.google.com/document/d/17FkjSqg55-MOvFxpmbAZ06okIdxyjXhHoN4DnxgIC8A/)

Build from source with Visual Studio
------------

There are 2 ways of running the *Data Checker*. In development or production mode. The main difference is that Development will run from your Visual Studio in IIS Express. While Production we will deploy to a local IIS.

For both environments you will need to complete the following common prerequisites:

### Prerequisites ###

* Basic knowledge of compiling .net Core applications.  
* Basic to mid skills on hosting application on Windows IIS Server
* Visual Studio 2019 Community edition. ([https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/))
* .Net Core 3.1
* .Net core 3.1 hosting bundle ( [https://dotnet.microsoft.com/download/dotnet-core/3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) , [https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.3-windows-hosting-bundle-installer](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.3-windows-hosting-bundle-installer) )
* NodeJs LTS ([https://nodejs.org/en/](https://nodejs.org/en/))
* Angular CLI ([https://cli.angular.io/](https://cli.angular.io/)) npm install -g @angular/cli
* Acquiring the codebase: [https://github.com/Ed-Fi-Exchange-OSS/DataChecker/](https://github.com/Ed-Fi-Exchange-OSS/DataChecker/)
    
**NOTE VERY IMPORTANT: The user that you will use to configure the execution environment should be read only.**

### Download or Clone the code to your machine ###

<<<<<<< HEAD
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
* Deploy the WebApp to a folder in the IIS root and convert it into an application in IIS.
=======
Clone or download the code located in the github repository: https://github.com/Ed-Fi-Exchange-OSS/DataChecker/
>>>>>>> parent of 90e1c37 (updated readme)

*We recommend that you download the code in the following path: c:\Projects\edfi\DataChecker*


To clone open your favorite shell, navigate to the path above or folder where you wish to download and then type in the following command: 

*c:\projects\edfi\> git clone https://github.com/Ed-Fi-Exchange-OSS/DataChecker*

![](https://drive.google.com/uc?export=view&id=1jYAZpnSJcNego1bb2JnuIOetO03t2Vua)

### Building the Binaries ###

1) Open the solution file: Once you have the code on your local machine, open Visual Studio and the open the solution file located at *c:\projects\edfi\DataChecker\MSDF.DataChecker.sln*

![](https://drive.google.com/uc?export=view&id=14cLuzjKSePlPYLiWzu-BoNX9llcPsB9m)

2) Ensure the solution builds: Right click the *“MSDF.DataChecker.WebApp”* project and click rebuild. If you get any errors it's probably because you are missing one of the dependencies or prerequisites listed above.

![](https://drive.google.com/uc?export=view&id=1jXaf0R6zZX_fViI5NPKMptm0-QsKUOlp)

### Configuring the Application Settings ###
In a .net Core application there are 2 files called “appsettings.json” and "appsettings.Production.json". This is where you need to ensure to configure the application’s settings. In this case we only need to update your database connection string. Data Checker supports both MsSQL security and Windows Integrated security. Update the connection string to suit your use case. In the example below we are using a local SQL server instance with SQL credentials.

![](https://drive.google.com/uc?export=view&id=1elvcdjHuBTkrfDC0nvCnkRp6P7YKJxdV)

### Running Data Checker in the Development Environment ###

1.  Make sure you have create schema or admin access to a MsSQL server instance so that we can create the database that supports the solution.
2.  Ensure you update the connection string in the “appsettings.json”.
3.  In Visual Studio open the “Package Manager Console”. Usually located in the menu: Tools->Nuget Package Manager->Package Manager Console.

![](https://drive.google.com/uc?export=view&id=1iwDv4auo48CxUAfX_3nXtG5p3iKDyhuc)

4.  Ensure you have set the MSDF.DataChecker.WebApp project as the startup project by right clicking the project and selecting “Set as Startup Project”.

![](https://drive.google.com/uc?export=view&id=1SjaEnzTka-uCHXlhmKj2zbEjDUhMx994)
    
5.  In the Package Manager Console ensure you have selected the WebApp project in the dropdown for “Default project:” and then run the command “Update-Database”. This will execute the Entity Framework Code First migrations and create the database for you.

![](https://drive.google.com/uc?export=view&id=1xnQJhRWFf4Rl8I7XnXiU2cSPtdC7tzeM)

6. Once you have created the database then you can click on IIS Express to run the application.

![](https://drive.google.com/uc?export=view&id=1wEjJJMhVLw-x1COY4NkUxXqec9pl1m2i)

### Installing in IIS ###

For beta users we are not providing a quick install but they are coming soon.

We recommend you review the following article by microsoft for hosting .Net Core Applications: [https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.1](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.1)

#### Publishing to IIS ####

1. Publish to your IIS: 
Right click on the web project and select Publish option

![](https://drive.google.com/uc?export=view&id=1_CdayEWsp3BMH5y_xIKsf-w8HfEM5wrw)

Select folder type and the path to were the files will be published and click Create Profile
![](https://drive.google.com/uc?export=view&id=1roOWB5ZT37xBI8PiBXnD-znfw2Sy7BsD)

Select the configuration Publish and the option "Delete all existing files prior to publish" and click Save
![](https://drive.google.com/uc?export=view&id=1POFx6WwD13eruA1MSg6mDdV6FawNCgl5)

Click on Publish
![](https://drive.google.com/uc?export=view&id=1lSs0aSD7UrxnkJIjRhy4_eePiWJdD4Hw)

After the Publish you will see this at the bottom of your output window

![](https://drive.google.com/uc?export=view&id=12QffibsX3KdM1_Es8YgEnUo9fChZI0Sb)

2. Open IIS and select the folder where the files were published and select Convert to Application
![](https://drive.google.com/uc?export=view&id=1XrrYCo5lIJJBneu0AKyUN5w-CdbmxXX-)

You will be able to see the application running on the url [https://localhost/DataCheck/](https://localhost/DataCheck/)

![](https://drive.google.com/uc?export=view&id=1hESvwNmwX1ofF36TBfVy4VzzP5jBRT9i)

#### Configure the Client Web Application ####

If you want to change the name of the application "DataCheck" you need to update the following files with the name you want:  

**environment.prodlocal.ts**

![](https://drive.google.com/uc?export=view&id=1ww5VgKstSqitfunso1DJG8awKwAGrLMh)

![](https://drive.google.com/uc?export=view&id=1G6ElzXjiWMmJulFsvR-7KEfgCFbe1gO8)

**package.json**

![](https://drive.google.com/uc?export=view&id=1tgHaGpCyfZt1kIOGAE602pB_JLzWrPya)

![](https://drive.google.com/uc?export=view&id=1UKI-lSO90dX9Oagd7Cbv0dDRYLemzQaX)

### Need to schedule it? ###
We provide a Console application that you can schedule with "Windows Task Scheduler".

1. Build the project MSDF.DataChecker.cmd with the Release configuration, change the connection string of the file "appsettings.json" if need it.
2. After the build, you will have in the folder Release all the necessary files to execute the program that you can use in the "Windows Task Scheduler".
3. The required parameters are: "--environmentid" and "--environmentname". If you want to run an entire container you need to add the parameter "--containerid" or if you only want to run a rule then you need to add the parameter "--ruleid".

**Example of Use:**
MSDF.DataChecker.cmd.exe --environmentid "1451E793-F100-4A91-845E-4E45130DCF31" --environmentname "V25 Douglas" --ruleid "8D7363E5-A98B-BB74-5179-1207FFC18A1F"

### Support ###
* Questions, comments, bug reports, features requests, or general feedback please use the #users-datachecker channel on the Ed-Fi Alliance Slack workspace or send an email to jon -at- redglobeinc.com

## Legal Information

Copyright (c) 2021 Ed-Fi Alliance, LLC and contributors.

Licensed under the [Apache License, Version 2.0](LICENSE) (the "License").

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.

See [NOTICES](NOTICES.md) for additional copyright and license notifications.
