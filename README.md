# RGB Fusion Tool

## Installation

You can build RGB Fusion Tool locally (see below), install it as a [Chocolatey package](https://chocolatey.org/packages/rgbfusiontool
) (`choco install rgbfusiontool`), or extract one of the builds listed on [GitHub Releases](https://github.com/tylerszabo/RGB-Fusion-Tool/releases).

## Dependencies

Get `GLedApi.dll`, `ycc.dll`, and `layout.ini` from [Gigabyte's RGB Fusion SDK](https://www.gigabyte.com/mb/rgb/sdk)

Direct link:
- [B17.0926.1.zip](https://www.gigabyte.com/WebPage/332/images/B17.0926.1.zip)
  - SHA-256 hash: `02a3ec94bbec022013bd1086a1eedf7ea4177edd3127b4179ccb2aeccad3a256`
- [B18.0206.1.zip](https://www.gigabyte.com/WebPage/332/images/B18.0206.1.zip)
  - SHA-256 hash: `fd312d17482a866fc9b7902549dc187f120d9d883a9504cccf9e9eef93243d8d`

## Building

Build with [Visual Studio 2017](https://www.visualstudio.com/downloads/), then copy the build output to a single directory.

```
GLedApiDotNet.dll
Mono.Options.dll
RGBFusionTool.exe
GLedApi.dll
ycc.dll
layout.ini
README.md
LICENSE
```

## Running

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
