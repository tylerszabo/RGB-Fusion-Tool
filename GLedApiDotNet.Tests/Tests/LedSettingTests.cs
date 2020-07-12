// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using GLedApiDotNet.LedSettings;
using System;

namespace GLedApiDotNetTests.Tests
{
    [TestClass]
    public class LedSettingTests
    {
        public class SettingByteArrays
        {
            public static readonly byte[] Empty = {
                0x00,   // Reserved0
                0,      // LedMode
                0,      // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0,      // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] Off = {
                0x00,   // Reserved0
                4,      // LedMode
                0,      // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0,      // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] StaticRed50 = {
                0x00,   // Reserved0
                4,      // LedMode
                50,     // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0,      // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] StaticPurple = {
                0x00,   // Reserved0
                4,      // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x80,   // dwColor BB
                0x00,   // dwColor GG
                0x80,   // dwColor RR
                0,      // dwColor WW
                0,      // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] StaticDodgerBlue = {
                0x00,   // Reserved0
                4,      // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0xFF,   // dwColor BB
                0x90,   // dwColor GG
                0x1E,   // dwColor RR
                0,      // dwColor WW
                0,      // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] StaticDodgerBlueOff = {
                0x00,   // Reserved0
                4,      // LedMode
                0,      // MaxBrightness
                0,      // MinBrightness
                0xFF,   // dwColor BB
                0x90,   // dwColor GG
                0x1E,   // dwColor RR
                0,      // dwColor WW
                0,      // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] PulseA = {
                0x00,   // Reserved0
                1,      // LedMode
                75,     // MaxBrightness
                25,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0xB8,   // wTime0
                0x0B,   // wTime0
                0xE7,   // wTime1
                0x03,   // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] PulseB = {
                0x00,   // Reserved0
                1,      // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0xC8,   // wTime0
                0x00,   // wTime0
                0xFF,   // wTime1
                0xFF,   // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] PulseC = {
                0x00,   // Reserved0
                1,      // LedMode
                100,    // MaxBrightness
                50,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0xFF,   // wTime0
                0xFF,   // wTime0
                0xFF,   // wTime1
                0xFF,   // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] ColorCycleA = {
                0x00,   // Reserved0
                3,      // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x70,   // wTime0
                0x17,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                7,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] ColorCycleA_1s = {
                0x00,   // Reserved0
                3,      // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x64,   // wTime0
                0x00,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                7,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] ColorCycleA_4s = {
                0x00,   // Reserved0
                3,      // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x90,   // wTime0
                0x01,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                7,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] ColorCycleA_500ms = {
                0x00,   // Reserved0
                3,      // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x32,   // wTime0
                0x00,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                7,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] ColorCycleB = {
                0x00,   // Reserved0
                3,      // LedMode
                100,    // MaxBrightness
                20,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x96,   // wTime0
                0x00,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                2,      // CtrlVal0
                1 };    // CtrlVal1

