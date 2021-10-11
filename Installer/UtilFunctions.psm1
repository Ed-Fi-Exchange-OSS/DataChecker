# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

############################################################
 
# Description: Downloads Ed-Fi binaries from the published MyGet feed and installs them.
#              After install it does appropriate configuration to have applications running.

# Note: This powershell has to be ran with Elevated Permissions (As Administrator) and in a x64 environment.
# Know issues and future todo's:
#   1) What about DSC? Should we contemplate Desired State Configuration? Should Ed-Fi Rereqs be verified and or installed?
#      Look at: Install-EdFiPrerequisites()
#   2) TODO: As of now, the code does not inspect MsSQL server data and log file locations. You have to provide them manually.
#   3) TODO: As of now you can not provide a MsSQL connection string and only does local "."
 
############################################################

#load assemblies
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
#Need SmoExtended for backups
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null

# Helper functions
Function Write-HostInfo($message) { 
    $divider = "----"
    for($i=0;$i -lt $message.length;$i++){ $divider += "-" }
    Write-Host $divider -ForegroundColor Cyan
    Write-Host " " $message -ForegroundColor Cyan
    Write-Host $divider -ForegroundColor Cyan 
}

Function Write-HostStep($message) { 
    Write-Host "*** " $message " ***"-ForegroundColor Green
}

Function Verify-IISPrerequisites() {
    $allPreReqsInstalled = $true;
    # Throw this infront 'IIS-ASP', to make fail.
    $prereqs = @('IIS-WebServerRole','IIS-WebServer','IIS-CommonHttpFeatures','IIS-HttpErrors','IIS-ApplicationDevelopment','NetFx4Extended-ASPNET45','IIS-NetFxExtensibility45','IIS-HealthAndDiagnostics','IIS-HttpLogging','IIS-Security','IIS-RequestFiltering','IIS-Performance','IIS-WebServerManagementTools','IIS-ManagementConsole','IIS-StaticContent','IIS-DefaultDocument','IIS-ISAPIExtensions','IIS-ISAPIFilter','IIS-HttpCompressionStatic','IIS-ASPNET45');
    # 'IIS-IIS6ManagementCompatibility','IIS-Metabase', 'IIS-HttpRedirect', 'IIS-LoggingLibraries','IIS-RequestMonitor''IIS-HttpTracing','IIS-WebSockets', 'IIS-ApplicationInit'?

    foreach($p in $prereqs)
    {
        if((Get-WindowsOptionalFeature -Online -FeatureName $p).State -eq "Disabled") { $allPreReqsInstalled = $false; Write-Host "Prerequisite not installed: $p" }
    }
	
	if($allPreReqsInstalled -eq $TRUE){
		Write-Host "IIS PreReq are installed."
	}
}

Function Verify-NodeJS(){

	$installNode = $FALSE
	
	if (Get-Command node -errorAction SilentlyContinue) {
		$current_version = (node -v)
		Write-Host "NodeJS is installed."
	}
	else{
		$allPreReqsInstalled = $false;
		$installNode = $TRUE
	}

	if($installNode -eq $TRUE){
		Write-Host "NodeJS is not installed."
	}
}

Function Verify-DotNetCore(){

	$installDotNetCore = $FALSE
	
	if (Get-Command dotnet -errorAction SilentlyContinue) {
		$current_version = (dotnet --list-sdks)
		Write-Host ".Net Core is installed."
	}
	else{
		$allPreReqsInstalled = $false;
		$installDotNetCore = $TRUE
	}

	if($installDotNetCore -eq $TRUE){
		Write-Host ".Net Core is not installed."
	}
}

Function Verify-AngularCli(){

	$installComponent = $FALSE
	
	if (Get-Command ng -errorAction SilentlyContinue) {
		$current_version = (ng --version)
		Write-Host "Angular/CLI is installed."
	}
	else{
		$allPreReqsInstalled = $false;
		$installComponent = $TRUE
	}

	if($installComponent -eq $TRUE){
		Write-Host "Angular/CLI is not installed."
	}
}

Function Verify-DataCheckPrerequisites() {

    $allPreReqsInstalled = $true
    
    Write-Host "Ensuring all Prerequisites are installed:"

    # Ensure the following are installed.
    Verify-IISPrerequisites    
    Verify-NodeJS
	Verify-DotNetCore
	Verify-AngularCli    

    # If not all Pre Reqs installed halt!
    if(!$allPreReqsInstalled){ 
		Write-Error "Error: Missing Prerequisites. Look above for list." -ErrorAction Stop 
	}
}



