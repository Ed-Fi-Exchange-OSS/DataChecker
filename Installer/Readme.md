Ed-Fi ODS Data Checker
============

Description
------------
Install From Binaries

Setup
------------

### Prerequisites ###

* Install the IIS Features to enable the IIS Server:  
![](https://drive.google.com/uc?export=view&id=1QIiweGLsmqEqRTRRY0X_u0P_ohRnHK1I)
* Install 7z to uncompress files (https://www.7-zip.org/download.html)
* Install .Net Core 2.2.5 Hosting Bundle (https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-2.2.5-windows-hosting-bundle-installer)
* Install .Net Core 3.1 Hosting Bundle (https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-3.1.20-windows-hosting-bundle-installer)
* SQL Server (https://www.microsoft.com/sql-server/sql-server-downloads)
* SQL Server Management Studio (SSMS) (https://aka.ms/ssmsfullsetup)

### Download Binaries ###

* Download the file Binaries.rar: https://github.com/Ed-Fi-Exchange-OSS/DataChecker/tree/main/Installer 
* You can validate if you have all your prerequisites installed running the powershell script VerifyPreReqs.ps1
* Uncompress the file and copy the folder DataChecker in the path: C:\inetpub\wwwroot\DataChecker
* Restore the database structure bacpac file in MSQL Server
* Update the connection string of the file appsettings.json
* Open IIS and convert the folder DataChecker from the path C:\inetpub\wwwroot\DataChecker to a WebApplication.
* If you have an error regarding the login of the IIS APPPOOL\DefaultAppPool you could do the following steps:
    * In SQL Server Management Studio, look for the Security folder (the security folder at the same level as the Databases, Server Objects, etc. folders...not the security folder within each individual database)
    * Right click logins and select "New Login"
    * In the Login name field, type IIS APPPOOL\DefaultAppPool - do not click search
    * Fill whatever other values you like (i.e., authentication type, default database, etc.)
    * Click OK

## License

Licensed under the [Ed-Fi
License](https://www.ed-fi.org/getting-started/license-ed-fi-technology/).