            public static readonly byte[] FlashA = {
                0x00,   // Reserved0
                5,      // LedMode
                100,    // MaxBrightness
                20,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0x2C,   // wTime0
                0x01,   // wTime0
                0xE8,   // wTime1
                0x03,   // wTime1
                0xA0,   // wTime2
                0x0F,   // wTime2
                2,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] FlashB = {
                0x00,   // Reserved0
                5,      // LedMode
                100,    // MaxBrightness
                10,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0xFA,   // wTime0
                0x00,   // wTime0
                0xE8,   // wTime1
                0x03,   // wTime1
                0xE8,   // wTime2
                0x03,   // wTime2
                1,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] FlashC = {
                0x00,   // Reserved0
                5,      // LedMode
                100,    // MaxBrightness
                10,     // MinBrightness
                0xFF,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x1E,   // wTime0
                0x00,   // wTime0
                0x64,   // wTime1
                0x00,   // wTime1
                0x90,   // wTime2
                0x01,   // wTime2
                3,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] TransitionA = {
                0x00,   // Reserved0
                8,      // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0x70,   // wTime0
                0x17,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] TransitionB = {
                0x00,   // Reserved0
                8,      // LedMode
                100,    // MaxBrightness
                20,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0xDC,   // wTime0
                0x05,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalA1 = {
                0x00,   // Reserved0
                10,     // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x64,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalA2 = {
                0x00,   // Reserved0
                10,     // LedMode
                80,     // MaxBrightness
                20,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0xF4,   // wTime0
                0x01,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                1,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalB1 = {
                0x00,   // Reserved0
                11,     // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0xC8,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalB2 = {
                0x00,   // Reserved0
                11,     // LedMode
                90,     // MaxBrightness
                10,     // MinBrightness
                0xFF,   // dwColor BB
                0x90,   // dwColor GG
                0x1E,   // dwColor RR
                0x00,   // dwColor WW
                0x5E,   // wTime0
                0x01,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalC1 = {
                0x00,   // Reserved0
                12,     // LedMode
                100,    // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0x64,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalC2 = {
                0x00,   // Reserved0
                12,     // LedMode
                50,     // MaxBrightness
                5,      // MinBrightness
                0xFF,   // dwColor BB
                0x90,   // dwColor GG
                0x1E,   // dwColor RR
                0x00,   // dwColor WW
                0xC8,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                50,     // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalC3 = {
                0x00,   // Reserved0
                12,     // LedMode
                80,     // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0xFF,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x5E,   // wTime0
                0x01,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                100,    // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalD1 = {
                0x00,   // Reserved0
                13,     // LedMode
                80,     // MaxBrightness
                0,      // MinBrightness
                0xFF,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x90,   // wTime0
                0x01,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalD2 = {
                0x00,   // Reserved0
                13,     // LedMode
                50,     // MaxBrightness
                10,     // MinBrightness
                0x3C,   // dwColor BB
                0x14,   // dwColor GG
                0xDC,   // dwColor RR
                0x00,   // dwColor WW
                0x64,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalE1 = {
                0x00,   // Reserved0
                14,     // LedMode
                60,     // MaxBrightness
                10,     // MinBrightness
                0x00,   // dwColor BB
                0x80,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x64,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalE2 = {
                0x00,   // Reserved0
                14,     // LedMode
                70,     // MaxBrightness
                20,     // MinBrightness
                0xFF,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x2C,   // wTime0
                0x01,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalF1 = {
                0x00,   // Reserved0
                15,     // LedMode
                90,     // MaxBrightness
                30,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0xC8,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalF2 = {
                0x00,   // Reserved0
                15,     // LedMode
                100,    // MaxBrightness
                10,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x58,   // wTime0
                0x02,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalG1 = {
                0x00,   // Reserved0
                16,     // LedMode
                90,     // MaxBrightness
                20,     // MinBrightness
                0x00,   // dwColor BB
                0x00,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0xC8,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalG2 = {
                0x00,   // Reserved0
                16,     // LedMode
                100,    // MaxBrightness
                30,     // MinBrightness
                0x00,   // dwColor BB
                0xFF,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0x58,   // wTime0
                0x02,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                50,     // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalG3 = {
                0x00,   // Reserved0
                16,     // LedMode
                70,     // MaxBrightness
                0,      // MinBrightness
                0xFF,   // dwColor BB
                0x00,   // dwColor GG
                0x00,   // dwColor RR
                0x00,   // dwColor WW
                0xE8,   // wTime0
                0x03,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                100,    // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalH1 = {
                0x00,   // Reserved0
                17,     // LedMode
                100,    // MaxBrightness
                30,     // MinBrightness
                0x80,   // dwColor BB
                0x00,   // dwColor GG
                0x80,   // dwColor RR
                0x00,   // dwColor WW
                0xC8,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalH2 = {
                0x00,   // Reserved0
                17,     // LedMode
                50,     // MaxBrightness
                5,      // MinBrightness
                0x00,   // dwColor BB
                0xFF,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0x58,   // wTime0
                0x02,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalI1 = {
                0x00,   // Reserved0
                18,     // LedMode
                60,     // MaxBrightness
                10,     // MinBrightness
                0xFF,   // dwColor BB
                0x90,   // dwColor GG
                0x1E,   // dwColor RR
                0x00,   // dwColor WW
                0x64,   // wTime0
                0,      // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1

            public static readonly byte[] DigitalI2 = {
                0x00,   // Reserved0
                18,     // LedMode
                80,     // MaxBrightness
                0,      // MinBrightness
                0x00,   // dwColor BB
                0xA5,   // dwColor GG
                0xFF,   // dwColor RR
                0x00,   // dwColor WW
                0xBC,   // wTime0
                0x02,   // wTime0
                0,      // wTime1
                0,      // wTime1
                0,      // wTime2
                0,      // wTime2
                0,      // CtrlVal0
                0 };    // CtrlVal1
        }

        [TestMethod]
        public void Empty()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.Empty,
                new LedSetting().ToByteArray()
            );
        }

        [DataRow(101)]
        [DataRow(200)]
        [DataRow(255)]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StaticException(int brightness)
        {
            LedSetting led = new StaticLedSetting(Color.Empty, (byte)brightness);
        }

        [TestMethod]
        public void StaticRed()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.StaticRed50,
                new StaticLedSetting(Color.Red, 50).ToByteArray()
            );
        }

        [TestMethod]
        public void StaticPurple()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.StaticPurple,
                new StaticLedSetting(Color.Purple, 100).ToByteArray()
            );
        }

        [TestMethod]
        public void StaticDodgerBlue()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.StaticDodgerBlue,
                new StaticLedSetting(Color.DodgerBlue, 100).ToByteArray()
            );
        }

        [TestMethod]
        public void StaticDodgerBlueOff()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.StaticDodgerBlueOff,
                new StaticLedSetting(Color.DodgerBlue, 0).ToByteArray()
            );
        }

        [TestMethod]
        public void Off()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.Off,
                new OffLedSetting().ToByteArray()
            );
        }

