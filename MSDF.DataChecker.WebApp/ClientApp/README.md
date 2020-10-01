Ed-Fi ODS Data Checker
============

Description
------------
A very simple data checker that will have a set of files defining SQL statements to do level 2 validations on the ODS database

Live Demo
------------

[http://datacheck.toolwise.net/](http://datacheck.toolwise.net/)

Setup
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
* Acquiring the codebase: [https://github.com/Ed-Fi-Alliance/Ed-Fi-X-DataChecker/](https://github.com/Ed-Fi-Alliance/Ed-Fi-X-DataChecker/)
    
**NOTE VERY IMPORTANT: The user that you will use to configure the execution environment should be read only.**

### Download or Clone the code to your machine ###

Clone or download the code located in the github repository: https://github.com/Ed-Fi-Alliance/Ed-Fi-X-DataChecker/ 

*We recommend that you download the code in the following path: c:\Projects\edfi\DataChecker*

To download click on this link and save to the folder where you wish to download. https://github.com/Ed-Fi-Alliance/Ed-Fi-X-DataChecker/archive/master.zip

![](https://drive.google.com/uc?export=view&id=1LtdR_KQEAumchiPDSmrXyEiN4KM_Puad)

To clone open your favorite shell, navigate to the path above or folder where you wish to download and then type in the following command: 

*c:\projects\edfi\> git clone https://github.com/Ed-Fi-Alliance/Ed-Fi-X-DataChecker.git*

![](https://drive.google.com/uc?export=view&id=1Y9cnuKUd_OpetNMK0Og07JDDlvxt_DO7)

### Building the Binaries ###

1) Open the solution file: Once you have the code on your local machine, open Visual Studio and the open the solution file located at *c:\projects\edfi\DataChecker\MSDF.DataChecker.sln*

![](https://drive.google.com/uc?export=view&id=1DFE5RKkmplR__Y8cWjSoCQR-gDPFzT3S)

2) Ensure the solution builds: Right click the *“MSDF.DataChecker.WebApp”* project and click rebuild. If you get any errors it's probably because you are missing one of the dependencies or prerequisites listed above.

![](https://drive.google.com/uc?export=view&id=179QUdy15gIxmGBM1IHg70T4IH2GMHMqK)

### Configuring the Application Settings ###
In a .net Core application there are 2 files called “appsettings.json” and "appsettings.Production.json". This is where you need to ensure to configure the application’s settings. In this case we only need to update your database connection string. Data Checker supports both MsSQL security and Windows Integrated security. Update the connection string to suit your use case. In the example below we are using a local SQL server instance with SQL credentials.

![](https://drive.google.com/uc?export=view&id=1Hv8WXm3w_h2Z-FrtBfbm-oDxLpBDCj8X)

### Running Data Checker in the Development Environment ###

1.  Make sure you have create schema or admin access to a MsSQL server instance so that we can create the database that supports the solution.
2.  Ensure you update the connection string in the “appsettings.json”.
3.  In Visual Studio open the “Package Manager Console”. Usually located in the menu: Tools->Nuget Package Manager->Package Manager Console.

![](https://drive.google.com/uc?export=view&id=14__G-0-lXF8E4btKXcdA07WQ5YD6_5QI)

4.  Ensure you have set the MSDF.DataChecker.WebApp project as the startup project by right clicking the project and selecting “Set as Startup Project”.

![](https://drive.google.com/uc?export=view&id=1lWY3rqJeu6ZJ7hTsoundXHZV0-VA1lih)
    
5.  In the Package Manager Console ensure you have selected the WebApp project in the dropdown for “Default project:” and then run the command “Update-Database”. This will execute the Entity Framework Code First migrations and create the database for you.

![](https://drive.google.com/uc?export=view&id=17Gg7dbpiGy44uTvfWigCnjR8fhMB2IMZ)

6. Once you have created the database then you can click on IIS Express to run the application.

![](https://drive.google.com/uc?export=view&id=1Tgzr5YAtob5RdziAczkWj5sUgeblIrcq)

### Installing in IIS ###

For beta users we are not providing a quick install but they are coming soon.

We recommend you review the following article by microsoft for hosting .Net Core Applications: [https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.1](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.1)

#### Publishing to IIS ####

1. Publish to your IIS: 
Right click on the web project and select Publish option

![](https://drive.google.com/uc?export=view&id=1E4T6hKLXjbb6-zhNrFkuz5n3iXiOH4v2)

Select folder type and the path to were the files will be published and click Create Profile
![](https://drive.google.com/uc?export=view&id=1skE-tjTv6gza6HMA2iakNjgfFqFLhJ_x)

Select the configuration Publish and the option "Delete all existing files prior to publish" and click Save
![](https://drive.google.com/uc?export=view&id=1SbWl4SYlfzW7dT1xcvA2654DuU0DiA-s)

Click on Publish
![](https://drive.google.com/uc?export=view&id=1T9w3Qk_Az807zqjwlUrZynu4uENfgRrA)

After the Publish you will see this at the bottom of your output window

![](https://drive.google.com/uc?export=view&id=1x3h4RltZHmNwnViY54FtWdXRkrpEKcW4)

2. Open IIS and select the folder where the files were published and select Convert to Application
![](https://drive.google.com/uc?export=view&id=1IOA8wgrtDTZKHiOIPoGm0E4Q30BMTub3)

You will be able to see the application running on the url [https://localhost/DataCheck/](https://localhost/DataCheck/)

![](https://drive.google.com/uc?export=view&id=1AxYkWTixg6_qD5i9PPG3AZqnj6zphQAq)

#### Configure the Client Web Application ####

If you want to change the name of the application "DataCheck" you need to update the following files with the name you want:  

**environment.prodlocal.ts**

![](https://drive.google.com/uc?export=view&id=14iWDXFj5LYPNdKAQhUB8gDzuIxSDj08Q)

![](https://drive.google.com/uc?export=view&id=1WN46ppNj617C0jpeod8_wvNNLd-IW4DE)

**package.json**

![](https://drive.google.com/uc?export=view&id=1XxGiFztk8whhgiZO0p4QNCrXKg8tw0ei)

![](https://drive.google.com/uc?export=view&id=12VStT7LM95bnHWbngA28hPx9OGO-JlgU)

### Need to schedule it? ###
We provide a Console application that you can schedule with "Windows Task Scheduler".

1. Build the project MSDF.DataChecker.cmd with the Release configuration, change the connection string of the file "appsettings.json" if need it.
2. After the build, you will have in the folder Release all the necessary files to execute the program that you can use in the "Windows Task Scheduler".
3. The required parameters are: "--environmentid" and "--environmentname". If you want to run an entire container you need to add the parameter "--containerid" or if you only want to run a rule then you need to add the parameter "--ruleid".

**Example of Use:**
MSDF.DataChecker.cmd.exe --environmentid "1451E793-F100-4A91-845E-4E45130DCF31" --environmentname "V25 Douglas" --ruleid "8D7363E5-A98B-BB74-5179-1207FFC18A1F"

## License

Licensed under the [Ed-Fi
License](https://www.ed-fi.org/getting-started/license-ed-fi-technology/).
