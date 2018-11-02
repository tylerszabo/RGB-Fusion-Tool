// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using GvLedLibDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GvLedLibDotNetTests
{
    public class GvLedLibv1_0Mock : GvLedLibDotNet.Raw.IGvLedLibv1_0
    {
        public static GvLedLibDotNet.RGBFusionPeripherals RGBFusionPeripheralsFactory(GvLedLibv1_0Mock mock)
        {
            return new GvLedLibDotNet.RGBFusionPeripherals(new GvLedLibDotNet.Raw.GvLedLibv1_0Wrapper(mock));
        }

        public class Status
        {
            // Known status codes
            public const uint GV_LED_API_OK = 0x0;
            public const uint GV_LED_API_DEVICE_NOT_AVAILABLE = 0x2;
            public const uint GV_LED_API_ERROR_PARAM = 0x3;

            // May be a real status code, but not documented
            public const uint ERROR_Fake = 0x1234;
        }

        public static readonly int[] DefaultDevices = { (int)DeviceType.VGA };

        private bool initialized = false;

        public int[] Devices { get; set; } = DefaultDevices;
        public int? DeviceCountOverride { get; set; } = null;
        public uint NextReturn { get; set; } = Status.GV_LED_API_OK;
        public List<GVLED_CFG?> Settings { get; set; }  = new List<GVLED_CFG?>();

        public uint GvLedGetVersion(out int iMajorVersion, out int iMinorVersion)
        {
            throw new NotImplementedException();
        }

        public uint GvLedGetVgaModelName(out byte[] pVgaModelName)
        {
            throw new NotImplementedException();
        }

        public uint GvLedInitial(out int iDeviceCount, int[] iDeviceIdArray)
        {
            if (initialized) { Assert.Fail("Already initialized"); }
            initialized = true;

            Settings = new List<GVLED_CFG?>(Devices.Length);
            for (int i = 0; i < Devices.Length; i++)
            {
                iDeviceIdArray[i] = Devices[i];
                Settings.Add(null);
            }
            iDeviceCount = DeviceCountOverride ?? Devices.Length;
            return NextReturn;
        }

        public uint GvLedSave(int nIndex, GVLED_CFG config)
        {
            if (!initialized) { Assert.Fail("Never initialized"); }

            if (nIndex == -1)
            {
                for (int i = 0; i < Settings.Count; i++)
                {
                    Settings[i] = config;
                }
            }
            else
            {
                Settings[nIndex] = config;
            }

            return NextReturn;
        }

        public uint GvLedSet(int nIndex, GVLED_CFG config)
        {
            if (!initialized) { Assert.Fail("Never initialized"); }

            throw new NotImplementedException();
        }
    }
}
