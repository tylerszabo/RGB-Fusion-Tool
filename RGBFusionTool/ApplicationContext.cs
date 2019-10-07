// Copyright (C) 2019 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GLedApiDotNet.LedSettings;
using GvLedLibDotNet.GvLedSettings;
using System.Collections.Generic;

namespace RGBFusionTool
{
    public class ApplicationContext
    {
        public int Verbosity { get; set; }
        public bool ShowHelp { get; set; }
        public bool ShowVersion { get; set; }
        public bool ListZones { get; set; }
        public bool ListPeripherals { get; set; }
        public void ListAll()
        {
            ListZones = true;
            ListPeripherals = true;
        }

        public LedSetting DefaultSetting { get; set; }
        public Dictionary<int, LedSetting> ZoneSettings { get; set; }
        public GvLedSetting PeripheralsSetting { get; set; }

        public ApplicationContext()
        {
            Verbosity = 0;
            ListZones = false;
            ListPeripherals = false;

            ShowHelp = false;
            ShowVersion = false;

            DefaultSetting = null;
            ZoneSettings = null;
            PeripheralsSetting = null;
        }
    }
}
