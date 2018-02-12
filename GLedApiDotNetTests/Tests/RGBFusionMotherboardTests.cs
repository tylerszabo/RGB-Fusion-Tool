// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GLedApiDotNet;
using System.Drawing;
using GLedApiDotNet.LedSettings;

namespace GLedApiDotNetTests.Tests
{
    [TestClass]
    public class RGBFusionMotherboardTests
    {
        GLedApiv1_0_0Mock mock;
        IRGBFusionMotherboard motherboard;

        [TestInitialize]
        public void Setup()
        {
            mock = new GLedApiv1_0_0Mock(true);
            motherboard = new RGBFusionMotherboard(new GLedApiDotNet.Raw.GLedAPIv1_0_0Wrapper(mock));
        }

        [TestMethod]
        [ExpectedException(typeof(GLedAPIException))]
        public void TestBadDivisions()
        {
            mock = new GLedApiv1_0_0Mock(true);
            mock.MaxDivisions = 0;

			new RGBFusionMotherboard(new GLedApiDotNet.Raw.GLedAPIv1_0_0Wrapper(mock));
        }

        [TestMethod]
        [ExpectedException(typeof(GLedAPIException))]
        public void TestBadVersion()
        {
            mock = new GLedApiv1_0_0Mock(true);
			mock.Version = "0.0.9";

			new RGBFusionMotherboard(new GLedApiDotNet.Raw.GLedAPIv1_0_0Wrapper(mock));
        }

        [TestMethod]
        public void GetLayout()
        {
            Assert.IsNotNull(motherboard.Layout);

            // Call again, just to be sure 
            Assert.IsNotNull(motherboard.Layout);
        }

        [TestMethod]
        public void SetWithoutSettings()
        {
            motherboard.Set();

            TestHelper.AssertAllLeds(mock,
                LedSettingTests.SettingByteArrays.Off,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [TestMethod]
        public void SetAll()
        {
            motherboard.SetAll(new StaticLedSetting(Color.Red, 50));

            TestHelper.AssertAllLeds(mock,
                LedSettingTests.SettingByteArrays.StaticRed50,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);

            // Verify again
            motherboard.Set();

            TestHelper.AssertAllLeds(mock,
                LedSettingTests.SettingByteArrays.StaticRed50,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(0, 1)]
        [DataRow(1, 2)]
        [DataRow(2, 4)]
        [DataRow(3, 8)]
        [DataRow(4, 16)]
        [DataRow(5, 32)]
        [DataRow(6, 64)]
        [DataRow(7, 128)]
        [DataRow(8, 256)]
        [DataTestMethod]
        public void SetSingle(int division, int expected)
        {
            motherboard.Set(division);
            Assert.AreEqual(expected, mock.LastApply);
        }

        [DataRow(0, 2, 4, 21)] // Doc example
        [DataRow(0, 1, 2, 7)]
        [DataRow(0, 0, 4, 17)]
        [DataTestMethod]
        public void SetDivisions3(int a, int b, int c, int expected)
        {
            motherboard.Set(a, b, c);
            Assert.AreEqual(expected, mock.LastApply);
        }

        [DataRow(1, 2, 3, 7, 142)]
        [DataRow(1, 1, 1, 1, 2)]
        [DataTestMethod]
        public void SetDivisions4(int a, int b, int c, int d, int expected)
        {
            motherboard.Set(a, b, c, d);
            Assert.AreEqual(expected, mock.LastApply);
        }

        [DataRow(-1)]
        [DataRow(-2)]
        [DataRow(GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS)]
        [DataRow(GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS + 1)]
        [DataRow(GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS + 10)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [DataTestMethod]
        public void SetOutOfRangeSingle(int division)
        {
            motherboard.Set(division);
        }

        [DataRow(0, 2, 4, GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS)]
        [DataRow(0, 2, 4, -1)]
        [DataRow(0, 2, GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS, 4)]
        [DataRow(0, 2, -1, 4)]
        [DataRow(GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS, 0, 2, 4)]
        [DataRow(-1, 0, 2, 4)]
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetOutOfRangeDivisions(int a, int b, int c, int d)
        {
            motherboard.Set(a, b, c, d);
        }

        [TestMethod]
        public void SetMix()
        {
            motherboard.LedSettings[0] = new StaticLedSetting(Color.Red, 50);
            motherboard.LedSettings[1] = new StaticLedSetting(Color.Purple, 100);
            motherboard.LedSettings[2] = new StaticLedSetting(Color.DodgerBlue, 100);

            for (int repeat = 0; repeat < 3; repeat++)
            {
                motherboard.Set();

                Assert.AreEqual(GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS * 16, mock.ConfiguredLeds.Length);

                byte[] part = new byte[16];

                Array.Copy(mock.ConfiguredLeds, 0, part, 0, 16);
                TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.StaticRed50, part);

                Array.Copy(mock.ConfiguredLeds, 1 * 16, part, 0, 16);
                TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.StaticPurple, part);

                Array.Copy(mock.ConfiguredLeds, 2 * 16, part, 0, 16);
                TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.StaticDodgerBlue, part);

                for (int i = 3; i < GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS; i++)
                {
                    Array.Copy(mock.ConfiguredLeds, i * 16, part, 0, 16);
                    TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.Off, part);
                }

                Assert.AreEqual(-1, mock.LastApply);
            }
        }

