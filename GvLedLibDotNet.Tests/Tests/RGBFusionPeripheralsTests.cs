// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GvLedLibDotNet;
using GvLedLibDotNet.GvLedSettings;
using System.Drawing;

namespace GvLedLibDotNetTests.Tests
{
    [TestClass]
    public class RGBFusionPeripheralsTests
    {
        GvLedLibv1_0Mock mock;
        IRGBFusionPeripherals peripherals;

        [TestInitialize]
        public void Setup()
        {
            mock = new GvLedLibv1_0Mock();
            peripherals = new RGBFusionPeripherals(new GvLedLibDotNet.Raw.GvLedLibv1_0Wrapper(mock));
        }

        [TestMethod]
        public void GetDevices()
        {
            Assert.IsNotNull(peripherals.Devices);

            // Call again, just to be sure
            Assert.IsNotNull(peripherals.Devices);
        }

        [TestMethod]
        public void SetAll()
        {
            peripherals.SetAll(new ColorCycleGvLedSetting(1, 10));

            foreach (GVLED_CFG ledSetting in mock.Settings)
            {
                GvLedSettingTests.AssertGVLedStructEqual(SettingStructs.ColorCycleA, ledSetting);
            }
        }

        [TestMethod]
        public void SetAllTwoVGA()
        {
            mock.Devices = new int[] { (int)DeviceType.VGA, (int)DeviceType.VGA };
            peripherals.SetAll(new ColorCycleGvLedSetting(1, 10));

            foreach (GVLED_CFG ledSetting in mock.Settings)
            {
                GvLedSettingTests.AssertGVLedStructEqual(SettingStructs.ColorCycleA, ledSetting);
            }
        }

        [TestMethod]
        public void SetStaticTwoVGA()
        {
            mock.Devices = new int[] { (int)DeviceType.VGA, (int)DeviceType.VGA };

            peripherals.LedSettings[0] = new StaticGvLedSetting(Color.Red, 5);
            peripherals.LedSettings[1] = new StaticGvLedSetting(Color.Purple, 10);

            GvLedSettingTests.AssertGVLedStructEqual(SettingStructs.StaticRed, mock.Settings[0].Value);
            GvLedSettingTests.AssertGVLedStructEqual(SettingStructs.StaticPurple, mock.Settings[1].Value);
        }
    }
}
