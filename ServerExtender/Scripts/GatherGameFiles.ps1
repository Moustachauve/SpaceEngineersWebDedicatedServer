param (
  [Parameter(Mandatory=$true)]
  [ValidateNotNullOrEmpty()]
  [string]$ProjectFolder
)
$ErrorActionPreference = 'Stop'
$spaceEngineersInstallationKey = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 244850'
$contentFolders = @('Data', 'Models', 'Particles', 'VoxelMaps')
try
{
  $GamePath = (Get-ItemProperty -Path $spaceEngineersInstallationKey).InstallLocation
  $contentFolders | %{ Copy-Item -Path "$GamePath\Content\$_" -Destination "$PsScriptRoot\..\bin\x64\Content\" -Recurse -Force -Verbose }
  Copy-Item -Path "$GamePath\DedicatedServer64\*" -Destination "$PSScriptRoot\..\GameFiles" -Recurse -Force -Verbose
} catch {
  $_
  exit 1
}