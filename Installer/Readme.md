Ed-Fi ODS Data Checker
============

Description
------------
Install From Binaries

Setup
------------

### Prerequisites ###

* Basic knowledge of compiling .net Core applications.  
* Basic to mid skills on hosting application on Windows IIS Server
* Visual Studio 2019 Community edition. ([https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/))
* .Net Core 3.1
* .Net core 3.1 hosting bundle ( [https://dotnet.microsoft.com/download/dotnet-core/3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) , [https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.3-windows-hosting-bundle-installer](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.3-windows-hosting-bundle-installer) )
* NodeJs LTS ([https://nodejs.org/en/](https://nodejs.org/en/))
* Angular CLI ([https://cli.angular.io/](https://cli.angular.io/)) npm install -g @angular/cli
* Acquiring the codebase: [https://github.com/Ed-Fi-Alliance/Ed-Fi-X-DataChecker/](https://github.com/Ed-Fi-Alliance/Ed-Fi-X-DataChecker/)
    

### Download Binaries ###

* Download the file Binaries.rar: https://github.com/Ed-Fi-Exchange-OSS/DataChecker/tree/main/Installer 
* You can validate if you have all your prerequisites installed running the powershell script VerifyPreReqs.ps1
* Uncompress the file and copy the folder DataChecker in the path: C:\inetpub\wwwroot\DataChecker
* Restore the database structure dacpac file in MSQL Server
* Update the connection string of the file appsettings.Production.json
* Open IIS and convert the folder DataChecker from the path C:\inetpub\wwwroot\DataChecker to a WebApplication.

## License

Licensed under the [Ed-Fi
License](https://www.ed-fi.org/getting-started/license-ed-fi-technology/).
