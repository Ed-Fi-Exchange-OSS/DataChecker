# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

############################################################
 
# Author: Douglas Loyo, Sr. Solutions Architect @ MSDF
 
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

# Ensure all prerequisites are installed.
Function Install-Chocolatey(){
    if(!(Test-Path "$($env:ProgramData)\chocolatey\choco.exe"))
    {
        Write-Host "Installing: Cocholatey..."
        Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
    }else{Write-Host "Skipping: Cocholatey is already installed."}
}

# Region: IIS Functions
Function Install-IISPrerequisites() {
    $allPreReqsInstalled = $true;
    # Throw this infront 'IIS-ASP', to make fail.
    $prereqs = @('IIS-WebServerRole','IIS-WebServer','IIS-CommonHttpFeatures','IIS-HttpErrors','IIS-ApplicationDevelopment','NetFx4Extended-ASPNET45','IIS-NetFxExtensibility45','IIS-HealthAndDiagnostics','IIS-HttpLogging','IIS-Security','IIS-RequestFiltering','IIS-Performance','IIS-WebServerManagementTools','IIS-ManagementConsole','IIS-BasicAuthentication','IIS-WindowsAuthentication','IIS-StaticContent','IIS-DefaultDocument','IIS-ISAPIExtensions','IIS-ISAPIFilter','IIS-HttpCompressionStatic','IIS-ASPNET45');
    # 'IIS-IIS6ManagementCompatibility','IIS-Metabase', 'IIS-HttpRedirect', 'IIS-LoggingLibraries','IIS-RequestMonitor''IIS-HttpTracing','IIS-WebSockets', 'IIS-ApplicationInit'?

    Write-Host "Ensuring all IIS prerequisites are already installed."
    foreach($p in $prereqs)
    {
        if((Get-WindowsOptionalFeature -Online -FeatureName $p).State -eq "Disabled") { $allPreReqsInstalled = $false; Write-Host "Prerequisite not installed: $p" }
    }

    if($allPreReqsInstalled){ Write-Host "Skipping: All IIS prerequisites are already installed." }
    else { Enable-WindowsOptionalFeature -Online -FeatureName $prereqs }
}

Function Install-IISUrlRewrite() {
# URLRewrite Module (File exists or Registry entry?)
    if(!(Test-Path "$env:SystemRoot\system32\inetsrv\rewrite.dll")) { 
        Write-Host "     Installing: Url-Rewrite Module..."
        choco install urlrewrite /y    
    }else { Write-Host "     Skipping: UrlRewrite module is already installed." }
}

Function IsNetVersionInstalled($major, $minor){
    $DotNetInstallationInfo = Get-ChildItem -Path 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP' -Recurse
    $InstalledDotNetVersions = $DotNetInstallationInfo | Get-ItemProperty -Name 'Version' -ErrorAction SilentlyContinue
    $InstalledVersionNumbers = $InstalledDotNetVersions | ForEach-Object {$_.Version -as [System.Version]}
    #$InstalledVersionNumbers;
    $Installed3Point5Versions = $InstalledVersionNumbers | Where-Object {$_.Major -eq $major -and $_.Minor -eq $minor}
    $DotNet3Point5IsInstalled = $Installed3Point5Versions.Count -ge 1
    return $DotNet3Point5IsInstalled
}

#endregion

Function Invoke-DownloadFile($url, $outputpath) {
    Invoke-WebRequest -Uri $url -OutFile $outputpath
}

# Region: MsSQL Database Functions
Function Add-SQLUser($serverInstance, $User, $Role) {
    $server = New-Object Microsoft.SqlServer.Management.Smo.Server "."
    if ($server.Logins.Contains($User)) { Write-Host "     Skipping: User '$User' already part of the MsSQL Logins" }
    else {
        # Add the WindowsUser
        $SqlUser = New-Object -TypeName Microsoft.SqlServer.Management.Smo.Login -ArgumentList $server, $User
        $SqlUser.LoginType = 'WindowsUser'
        $sqlUser.PasswordPolicyEnforced = $false
        $SqlUser.Create()

        # Add to the role.
        $serverRole = $server.Roles | where {$_.Name -eq $Role}
        $serverRole.AddMember("$User")
    }
}
# endregion

