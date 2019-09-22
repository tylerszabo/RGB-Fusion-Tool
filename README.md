# RGB Fusion Tool

## Installation

You can build RGB Fusion Tool locally (see below), install it as a [Chocolatey package](https://chocolatey.org/packages/rgbfusiontool
) (`choco install rgbfusiontool`), or extract one of the builds listed on [GitHub Releases](https://github.com/tylerszabo/RGB-Fusion-Tool/releases).

## Dependencies

Get `GLedApi.dll`, `YccV2.dll`, and `layout.ini` from [Gigabyte's RGB Fusion SDK](https://www.gigabyte.com/mb/rgb/sdk).

Get `GvBiosLib.dll`, `GvDisplay.dll`, and `GvLedLib.dll` from a Gigabyte RGB Fusion Utility (the version of `GvLedLib.dll` included in SDKs `18.1004.1` to `19.0311.1` have a missing dependency on `GvIllumLib.dll`)

- [B17.0926.1.zip](https://www.gigabyte.com/WebPage/332/images/B17.0926.1.zip)
  - SHA-256 hash: `02a3ec94bbec022013bd1086a1eedf7ea4177edd3127b4179ccb2aeccad3a256`
- [B18.0206.1.zip](https://www.gigabyte.com/WebPage/332/images/B18.0206.1.zip)
  - SHA-256 hash: `fd312d17482a866fc9b7902549dc187f120d9d883a9504cccf9e9eef93243d8d`
- [B18.1004.1.zip](https://www.gigabyte.com/WebPage/332/images/B18.1004.1.zip)
  - SHA-256 hash: `74aaedc8b5e901f5e5f0296e5c01dac4cb429845d437b1cde42480c0c480f6aa`
- [B19.0311.1.zip](https://www.gigabyte.com/WebPage/332/images/B19.0311.1.zip)
  - SHA-256 hash: `55862af1ace8e7990757510fce106f4d6806c85ebb0354f6fc73f223be92cc8c`
- [mb_utility_rgb-fusion_B18.0629.1.zip](https://download.gigabyte.us/FileList/Utility/mb_utility_rgb-fusion_B18.0629.1.zip)
  - SHA-256 hash: `89e481b5648d989fd73062ef36f3f40b72666da4bd4c9182f4d3e9b7b2b01f83`

Unfortunately, `GvLedLib.dll` was built against a debug version of the VS2012 SDK. It may be possible to rename `mfc110u.dll` and `msvcr110.dll` from the official redistributable package to their respective debug (`d`) versions. Otherwise the solution is to fetch them from [Visual Studio 2012 Update 5](https://visualstudio.microsoft.com/vs/older-downloads/):

- mu_visual_studio_2012_update_5_x86_dvd_6967467.iso
  - SHA-256 hash: `405bad3d4249dd94b4fa309bb482ade9ce63d968b59cac9e2d63b0a24577285e`

Extract the libraries from `mu_visual_studio_2012_update_5_x86_dvd_6967467.iso\packages\vcRuntimeDebug_x86\cab1.cab`:

- `F_CENTRAL_mfc110ud_x86`, rename to `mfc110ud.dll`
  - SHA-256 hash: `af4cc763c48bd0cc8b8b89d2f81b98d38ce52bfa01ef95a4e7430a19e0fbae3c`
- `F_CENTRAL_msvcr110d_x86`, rename to `msvcr110d.dll`
  - SHA-256 hash: `bdbb4071a50e47ccf69f3ed35f46bdc97e27636d2c165fdf87426452c30ec58f`

## Building

Build with [Visual Studio 2019](https://www.visualstudio.com/downloads/), then copy the build output to a single directory.

```
GLedApiDotNet.dll
GvLedLibDotNet.dll
Mono.Options.dll
RGBFusionTool.exe

GLedApi.dll
YccV2.dll
layout.ini

GvBiosLib.dll
GvDisplay.dll
GvLedLib.dll

mfc110ud.dll
msvcr110d.dll

README.md
LICENSE
```

## Running

As of the `B19.0311.1` SDK release this tool will need to be run as an Administrator.

`RGBFusionTool` is a command line tool. For usage instructions run:

```
RGBFusionTool.exe --help
```

### Examples

Set all zones to red (50% brightness)

```
RGBFusionTool.exe --static=Red --brightness 50
```

Color cycle with 2 second transitions in zone 0

```
RGBFusionTool.exe --zone=0 --colorcycle=2
```

List zones

```
RGBFusionTool.exe --list
```

Set zones 0 through 3 to color cycle at different speeds (with verbose output)

```
RGBFusionTool.exe --verbose --zone=0 --cycle=32 --zone=1 --cycle=16 --zone=2 --cycle=8 --zone=3 --cycle=4
```

## Troubleshooting

- Verify you're using a supported [motherboard](https://www.gigabyte.com/mb/rgb/)
- Attempt running the tool in an elevated command prompt
- Ensure all Gigabyte SDK DLLs are present (see *Dependencies*)
- Power down the motherboard completely (such that the power supply is shut off or unplugged) and power isn't coming in through USB. Wait ~20 seconds for the board to discharge. (There's an issue where the controller can get stuck and only a complete power down seems to reset it.)

## Legal

### Copyright

Copyright © 2018 Tyler Szabo

### License

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

See `LICENSE` file for a full copy of GPLv3 text