        [TestMethod]
        public void PulseA()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.PulseA,
                new PulseLedSetting(Color.Red, 75, 25, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(9.99)).ToByteArray()
            );
        }

        [TestMethod]
        public void PulseA2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.PulseA,
                new PulseLedSetting(Color.Red, 75, 25, 3000, 999).ToByteArray()
            );
        }

        [TestMethod]
        public void PulseA3()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.PulseA,
                new PulseLedSetting(Color.Red, 75, 25, TimeSpan.FromSeconds(30.00), TimeSpan.FromSeconds(9.99)).ToByteArray()
            );
        }

        [TestMethod]
        public void PulseB()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.PulseB,
                new PulseLedSetting(Color.Red, 100, 0, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(655350)).ToByteArray()
            );
        }

        [TestMethod]
        public void PulseC()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.PulseC,
                new PulseLedSetting(Color.Red, 100, 50, 65535, 65535).ToByteArray()
            );
        }

        [DataRow(101, 0)]
        [DataRow(100, 101)]
        [DataRow(200, 101)]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PulseExceptionBrightness(int max, int min)
        {
            LedSetting led = new PulseLedSetting(Color.Red, (byte)max, (byte)min, 100, 200);
        }

        [DataRow(-300.0, 700.0)]
        [DataRow(300.0, -700.0)]
        [DataRow(655359.0, 700.0)]
        [DataRow(655360.0, 700.0)]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PulseExceptionTimeSpan(double on, double off)
        {
            LedSetting led = new PulseLedSetting(Color.Red, 100, 0, TimeSpan.FromMilliseconds(on), TimeSpan.FromMilliseconds(off));
        }

        [TestMethod]
        public void ColorCycleA()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.ColorCycleA,
                new ColorCycleLedSetting(100, 0, TimeSpan.FromMinutes(1), 7, false).ToByteArray()
            );
        }

        [TestMethod]
        public void ColorCycleB()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.ColorCycleB,
                new ColorCycleLedSetting(100, 20, 150, 2, true).ToByteArray()
            );
        }

        [DataRow(0)]
        [DataRow(8)]
        [DataRow(10)]
        [DataRow(14)]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ColorCycleInvalidColorCount(int colors)
        {
            LedSetting led = new ColorCycleLedSetting(100, 0, 200, (byte)colors, false);
        }

        [DataRow(655355)]
        [DataRow(900000)]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ColorCycleInvalidTimeSpan(int time)
        {
            LedSetting led = new ColorCycleLedSetting(100, 0, TimeSpan.FromMilliseconds(time), 7, false);
        }

        [DataRow(90000)]
        [DataRow(655350)]
        [DataRow(100000)]
        [DataTestMethod]
        public void ColorCycleValidTimeSpan(int time)
        {
            Assert.AreEqual(16,
                new ColorCycleLedSetting(100, 0, TimeSpan.FromMilliseconds(time), 7, false).ToByteArray().Length
            );
        }

        [TestMethod]
        public void FlashA()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.FlashA,
                new FlashLedSetting(Color.Red, 100, 20, 300, 1000, 4000, 2).ToByteArray()
            );
        }

        [TestMethod]
        public void FlashA2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.FlashA,
                new FlashLedSetting(Color.Red, 100, 20, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(40), 2).ToByteArray()
            );
        }

        [TestMethod]
        public void FlashC()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.FlashC,
                new FlashLedSetting(Color.Blue, 100, 10, TimeSpan.FromSeconds(.3), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4), 3).ToByteArray()
            );
        }

        [TestMethod]
        public void FlashB()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.FlashB,
                new FlashLedSetting(Color.Red, 100, 10, 250, 1000, 1000, 1).ToByteArray()
            );
        }

        [DataRow(1000, 1000, 2000, 1, DisplayName = "!(onoff < interval)")]
        [DataRow(250, 1000, 1000, 2, DisplayName = "!(count * interval <= cycle)")]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FlashException(int onoff, int interval, int cycle, int count)
        {
            LedSetting led = new FlashLedSetting(Color.Red, 100, 0, (ushort)onoff, (ushort)interval, (ushort)cycle, (byte)count);
        }

        [DataRow(1, 1, 2, 1, DisplayName = "!(onoff < interval)")]
        [DataRow(.25, 1, 1, 2, DisplayName = "!(count * interval <= cycle)")]
        [DataRow(.25, 1, 660, 1, DisplayName = "cycle too long")]
        [DataRow(0, 0, -1, 1, DisplayName = "cycle negative time")]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FlashExceptionTimeSpan(double onoff, double interval, double cycle, int count)
        {
            LedSetting led = new FlashLedSetting(Color.Red, 100, 0, TimeSpan.FromSeconds(onoff), TimeSpan.FromSeconds(interval), TimeSpan.FromSeconds(cycle), (byte)count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FlashExceptionCount()
        {
			LedSetting led = new FlashLedSetting(Color.Red, 100, 10, 250, 1000, 1000, 0);
        }

        [DataRow(.25, 1, 2, 1)]
        [DataRow(.25, 1, 2, 2)]
        [DataTestMethod]
        public void FlashValid(double onoff, double interval, double cycle, int count)
        {
            Assert.AreEqual(16,
                new FlashLedSetting(Color.Red, 100, 0, TimeSpan.FromSeconds(onoff), TimeSpan.FromSeconds(interval), TimeSpan.FromSeconds(cycle), (byte)count).ToByteArray().Length);
        }

        [TestMethod]
        public void TransitionA()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.TransitionA,
                new TransitionLedSetting(Color.Red, 100, 0, TimeSpan.FromMinutes(1)).ToByteArray()
            );
        }

        [TestMethod]
        public void TransitionB()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.TransitionB,
                new TransitionLedSetting(Color.Red, 100, 20, TimeSpan.FromSeconds(15)).ToByteArray()
            );
        }

        [DataRow(655356)]
        [DataRow(655355)]
        [DataRow(655351)]
        [DataRow(900000)]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TransitionException(double time)
        {
            LedSetting led = new TransitionLedSetting(Color.Red, 100, 0, TimeSpan.FromMilliseconds(time));
        }

        [TestMethod]
        public void DigitalA1()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalA1,
                new DigitalA(100, 0, TimeSpan.FromSeconds(1), DigitalA.Direction.RightToLeft).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalA1,
                new DigitalA(100, 0, 100, DigitalA.Direction.RightToLeft).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalA2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalA2,
                new DigitalA(80, 20, TimeSpan.FromSeconds(5), DigitalA.Direction.LeftToRight).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalA2,
                new DigitalA(80, 20, 500, DigitalA.Direction.LeftToRight).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalB1()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalB1,
                new DigitalB(Color.Red, 100, 0, TimeSpan.FromSeconds(2)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalB1,
                new DigitalB(Color.Red, 100, 0, 200).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalB2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalB2,
                new DigitalB(Color.DodgerBlue, 90, 10, TimeSpan.FromSeconds(3.5)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalB2,
                new DigitalB(Color.DodgerBlue, 90, 10, 350).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalC1()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalC1,
                new DigitalC(Color.Red, 100, 0, TimeSpan.FromSeconds(1), 0).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalC1,
                new DigitalC(Color.Red, 100, 0, 100, 0).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalC2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalC2,
                new DigitalC(Color.DodgerBlue, 50, 5, TimeSpan.FromSeconds(2), 50).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalC2,
                new DigitalC(Color.DodgerBlue, 50, 5, 200, 50).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalC3()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalC3,
                new DigitalC(Color.Lime, 80, 0, TimeSpan.FromSeconds(3.5), 100).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalC3,
                new DigitalC(Color.Lime, 80, 0, 350, 100).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalD1()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalD1,
                new DigitalD(Color.Blue, 80, 0, TimeSpan.FromSeconds(4)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalD1,
                new DigitalD(Color.Blue, 80, 0, 400).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalD2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalD2,
                new DigitalD(Color.Crimson, 50, 10, TimeSpan.FromSeconds(1)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalD2,
                new DigitalD(Color.Crimson, 50, 10, 100).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalE1()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalE1,
                new DigitalE(Color.Green, 60, 10, TimeSpan.FromSeconds(1)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalE1,
                new DigitalE(Color.Green, 60, 10, 100).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalE2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalE2,
                new DigitalE(Color.Blue, 70, 20, TimeSpan.FromSeconds(3)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalE2,
                new DigitalE(Color.Blue, 70, 20, 300).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalF1()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalF1,
                new DigitalF(90, 30, TimeSpan.FromSeconds(2)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalF1,
                new DigitalF(90, 30, 200).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalF2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalF2,
                new DigitalF(100, 10, TimeSpan.FromSeconds(6)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalF2,
                new DigitalF(100, 10, 600).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalG1()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalG1,
                new DigitalG(Color.Red, 90, 20, TimeSpan.FromSeconds(2), 0).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalG1,
                new DigitalG(Color.Red, 90, 20, 200, 0).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalG2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalG2,
                new DigitalG(Color.Lime, 100, 30, TimeSpan.FromSeconds(6), 50).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalG2,
                new DigitalG(Color.Lime, 100, 30, 600, 50).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalG3()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalG3,
                new DigitalG(Color.Blue, 70, 0, TimeSpan.FromSeconds(10), 100).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalG3,
                new DigitalG(Color.Blue, 70, 0, 1000, 100).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalH1()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalH1,
                new DigitalH(Color.Purple, 100, 30, TimeSpan.FromSeconds(2)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalH1,
                new DigitalH(Color.Purple, 100, 30, 200).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalH2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalH2,
                new DigitalH(Color.Yellow, 50, 5, TimeSpan.FromSeconds(6)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalH2,
                new DigitalH(Color.Yellow, 50, 5, 600).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalI1()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalI1,
                new DigitalI(Color.DodgerBlue, 60, 10, TimeSpan.FromSeconds(1)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalI1,
                new DigitalI(Color.DodgerBlue, 60, 10, 100).ToByteArray()
            );
        }

        [TestMethod]
        public void DigitalI2()
        {
            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalI2,
                new DigitalI(Color.Orange, 80, 0, TimeSpan.FromSeconds(7)).ToByteArray()
            );

            TestHelper.AssertLedSettingsEqual(
                SettingByteArrays.DigitalI2,
                new DigitalI(Color.Orange, 80, 0, 700).ToByteArray()
            );
        }
    }
}