Function Set-PermissionsOnPath($path, $user, $permision){
    $acl = Get-Acl $path
    $ar = New-Object System.Security.AccessControl.FileSystemAccessRule($user, $permision, "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($ar)
    Set-Acl $path $acl
}

Function Install-NodeJSWithDownload(){

	$version = "13.13.0-x64"
	$url = "https://nodejs.org/dist/latest-v13.x/node-v$version.msi"
	$installNode = $FALSE
	
	if (Get-Command node -errorAction SilentlyContinue) {
		$current_version = (node -v)
	}
	else{
		$installNode = $TRUE
	}
	
	if($installNode -eq $TRUE){
	
		Write-Host $installNode
		
		### download nodejs msi file
		# warning : if a node.msi file is already present in the current folder, this script will simply use it
			
		write-host "`n----------------------------"
		write-host "  nodejs msi file retrieving  "
		write-host "----------------------------`n"
		
		$download_node = $TRUE
		$filename = "node.msi"
		$node_msi = "$PSScriptRoot\$filename"

		if (Test-Path $node_msi) {
			$download_node = $FALSE
		}
		
		if ($download_node) {
			write-host "[NODE] downloading nodejs install"
			write-host "url : $url"
			$start_time = Get-Date
			$wc = New-Object System.Net.WebClient
			$wc.DownloadFile($url, $node_msi)
			write-Output "$filename downloaded"
			write-Output "Time taken: $((Get-Date).Subtract($start_time).Seconds) second(s)"
		}
		
		### nodejs install

		write-host "`n----------------------------"
		write-host " nodejs installation  "
		write-host "----------------------------`n"

		write-host "[NODE] running $node_msi"
		Start-Process $node_msi -Wait
		
		$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User") 
    
	}
	else{
		write-host "[NODE] nodejs $current_version already installed"
	}
}

Function Install-NodeJS(){

	$installNode = $FALSE
	
	if (Get-Command node -errorAction SilentlyContinue) {
		$current_version = (node -v)
	}
	else{
		$installNode = $TRUE
	}

	if($installNode -eq $TRUE){
		(choco install nodejs)
		Write-Host "NodeJS installed."
	}
	else{
		Write-Host "Skipping: NodeJS is already installed."
	}
}

Function Install-DotNetCore(){

	$installDotNetCore = $FALSE
	
	if (Get-Command dotnet -errorAction SilentlyContinue) {
		$current_version = (dotnet --list-sdks)
	}
	else{
		$installDotNetCore = $TRUE
	}

	if($installDotNetCore -eq $TRUE){
		(choco install dotnetcore-sdk)
		Write-Host ".Net Core installed."
	}
	else{
		Write-Host "Skipping: .Net Core is already installed."
	}
}

Function Install-EFTools(){

	$installDotNetEF = $FALSE
	
	if (Get-Command dotnet-ef -errorAction SilentlyContinue) {
		$current_version = (dotnet-ef --version)
	}
	else{
		$installDotNetEF = $TRUE
	}

	if($installDotNetEF -eq $TRUE){
		(dotnet tool install --global dotnet-ef)
		Write-Host ".Net EF tools installed."
	}
	else{
		Write-Host "Skipping: .Net EF tools is already installed."
	}
}

Function Install-AngularCli(){

	$installComponent = $FALSE
	
	if (Get-Command ng -errorAction SilentlyContinue) {
		$current_version = (ng --version)
	}
	else{
		$installComponent = $TRUE
	}

	if($installComponent -eq $TRUE){
		(npm install -g @angular/cli)
		Write-Host "Angular/CLI installed."
	}
	else{
		Write-Host "Skipping: Angular/CLI is already installed."
	}
}

Function Install-DataCheckPrerequisites() {

    $allPreReqsInstalled = $true
    
    Write-Host "Ensurering all Prerequisites are installed:"

    # Ensure the following are installed.
    Install-Chocolatey
    Install-IISPrerequisites #TODO uncomment for production
    Install-IISUrlRewrite	  #TODO uncomment for production
    
    Install-NodeJS
	Install-DotNetCore
	Install-EFTools
	Install-AngularCli    

    # MsSQL Server
    if (!(Test-Path 'HKLM:\Software\Microsoft\Microsoft SQL Server\Instance Names\SQL')) { 
		$allPreReqsInstalled = $false; 
		Write-Host "     Prerequisite not installed: MsSQL-Server" 
	}

    # If not all Pre Reqs installed halt!
    if(!$allPreReqsInstalled){ 
		Write-Error "Error: Missing Prerequisites. Look above for list." -ErrorAction Stop 
	}
}

Function Install-App($connectionToUpdate){

	# Initial Parameters and Variables used as Settings
    $installPathForBinaries = "C:\inetpub\wwwroot\DataCheck" # The final path where the binaries will be installed.

    #IIS Settings
    $parentSiteName = "Default Web Site"
    $applicationPool = "DataCheck"
    $virtualDirectoryName = "DataCheck"
    $appsBaseUrl = "https://localhost/$virtualDirectoryName"

    #MsSQL Db Settings
    $sqlServerInstance = "."
    $backupLocation = "$installPathForBinaries\dbs\"
    $dbNamePrefix = ""
    $dbNameSufix = ""
    $integratedSecurityUser = "IIS APPPOOL\DataCheck"
    $integratedSecurityRole = 'sysadmin'
        # SQL Server 2017 Path Variables
    #$dataFileDestination = "C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA"
    #$logFileDestination  = "C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA"
        # SQL Server 2019 Path Variables
    $dataFileDestination = "C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER19\MSSQL\DATA"
    $logFileDestination  = "C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER19\MSSQL\DATA"

    # Other Parameters you should not need to change
    $tempPathForBinaries = "C:\temp\datacheck\binaries\" # The temp path to use to download needed DataCheck binaries.
	
	# Binaries Metadata
    $binaries = @(  
                    @{  name = "DataCheck"; type = "WebApp";
                        requiredInEnvironments = @("Production")
                        url = "https://www.myget.org/F/toolwise/api/v2/package/MSDF.DataChecker.WebApp/1.0.1";
                        iisAuthentication = @{ "anonymousAuthentication" = $true 
                                                "windowsAuthentication"  = $false
                                             }
                    }
                )
				
	#Starting Install
    Write-HostInfo "Installing DataCheck from MyGet feed binaries."
    # 0) Ensure all Prerequisites are installed.
    Write-HostStep "Step: Ensuring all Prerequisites are installed."
    Install-DataCheckPrerequisites
	
	#1) Ensure temp path is accessible and exists if not create it.
    Write-HostStep "Step: Ensuring temp path is accessible. ($tempPathForBinaries)"
    New-Item -ItemType Directory -Force -Path $tempPathForBinaries
	
	#2) Download necesarry binaries and unzip them to its final install location.
    Write-HostStep "Step: Downloading and Unziping all binaries."
    foreach ($b in $binaries) {
        #build path for binay. Note: all NuGet packages are zips.
        $destPath = "$tempPathForBinaries\" + $b.name + ".zip"

        # Optimization (Caching Packages): Check to see if file exists. If it does NOT then download.
		
        if(!(Test-Path $destPath -PathType Leaf)){
            $downloadUrl = $b.url
            Write-Host "     Downloading " $downloadUrl " to -> " $destPath
            Invoke-DownloadFile $downloadUrl $destPath
        }

        #2.1) Once downloaded unzip to install path.		
		$installPath = "$installPathForBinaries\"+$b.name
		Write-Host "     Installing '"$b.name"' to -> $installPath"
		Expand-Archive -LiteralPath $destPath -DestinationPath $installPath -Force     
    }
	
	#2.1) Set folder permissions to the installPathForBinaries so that the IIS_IUSRS can (Read & Execute)
    #     Additionally for the AdminApp setup it is necessary to have the IIS_IUSRS (write) permission.
    Write-HostStep "Step: Setting file system permissions"
    Write-Host "     Setting permissions on: $installPathForBinaries"
    Set-PermissionsOnPath $installPathForBinaries "IIS_IUSRS" "ReadAndExecute"

    #3) Configuring IIS
    #3.1) Installing Virtual Directory
    Write-Host "     IIS Creating Virtual Directory. ($parentSiteName\$virtualDirectoryName)" 
    New-WebVirtualDirectory -Site $parentSiteName -Name $virtualDirectoryName -PhysicalPath "$installPathForBinaries\DataCheck\release" -Force

    #3.2) Installing Web Sites
    # Only look for WebApps within the environment or non-environment specific and create WebApplications
    Write-HostStep "Step: IIS Creating WebApplications and Configuring Authetication Settings"
	New-WebAppPool DataCheck
	
    foreach ($b in $binaries | Where-Object {($_.type -eq "WebApp")}) {
        $appName = $b.name
        $appPhysicalPath = "$installPathForBinaries\"+$b.name+"\release"
		$applicationIISPath = "$parentSiteName/$virtualDirectoryName/$appName/release"
		
        # Create Web Application
        New-WebApplication -Name $appName -Site $parentSiteName -PhysicalPath $appPhysicalPath -ApplicationPool $applicationPool -Force

        # Set IIS Authentication settings
        if($b.iisAuthentication) {
            foreach($key in $b.iisAuthentication.Keys)
            {
                Write-Host "     $key  = " $b.iisAuthentication.Item($key)
                Set-WebConfigurationProperty -Filter "/system.webServer/security/authentication/$key" -Name Enabled -Value $b.iisAuthentication.Item($key) -PSPath IIS:\ -Location "$applicationIISPath"
            }
        }
    }
	
	Import-Module WebAdministration -ErrorAction SilentlyContinue
	$appPool = "IIS:\\AppPools\DataCheck"
	Set-ItemProperty $appPool managedRuntimeVersion ""
	
	#4) Update appsettings.json with connection provided by user
	if($connectionToUpdate){
		Write-HostStep "Step: IIS Configuring appsettings.json properties"
		Get-ChildItem -Path "$installPathForBinaries\DataCheck\release\*" -Include *.json | foreach{
			if($_.Name -match "appsettings."){
				$a = Get-Content $_.FullName | ConvertFrom-Json
				if($a.ConnectionStrings.DataCheckerStore){				
					$a.ConnectionStrings.DataCheckerStore = $connectionToUpdate
					$a | ConvertTo-Json | set-content $_.FullName					
					Write-Host "File Updated: " $_.Name
				}
			}
		}
	}

    #5) Restore Database from DLL
    Write-HostStep "Step: MsSQL Restoring databases"
    
	cd "$installPathForBinaries\DataCheck\release"
	dotnet exec --depsfile MSDF.DataChecker.WebApp.deps.json --runtimeconfig MSDF.DataChecker.WebApp.runtimeconfig.json ef.dll database update --assembly MSDF.DataChecker.WebApp.dll --startup-assembly MSDF.DataChecker.WebApp.dll --root-namespace MSDF.DataChecker.WebApp --verbose

    #6) MsSQL Ensure that the "IIS APPPOOL\DefaultAppPool" user has security login and has Server Roles -> sysadmin.
    Add-SQLUser $sqlServerInstance $integratedSecurityUser $integratedSecurityRole	
}


