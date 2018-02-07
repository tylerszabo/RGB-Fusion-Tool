# RGB Fusion Tool

## Dependencies

Get `GLedApi.dll`, `ycc.dll`, and `layout.ini` from [Gigabyte's RGB Fusion SDK](https://www.gigabyte.com/mb/rgb/sdk)

Direct link:
- [B17.0926.1.zip](https://www.gigabyte.com/WebPage/332/images/B17.0926.1.zip)
  - SHA-256 hash: `02a3ec94bbec022013bd1086a1eedf7ea4177edd3127b4179ccb2aeccad3a256`

## Building

Build with [Visual Studio 2017](https://www.visualstudio.com/downloads/), then copy the build output to a single directory.

    GLedApiDotNet.dl
    Mono.Options.dll
    RGBFusionTool.exe
    GLedApi.dll
    ycc.dll
    layout.ini

## Running

`RGBFusionTool` is a command line tool. For usage instructions run:

    RGBFusionTool.exe --help

## Legal

### Copyright

Copyright © 2018 Tyler Szabo

### License

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

See `LICENSE` file for a full copy of GPLv3 text