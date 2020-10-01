# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

############################################################
 
# Author: Douglas Loyo, Sr. Solutions Architect @ MSDF
 
# Description: Downloads Ed-Fi binaries from the published MyGet feed and installs them.
#              After install it does appropriate configuration to have applications running.

# Note: This powershell has to be ran with Elevated Permissions (As Administrator) and in a x64 environment.
# Know issues and future todo's: (look at the .PSM1 file)
 
############################################################

$param1 = $args[0]

Import-Module "$PSScriptRoot\BinaryInstall1" -Force #-Verbose #-Force

if($param1){

	Write-HostInfo "Data-Check binary installer functions loaded correctly."

	Write-HostStep "Installing App..."

	Install-App $param1

	Write-HostStep "Finished"
}
else{
	Write-HostInfo "A connection string is required."
	return;
}
