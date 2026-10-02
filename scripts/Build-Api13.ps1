param([Parameter(Mandatory = $true)][string]$DalamudLibPath)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$sdk = (Resolve-Path -LiteralPath $DalamudLibPath).Path
$version = [Reflection.AssemblyName]::GetAssemblyName((Join-Path $sdk 'Dalamud.dll')).Version
if ($version.Major -ne 13) { throw "API 13 SDK required; found $version" }
dotnet run --project "$root/tests/LocalizationChecks.csproj" -c Release
if ($LASTEXITCODE -ne 0) { throw 'Localization checks failed' }
dotnet build "$root/VFXEditor/VFXEditor.csproj" -c Release "-p:DalamudLibPath=$sdk/"
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
$package = Join-Path $root 'VFXEditor/bin/Release/VFXEditor/latest.zip'
if (!(Test-Path -LiteralPath $package)) { throw 'Packager did not produce latest.zip' }
Get-FileHash -LiteralPath $package -Algorithm SHA256