        [TestMethod]
        public void SetMultiple()
        {
            motherboard.LedSettings[0] = new StaticLedSetting(Color.Red, 50);
            motherboard.LedSettings[1] = new StaticLedSetting(Color.Purple, 100);
            motherboard.LedSettings[2] = new StaticLedSetting(Color.DodgerBlue, 100);

            for (int repeat = 0; repeat < 3; repeat++)
            {
                motherboard.Set();

                Assert.AreEqual(GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS * 16, mock.ConfiguredLeds.Length);

                byte[] part = new byte[16];

                Array.Copy(mock.ConfiguredLeds, 0, part, 0, 16);
                TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.StaticRed50, part);

                Array.Copy(mock.ConfiguredLeds, 1 * 16, part, 0, 16);
                TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.StaticPurple, part);

                Array.Copy(mock.ConfiguredLeds, 2 * 16, part, 0, 16);
                TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.StaticDodgerBlue, part);

                for (int i = 3; i < GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS; i++)
                {
                    Array.Copy(mock.ConfiguredLeds, i * 16, part, 0, 16);
                    TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.Off, part);
                }

                Assert.AreEqual(-1, mock.LastApply);
            }
        }

        [TestMethod]
        public void SetMultiplePartial()
        {
            motherboard.LedSettings[0] = new StaticLedSetting(Color.Red, 50);
            motherboard.LedSettings[1] = new StaticLedSetting(Color.Purple, 100);
            motherboard.LedSettings[2] = new StaticLedSetting(Color.DodgerBlue, 100);

            int[] divisions = {4, 2, 3, 0, 1, 1, 0};
            for (int repeat = 0; repeat < divisions.Length; repeat++)
            {
                motherboard.Set(divisions[repeat]);

                byte[] part = new byte[16];

                Assert.AreEqual(GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS * 16, mock.ConfiguredLeds.Length);

                Array.Copy(mock.ConfiguredLeds, 0, part, 0, 16);
                TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.StaticRed50, part);

                Array.Copy(mock.ConfiguredLeds, 1 * 16, part, 0, 16);
                TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.StaticPurple, part);

                Array.Copy(mock.ConfiguredLeds, 2 * 16, part, 0, 16);
                TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.StaticDodgerBlue, part);

                for (int i = 3; i < GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS; i++)
                {
                    Array.Copy(mock.ConfiguredLeds, i * 16, part, 0, 16);
                    TestHelper.AssertLedSettingsEqual(LedSettingTests.SettingByteArrays.Off, part);
                }

                Assert.AreEqual(1 << divisions[repeat], mock.LastApply);
            }
        }
    }
}
