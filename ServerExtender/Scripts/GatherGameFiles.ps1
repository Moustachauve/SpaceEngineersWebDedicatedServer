param (
  [Parameter(Mandatory=$true)]
  [ValidateNotNullOrEmpty()]
  [string]$ProjectFolder
)
$ErrorActionPreference = 'Stop'
$spaceEngineersInstallationKey = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 244850'
$contentFolders = @('Data', 'Models', 'Particles', 'VoxelMaps')
try {
  $GamePath = (Get-ItemProperty -Path $spaceEngineersInstallationKey -ErrorAction SilentlyContinue).InstallLocation
  if(-Not $GamePath) {
    $GamePath = "$((Get-ItemProperty -Path 'HKLM:\SOFTWARE\WOW6432Node\Valve\Steam').InstallPath)\steamapps\common\SpaceEngineers"
    if(-Not $(Test-Path -Path $GamePath)) {
      throw "The heck, where is your Space Engineers Installation!?"
    }
  }
  $contentFolders | %{ Copy-Item -Path "$GamePath\Content\$_" -Destination "$PsScriptRoot\..\bin\x64\Content\" -Recurse -Force -Verbose }
  Copy-Item -Path "$GamePath\DedicatedServer64\*" -Destination "$PSScriptRoot\..\GameFiles" -Recurse -Force -Verbose
} catch {
  $_
  exit 1
}