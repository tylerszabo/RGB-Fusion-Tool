// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GLedApiDotNet;
using GLedApiDotNetTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RGBFusionToolTests.Tests
{
    [TestClass]
    public class RGBFusionToolExeTests
    {
        public readonly Regex ANY = new Regex(".+", RegexOptions.Compiled);
        public readonly Regex USAGE = new Regex("^Usage:", RegexOptions.Compiled);

        private class LazyTestMotherboard : IRGBFusionMotherboard
        {
            Lazy<RGBFusionMotherboard> motherboard;
            public LazyTestMotherboard(GLedApiv1_0_0Mock mock)
            {
                motherboard = new Lazy<RGBFusionMotherboard>(() => GLedApiv1_0_0Mock.RGBFusionMotherboardFactory(mock));
            }

            public IMotherboardLedLayout Layout => motherboard.Value.Layout;
            public IMotherboardLedSettings LedSettings => motherboard.Value.LedSettings;
            public void Set(params int[] divisions) => motherboard.Value.Set(divisions);
            public void SetAll(GLedApiDotNet.LedSettings.LedSetting ledSetting) => motherboard.Value.SetAll(ledSetting);
        }

        private GLedApiv1_0_0Mock mock;
        private RGBFusionTool.Application rgbFusionTool;
        private StringBuilder stdout;
        private StringBuilder stderr;

        [TestInitialize]
        public void Setup()
        {
            mock = new GLedApiv1_0_0Mock(true);
            stdout = new StringBuilder();
            stderr = new StringBuilder();
            rgbFusionTool = new RGBFusionTool.Application(new LazyTestMotherboard(mock), new StringWriter(stdout), new StringWriter(stderr));
        }

        [DataRow(new string[] { "--help" })]
        [DataRow(new string[] { "-h" })]
        [DataRow(new string[] { "-?" })]
        [DataTestMethod]
        public void Help(string[] args)
        {
            rgbFusionTool.Main(args);

            Assert.IsFalse(mock.IsInitialized, "Expect uninitialized");
            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), USAGE, "Expect stdout shows usage");
        }

        [TestMethod]
        public void NoArgs()
        {
            rgbFusionTool.Main(new string[]{});

            Assert.IsFalse(mock.IsInitialized, "Expect uninitialized");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");
        }

        // Color options
        [DataRow(new string[] { "--color" }, DisplayName = "No value")]
        [DataRow(new string[] { "--color=Invalid" }, DisplayName = "Bad name")]
        // ColorCycle options
        [DataRow(new string[] { "--colorcycle=Invalid" }, DisplayName = "Cycle, Bad name")]
        [DataRow(new string[] { "--colorcycle=9999" }, DisplayName = "Cycle, Too high")]
        // Brightness options
        [DataRow(new string[] { "--color=Red", "--brightness" }, DisplayName = "Brightness, No value")]
        [DataRow(new string[] { "--color=Red", "--brightness=Invalid" }, DisplayName = "Brightness, Bad name")]
        [DataRow(new string[] { "--color=Red", "--brightness=101" }, DisplayName = "Brightness, Too high")]
        [DataTestMethod]
        public void BadOptions(string[] args)
        {
            try
            {
                rgbFusionTool.Main(args);
                Assert.Fail("Expect exception thrown");
            }
            catch (Exception)
            {
                // This pattern avoids a dependency on any particular options library
            }

            Assert.IsFalse(mock.IsInitialized, "Expect uninitialized");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");
            StringAssert.Matches(stderr.ToString(), USAGE, "Expect stderr shows usage");
        }


        // By Name
        [DataRow(new string[] { "--color=DodgerBlue" }, DisplayName = "--color=DodgerBlue")]
        [DataRow(new string[] { "--color", "DodgerBlue" }, DisplayName = "--color DodgerBlue")]
        [DataRow(new string[] { "--static=DodgerBlue" }, DisplayName = "--static=DodgerBlue")]
        [DataRow(new string[] { "--static", "DodgerBlue" }, DisplayName = "--static DodgerBlue")]
        [DataRow(new string[] { "-cDodgerBlue" }, DisplayName = "-cDodgerBlue")]
        [DataRow(new string[] { "-c", "DodgerBlue" }, DisplayName = "-c DodgerBlue")]
        [DataRow(new string[] { "-c DodgerBlue" }, DisplayName = "-c DodgerBlue (OneWord)")]
        // By Hex RGB
        [DataRow(new string[] { "--color=1e90fF" }, DisplayName = "--color=1e90fF")]
        [DataRow(new string[] { "--color", "1e90fF" }, DisplayName = "--color 1e90fF")]
        [DataRow(new string[] { "--color=1e90fF" }, DisplayName = "--color=1e90fF")]
        [DataRow(new string[] { "--color", "1e90fF" }, DisplayName = "--color 1e90fF")]
        [DataRow(new string[] { "-c1E90Ff" }, DisplayName = "-c1E90Ff")]
        [DataRow(new string[] { "-c", "1E90Ff" }, DisplayName = "-c 1E90Ff")]
        [DataRow(new string[] { "-c 1E90Ff" }, DisplayName = "-c 1E90Ff (OneWord)")]
        [DataTestMethod]
        public void Color(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");


            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.StaticDodgerBlue,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--color=Red", "-b50" }, DisplayName = "-b50")]
        [DataRow(new string[] { "--color=Red", "-b", "50" }, DisplayName = "-b 50")]
        [DataRow(new string[] { "--color=Red", "-b 50" }, DisplayName = "-b 50 (OneWord)")]
        [DataRow(new string[] { "--color=Red", "--brightness=50" }, DisplayName = "--brightness=50")]
        [DataRow(new string[] { "--color=Red", "--brightness", "50" }, DisplayName = "--brightness 50")]
        [DataTestMethod]
        public void Brightness(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.StaticRed50,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--colorcycle" }, DisplayName = "--colorcycle")]
        [DataRow(new string[] { "--cycle" }, DisplayName = "--cycle")]
        [DataRow(new string[] { "--colorcycle=1" }, DisplayName = "--colorcycle=1")]
        [DataRow(new string[] { "--cycle=1" }, DisplayName = "--cycle=1")]
        [DataRow(new string[] { "--colorcycle=1.0" }, DisplayName = "--colorcycle=1.0")]
        [DataRow(new string[] { "--cycle=1.0" }, DisplayName = "--cycle=1.0")]
        [DataTestMethod]
        public void ColorCycle_1s(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleA_1s,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--colorcycle=4" }, DisplayName = "--colorcycle=4")]
        [DataRow(new string[] { "--cycle=4" }, DisplayName = "--cycle=4")]
        [DataRow(new string[] { "--colorcycle=4.0" }, DisplayName = "--colorcycle=4.0")]
        [DataRow(new string[] { "--cycle=4.0" }, DisplayName = "--cycle=4.0")]
        [DataTestMethod]
        public void ColorCycle_4s(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleA_4s,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--colorcycle=0.5" }, DisplayName = "--colorcycle=0.5")]
        [DataRow(new string[] { "--cycle=0.5" }, DisplayName = "--cycle=0.5")]
        [DataRow(new string[] { "--colorcycle=.5" }, DisplayName = "--colorcycle=.5")]
        [DataRow(new string[] { "--cycle=.5" }, DisplayName = "--cycle=.5")]
        [DataTestMethod]
        public void ColorCycle_500ms(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleA_500ms,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }
    }
}
