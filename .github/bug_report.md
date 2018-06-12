# Bug Report

Thank you for taking the time to experiment with this tool and report an issue.

## Summary

Give a brief summary of the issue.

## Repro

### Steps and Commands

- Include the command line input that results in incorrect behavior or sequence of events

### Actual behavior

- Describe the actual behavior. Be as specific as possible
- Paste error messages / exception details here. Use [triple backtick](https://guides.github.com/features/mastering-markdown/) markdown "`` ``` ``" format.

```
Paste output here
```

### Expected behavior

- Describe the expected behavior

## Environment

Please include the following information:

- Windows Version
- RGB Fusion Tool Version
- Motherboard model
- BIOS version
- SHA-256 hashes of required files in the directory containing `RGBFusionTool.exe`

### Gathering Script

Consider running the following PowerShell script to gather environment information. If any is missing please add mention that the script isn't correctly gathering the information so it can be updated. Run this from the directory containing `RGBFusionTool.exe`.

```````powershell
$winver = [System.Environment]::OSVersion.Version.ToString()
$toolver = (Get-Item .\RGBFusionTool.exe).VersionInfo.FileVersion.ToString()
$hashes = @("GLedApi.dll", "GLedApiDotNet.dll", "layout.ini", "Mono.Options.dll", "RGBFusionTool.exe", "ycc.dll") | % { Get-FileHash -Algorithm SHA256 -Path $_ }
$biosversion = (Get-WmiObject -Class Win32_BIOS).SMBIOSBIOSVersion
$motherboard = (Get-WmiObject -Class Win32_BaseBoard).Product

Write-Host -ForegroundColor White -BackgroundColor Black @"
``````
- Windows Version: $winver
- RGB Fusion Tool Version: $toolver
- Motherboard: $motherboard
- BIOS: $biosversion

Hashes:

$(($hashes | % { "$($_.Hash) $(Split-Path -Leaf $_.Path)"}) -join "`n")
``````
"@
Out-Null | Write-Host
```````

### Sample output

```
- Windows Version: 10.0.17134.0
- RGB Fusion Tool Version: 0.9.1
- Motherboard: Z370 AORUS Gaming 7
- BIOS: F5h

Hashes:

1D0D8D01382CD2617AEA26162C2A3FA2FF845B93A815882CC74B512568DF6BC4 GLedApi.dll
3A4AF1C0C90085286E24794A1DBA4CD373BC508B3B4951E3668E4B967457E3F8 GLedApiDotNet.dll
C99AB9E904CE507F43D27528D51C09BB824E2071EC52571E50F7CED938857002 layout.ini
05FC657EB5B5D563DE7807DF6EE07FC8B89DC6F6F08EB76C30E5192429FBBDCF Mono.Options.dll
06011A2510004D752794F1E89A20E9D12DB81C86B53259A5C8E24BCB8DB080C2 RGBFusionTool.exe
4D38B1D9F614D0E49C22E7273FCB56F109A6ED34632247D1B60C14D72D495569 ycc.dll
```

## Detailed Description and Additional Context

Add any other information here