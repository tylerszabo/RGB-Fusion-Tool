// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using GvLedLibDotNet;
using GvLedLibDotNet.GvLedSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GvLedLibDotNetTests.Tests
{
    public class SettingStructs
    {
        public static readonly GVLED_CFG Empty = new GVLED_CFG(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1);
        public static readonly GVLED_CFG StaticRed = new GVLED_CFG(1, 0, 0, 0, 0, 0, 5, 0x00FF0000, 0, 1, 1);
        public static readonly GVLED_CFG StaticPurple = new GVLED_CFG(1, 0, 0, 0, 0, 0, 10, 0x00800080, 0, 1, 1);
        public static readonly GVLED_CFG StaticDodgerBlue = new GVLED_CFG(1, 0, 0, 0, 0, 0, 10, 0x001E90FF, 0, 1, 1);
        public static readonly GVLED_CFG StaticDodgerBlueOff = new GVLED_CFG(1, 0, 0, 0, 0, 0, 0, 0x001E90FF, 0, 1, 1);
        public static readonly GVLED_CFG Off = new GVLED_CFG(1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1);
        public static readonly GVLED_CFG ColorCycleA = new GVLED_CFG(5, 1, 0, 0, 0, 0, 10, 0, 0, 1, 1);
        public static readonly GVLED_CFG ColorCycleB = new GVLED_CFG(5, 5, 0, 0, 0, 0, 8, 0, 0, 1, 1);
    }

    [TestClass]
    public class GvLedSettingTests
    {
        public static void AssertGVLedStructEqual(GVLED_CFG expected, GVLED_CFG actual)
        {
            Assert.AreEqual(expected.nType, actual.nType, "nType");
            Assert.AreEqual(expected.nSpeed, actual.nSpeed, "nSpeed");
            Assert.AreEqual(expected.dwTime1, actual.dwTime1, "dwTime1");
            Assert.AreEqual(expected.dwTime2, actual.dwTime2, "dwTime2");
            Assert.AreEqual(expected.dwTime3, actual.dwTime3, "dwTime3");
            Assert.AreEqual(expected.nMinBrightness, actual.nMinBrightness, "nMinBrightness");
            Assert.AreEqual(expected.nMaxBrightness, actual.nMaxBrightness, "nMaxBrightness");
            Assert.AreEqual(expected.dwColor, actual.dwColor, "dwColor");
            Assert.AreEqual(expected.nAngle, actual.nAngle, "nAngle");
            Assert.AreEqual(expected.nOn, actual.nOn, "nOn");
            Assert.AreEqual(expected.nSync, actual.nSync, "nSync");
        }

        [TestMethod]
        public void Empty()
        {
            AssertGVLedStructEqual(SettingStructs.Empty, (new GvLedSetting()).ToStruct());
        }

        [DataRow(11U)]
        [DataRow(12U)]
        [DataRow(255U)]
        [DataRow(65535U)]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StaticBadBrightness(uint brightness)
        {
            GvLedSetting setting = new StaticGvLedSetting(Color.Empty, brightness);
        }

        [TestMethod]
        public void StaticRed()
        {
            AssertGVLedStructEqual(SettingStructs.StaticRed, new StaticGvLedSetting(Color.Red, 5).ToStruct());
        }

        [TestMethod]
        public void StaticPurple()
        {
            AssertGVLedStructEqual(SettingStructs.StaticPurple, new StaticGvLedSetting(Color.Purple, 10).ToStruct());
        }

        [TestMethod]
        public void StaticDodgerBlue()
        {
            AssertGVLedStructEqual(SettingStructs.StaticDodgerBlue, new StaticGvLedSetting(Color.DodgerBlue, 10).ToStruct());
        }

        [TestMethod]
        public void StaticDodgerBlueOff()
        {
            AssertGVLedStructEqual(SettingStructs.StaticDodgerBlueOff, new StaticGvLedSetting(Color.DodgerBlue, 0).ToStruct());
        }

        [TestMethod]
        public void Off()
        {
            AssertGVLedStructEqual(SettingStructs.Off, new OffGvLedSetting().ToStruct());
        }

        [TestMethod]
        public void ColorCycleA()
        {
            AssertGVLedStructEqual(SettingStructs.ColorCycleA, new ColorCycleGvLedSetting(1, 10).ToStruct());
        }

        [TestMethod]
        public void ColorCycleB()
        {
            AssertGVLedStructEqual(SettingStructs.ColorCycleB, new ColorCycleGvLedSetting(5, 8).ToStruct());
        }
    }
}
