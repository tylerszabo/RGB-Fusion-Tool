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
using System.Collections.Generic;
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
        public readonly Regex COLORCYCLE = new Regex("\\b(color ?)?cycle\\b.*\\b1(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
            public void Set(IEnumerable<int> divisions) => motherboard.Value.Set(divisions);
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

        [DataRow(new string[] { "--help" }, DisplayName = "--help")]
        [DataRow(new string[] { "-h" }, DisplayName = "-h")]
        [DataRow(new string[] { "-?" }, DisplayName = "-?")]
        [DataTestMethod]
        public void Help(string[] args)
        {
            rgbFusionTool.Main(args);

            Assert.IsFalse(mock.IsInitialized, "Expect uninitialized");
            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), USAGE, "Expect stdout shows usage");

            // Ensure each supported option appears in help
            string[] supportedOptions = {
                "zone",

                "static",
                "colorcycle",
                "pulse",
                "flash",
                "off",

                "verbose",
                "list",
                "help",
                "version"
            };
            foreach (string option in supportedOptions)
            {
                Regex regex = new Regex(string.Format("--{0}\\b", option));
                StringAssert.Matches(stdout.ToString(), regex, "Expect stdout shows usage");
            }
        }

        [DataRow(new string[] { "--version" }, DisplayName = "--version")]
        [DataTestMethod]
        public void Version(string[] args)
        {
            rgbFusionTool.Main(args);

            Assert.IsFalse(mock.IsInitialized, "Expect uninitialized");
            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");

            string[] requiredPatterns = {
                "\\bRGB Fusion Tool\\b\\s+\\b\\d+(\\.\\d+){1,3}\\b", // name and version
                "\\b[cC]opyright\\b\\s+(\\([cC]\\)\\s+)\\b2018\\b\\s+\\bTyler Szabo\\b", // copyright year and name

                "\\bGNU General Public License\\b",
                "\\bFree Software Foundation\\b",
                "\\bversion 3\\b",
                "\\bWITHOUT ANY WARRANTY\\b",
                "https://www.gnu.org/licenses/"
            };
            foreach (string pattern in requiredPatterns)
            {
                Regex regex = new Regex(pattern);
                StringAssert.Matches(stdout.ToString(), regex, "Expect stdout shows version information");
            }
        }

        [TestMethod]
        public void NoArgs()
        {
            rgbFusionTool.Main(new string[]{});

            Assert.IsFalse(mock.IsInitialized, "Expect uninitialized");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");
        }

        // Color options
        [DataRow(new string[] { "--color" }, DisplayName = "--color")]
        [DataRow(new string[] { "--color=Invalid" }, DisplayName = "--color=Invalid")]
        [DataRow(new string[] { "--color", "--color=Red" }, DisplayName = "--color --color=Red")]
        // ColorCycle options
        [DataRow(new string[] { "--colorcycle=Invalid" }, DisplayName = "--colorcycle=Invalid")]
        [DataRow(new string[] { "--colorcycle=9999" }, DisplayName = "--colorcycle=9999")]
        [DataRow(new string[] { "--colorcycle=--colorcycle" }, DisplayName = "--colorcycle=--colorcycle")]
        // Brightness options
        [DataRow(new string[] { "--color=Red", "--brightness" }, DisplayName = "--color=Red --brightness")]
        [DataRow(new string[] { "--color=Red", "--brightness=Invalid" }, DisplayName = "--color=Red --brightness=Invalid")]
        [DataRow(new string[] { "--color=Red", "--brightness=101" }, DisplayName = "--color=Red --brightness=101")]
        // Zone options
        [DataRow(new string[] { "--zone=-1", "--color=DodgerBlue" }, DisplayName = "--zone=-1 --color=DodgerBlue")]
        [DataRow(new string[] { "--zone=99", "--color=DodgerBlue" }, DisplayName = "--zone=99 --color=DodgerBlue")]
        // Mixing zones & default
        [DataRow(new string[] { "--colorcycle", "--zone=0", "--color=DodgerBlue" }, DisplayName = "--colorcycle --zone=0 --color=DodgerBlue")]
        // Extra parameters
        [DataRow(new string[] { "1" }, DisplayName = "1")]
        [DataRow(new string[] { "0" }, DisplayName = "0")]
        [DataRow(new string[] { "--badopt" }, DisplayName = "--badopt")]
        [DataRow(new string[] { "badopt" }, DisplayName = "badopt")]
        [DataRow(new string[] { "--", "badopt" }, DisplayName = "-- badopt")]
        // Ambiguious options
        [DataRow(new string[] { "--colorcycle", "4.0" }, DisplayName = "--colorcycle 4.0")] // Optional values expect explicit "="
        [DataRow(new string[] { "--colorcycle 4.0" }, DisplayName = "--colorcycle 4.0 (OneWord)")] // Optional values expect explicit "="
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

        [DataRow(new string[] { "-v", "--color=DodgerBlue" }, DisplayName = "-v --color=DodgerBlue")]
        [DataRow(new string[] { "-vv", "--color=DodgerBlue" }, DisplayName = "-vv --color=DodgerBlue")]
        [DataRow(new string[] { "--verbose", "--color=DodgerBlue" }, DisplayName = "--verbose --color=DodgerBlue")]
        [DataRow(new string[] { "--color=DodgerBlue", "--verbose" }, DisplayName = "--color=DodgerBlue --verbose")]
        [DataTestMethod]
        public void Color_verbose_name(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("color\\b.*dodgerblue", RegexOptions.IgnoreCase), "Expect stdout includes color name");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.StaticDodgerBlue,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "-v", "--color=1E90FF" }, DisplayName = "-v --color=1E90FF")]
        [DataRow(new string[] { "-vv", "--color=1E90FF" }, DisplayName = "-vv --color=1E90FF")]
        [DataRow(new string[] { "--verbose", "--color=1E90FF" }, DisplayName = "--verbose --color=1E90FF")]
        [DataRow(new string[] { "--color=1E90FF", "--verbose" }, DisplayName = "--color=1E90FF --verbose")]
        [DataTestMethod]
        public void Color_verbose_rgb(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("color\\b.*\\b30\\b.*\\b144\\b.*\\b255\\b", RegexOptions.IgnoreCase), "Expect stdout includes color values");

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

        [DataRow(new string[] { "-v", "--color=Red", "--brightness=50" }, DisplayName = "-v --color=Red --brightness=50")]
        [DataRow(new string[] { "-vv", "--color=Red", "--brightness=50" }, DisplayName = "-vv --color=Red --brightness=50")]
        [DataRow(new string[] { "--verbose", "--color=Red", "--brightness=50" }, DisplayName = "--verbose --color=Red --brightness=50")]
        [DataRow(new string[] { "--color=Red", "--brightness=50", "--verbose" }, DisplayName = "--color=Red --brightness=50 --verbose")]
        [DataTestMethod]
        public void Brightness_verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("brightness\\b.*\\b50\\b", RegexOptions.IgnoreCase), "Expect stdout includes brightness value");

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
        public void ColorCycleA_1s(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleA_1s,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "-v", "--colorcycle" }, DisplayName = "-v --colorcycle")]
        [DataRow(new string[] { "-vv", "--colorcycle" }, DisplayName = "-vv --colorcycle")]
        [DataRow(new string[] { "--verbose", "--colorcycle" }, DisplayName = "--verbose --colorcycle")]
        [DataRow(new string[] { "--colorcycle", "--verbose" }, DisplayName = "--colorcycle --verbose")]
        [DataTestMethod]
        public void ColorCycleA_1s_verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), COLORCYCLE, "Expect stdout includes mode and time");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleA_1s,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--colorcycle=4" }, DisplayName = "--colorcycle=4")]
        [DataRow(new string[] { "--cycle=4" }, DisplayName = "--cycle=4")]
        [DataRow(new string[] { "--colorcycle=4.0" }, DisplayName = "--colorcycle=4.0")]
        [DataRow(new string[] { "--cycle=4.0" }, DisplayName = "--cycle=4.0")]
        [DataTestMethod]
        public void ColorCycleA_4s(string[] args)
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
        public void ColorCycleA_500ms(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleA_500ms,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--colorcycle=1.5", "--minbrightness=20", "--numcolors=2", "--cyclepulse" }, DisplayName = "--colorcycle=1.5 --minbrightness=20 --numcolors=2 --cyclepulse")]
        [DataRow(new string[] { "--colorcycle=1.5", "--brightness=100", "--minbrightness=20", "--numcolors=2", "--cyclepulse" }, DisplayName = "--colorcycle=1.5 --brightness=100 --minbrightness=20 --numcolors=2 --cyclepulse")]
        [DataRow(new string[] { "--colorcycle=1.5", "--maxbrightness=100", "--minbrightness=20", "--numcolors=2", "--cyclepulse" }, DisplayName = "--colorcycle=1.5 --maxbrightness=100 --minbrightness=20 --numcolors=2 --cyclepulse")]
        [DataRow(new string[] { "--maxbrightness=100", "--minbrightness=20", "--numcolors=2", "--cyclepulse", "--colorcycle=1.5" }, DisplayName = "--maxbrightness=100 --minbrightness=20 --numcolors=2 --cyclepulse --colorcycle=1.5")]
        [DataTestMethod]
        public void ColorCycleB(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleB,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--colorcycle=1.5", "--minbrightness=20", "--numcolors=2", "--cyclepulse" }, DisplayName = "--verbose --colorcycle=1.5 --minbrightness=20 --numcolors=2 --cyclepulse")]
        [DataTestMethod]
        public void ColorCycleB_verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b(color ?)?cycle\\b.*\\b1.5\\s?s\\b", RegexOptions.IgnoreCase), "Expect stdout includes mode and time");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bpulse\\b", RegexOptions.IgnoreCase), "Expect stdout to note pulse option");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b20\\b", RegexOptions.IgnoreCase), "Expect stdout to note min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bred\\b", RegexOptions.IgnoreCase), "Expect stdout to note red");
            StringAssert.Matches(stdout.ToString(), new Regex("\\borange\\b", RegexOptions.IgnoreCase), "Expect stdout to note orange");
            StringAssert.DoesNotMatch(stdout.ToString(), new Regex("\\byellow\\b", RegexOptions.IgnoreCase), "Expect stdout to note yellow");
            StringAssert.DoesNotMatch(stdout.ToString(), new Regex("\\bgreen\\b", RegexOptions.IgnoreCase), "Expect stdout to note green");
            StringAssert.DoesNotMatch(stdout.ToString(), new Regex("\\bblue\\b", RegexOptions.IgnoreCase), "Expect stdout to note blue");
            StringAssert.DoesNotMatch(stdout.ToString(), new Regex("\\bindigo\\b", RegexOptions.IgnoreCase), "Expect stdout to note indigo");
            StringAssert.DoesNotMatch(stdout.ToString(), new Regex("\\bviolet\\b", RegexOptions.IgnoreCase), "Expect stdout to note violet");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleB,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--pulse=Red", "--maxbrightness=75", "--minbrightness=25", "--fadeon=30", "--fadeoff=10" }, DisplayName = "--pulse=Red --maxbrightness=75 --minbrightness=25 --fadeon=30 --fadeoff=10")]
        [DataTestMethod]
        public void Pulse(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.PulseA,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--pulse=Red", "--maxbrightness=75", "--minbrightness=25", "--fadeon=30", "--fadeoff=10" }, DisplayName = "--verbose --pulse=Red --maxbrightness=75 --minbrightness=25 --fadeon=30 --fadeoff=10")]
        [DataTestMethod]
        public void Pulse_verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bpulse\\b", RegexOptions.IgnoreCase),"Expect stdout includes pulse config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bred\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b75\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b25\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b30(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes fade on time");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b10(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes fade off time");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.PulseA,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--flash=Blue", "--maxbrightness=100", "--minbrightness=10", "--time=0.3", "--interval=1", "--flashcycle=4", "--count=3" }, DisplayName = "--flash=Blue --maxbrightness=100 --minbrightness=10 --time=0.3 --interval=1 --flashcycle=4 --count=3")]
        [DataTestMethod]
        public void Flash(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.FlashC,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--flash=Blue", "--maxbrightness=100", "--minbrightness=10", "--time=0.3", "--interval=1", "--flashcycle=4", "--count=3" }, DisplayName = "--verbose --flash=Blue --maxbrightness=100 --minbrightness=10 --time=0.3 --interval=1 --flashcycle=4 --count=3")]
        [DataTestMethod]
        public void Flash_verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bflash\\b", RegexOptions.IgnoreCase),"Expect stdout includes flash config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bblue\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b100\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b10\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b0?\\.3(0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes on/off time");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b1(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes interval time");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b4(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes cycle time");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b2\\b", RegexOptions.IgnoreCase),"Expect stdout includes flash count");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.FlashC,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-a" }, DisplayName = "--digital-a")]
        [DataRow(new string[] { "--digital-a", "--maxbrightness=100", "--minbrightness=0", "--speed=1", "--rtl" }, DisplayName = "--digital-a --maxbrightness=100 --minbrightness=0 --speed=1 --rtl")]
        [DataTestMethod]
        public void DigitalA1(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalA1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-a" }, DisplayName = "--verbose --digital-a")]
        [DataRow(new string[] { "--verbose", "--digital-a", "--maxbrightness=100", "--minbrightness=0", "--speed=1", "--rtl" }, DisplayName = "--verbose --digital-a --maxbrightness=100 --minbrightness=0 --speed=1 --rtl")]
        [DataTestMethod]
        public void DigitalA1Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitala\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-a config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b100\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b0\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b1(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes speed");
            StringAssert.Matches(stdout.ToString(), new Regex("\\brighttoleft\\b", RegexOptions.IgnoreCase),"Expect stdout includes direction");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalA1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-a", "--maxbrightness=80", "--minbrightness=20", "--speed=5", "--ltr" }, DisplayName = "--digital-a --maxbrightness=80 --minbrightness=20 --speed=5 --ltr")]
        [DataTestMethod]
        public void DigitalA2(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalA2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-a", "--maxbrightness=80", "--minbrightness=20", "--speed=5", "--ltr" }, DisplayName = "--verbose --digital-a --maxbrightness=80 --minbrightness=20 --speed=5 --ltr")]
        [DataTestMethod]
        public void DigitalA2Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitala\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-a config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b80\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b20\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b5(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes speed");
            StringAssert.Matches(stdout.ToString(), new Regex("\\blefttoright\\b", RegexOptions.IgnoreCase),"Expect stdout includes direction");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalA2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-b=Red", "--speed=2" }, DisplayName = "--digital-b=Red --speed=2")]
        [DataRow(new string[] { "--digital-b=Red", "--maxbrightness=100", "--minbrightness=0", "--speed=2" }, DisplayName = "--digital-b=Red --maxbrightness=100 --minbrightness=0 --speed=2")]
        [DataTestMethod]
        public void DigitalB1(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalB1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-b=Red", "--speed=2" }, DisplayName = "--verbose --digital-b=Red --speed=2")]
        [DataRow(new string[] { "--verbose", "--digital-b=Red", "--maxbrightness=100", "--minbrightness=0", "--speed=2" }, DisplayName = "--verbose --digital-b=Red --maxbrightness=100 --minbrightness=0 --speed=2")]
        [DataTestMethod]
        public void DigitalB1Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalb\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-b config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bred\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b100\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b0\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b2(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes speed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalB1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-b=DodgerBlue", "--maxbrightness=90", "--minbrightness=10", "--speed=3.5" }, DisplayName = "--digital-b=DodgerBlue --maxbrightness=90 --minbrightness=10 --speed=3.5")]
        [DataTestMethod]
        public void DigitalB2(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalB2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-b=DodgerBlue", "--maxbrightness=90", "--minbrightness=10", "--speed=3.5" }, DisplayName = "--verbose --digital-b=DodgerBlue --maxbrightness=90 --minbrightness=10 --speed=3.5")]
        [DataTestMethod]
        public void DigitalB2Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalb\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-b config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdodgerblue\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b90\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b10\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b3.5\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes speed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalB2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-c=Red" }, DisplayName = "--digital-c=Red")]
        [DataRow(new string[] { "--digital-c=Red", "--maxbrightness=100", "--minbrightness=0", "--interval=1", "--dimspeed=0" }, DisplayName = "--digital-c=Red --maxbrightness=100 --minbrightness=0 --interval=1 --dimspeed=0")]
        [DataTestMethod]
        public void DigitalC1(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalC1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-c=Red" }, DisplayName = "--verbose --digital-c=Red")]
        [DataRow(new string[] { "--verbose", "--digital-c=Red", "--maxbrightness=100", "--minbrightness=0", "--interval=1", "--dimspeed=0" }, DisplayName = "--verbose --digital-c=Red --maxbrightness=100 --minbrightness=0 --interval=1 --dimspeed=0")]
        [DataTestMethod]
        public void DigitalC1Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalc\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-c config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bred\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b100\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b0\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b1(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes interval");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b0\\b", RegexOptions.IgnoreCase),"Expect stdout includes dimspeed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalC1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-c=DodgerBlue", "--maxbrightness=50", "--minbrightness=5", "--interval=2", "--dimspeed=50" }, DisplayName = "--digital-c=DodgerBlue --maxbrightness=50 --minbrightness=5 --interval=2 --dimspeed=50")]
        [DataTestMethod]
        public void DigitalC2(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalC2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-c=DodgerBlue", "--maxbrightness=50", "--minbrightness=5", "--interval=2", "--dimspeed=50" }, DisplayName = "--verbose --digital-c=DodgerBlue --maxbrightness=50 --minbrightness=5 --interval=2 --dimspeed=50")]
        [DataTestMethod]
        public void DigitalC2Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalc\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-c config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdodgerblue\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b50\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b5\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b2(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes interval");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b50\\b", RegexOptions.IgnoreCase),"Expect stdout includes dimspeed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalC2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-c=Lime", "--maxbrightness=80", "--interval=3.5", "--dimspeed=100" }, DisplayName = "--digital-c=Lime --maxbrightness=80 --interval=3.5 --dimspeed=100" )]
        [DataRow(new string[] { "--digital-c=Lime", "--maxbrightness=80", "--minbrightness=0", "--interval=3.5", "--dimspeed=100" }, DisplayName = "--digital-c=Lime --maxbrightness=80 --minbrightness=0 --interval=3.5 --dimspeed=100" )]
        [DataTestMethod]
        public void DigitalC3(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalC3,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-c=Lime", "--maxbrightness=80", "--interval=3.5", "--dimspeed=100" }, DisplayName = "--verbose --digital-c=Lime --maxbrightness=80 --interval=3.5 --dimspeed=100" )]
        [DataRow(new string[] { "--verbose", "--digital-c=Lime", "--maxbrightness=80", "--minbrightness=0", "--interval=3.5", "--dimspeed=100" }, DisplayName = "--verbose --digital-c=Lime --maxbrightness=80 --minbrightness=0 --interval=3.5 --dimspeed=100" )]
        [DataTestMethod]
        public void DigitalC3Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalc\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-c config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\blime\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b80\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b0\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b3.5\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes interval");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b100\\b", RegexOptions.IgnoreCase),"Expect stdout includes dimspeed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalC3,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-d=Blue", "--maxbrightness=80", "--speed=4" }, DisplayName = "--digital-d=Blue --maxbrightness=80 --speed=4")]
        [DataRow(new string[] { "--digital-d=Blue", "--maxbrightness=80", "--minbrightness=0", "--speed=4" }, DisplayName = "--digital-d=Blue --maxbrightness=80 --minbrightness=0 --speed=4")]
        [DataTestMethod]
        public void DigitalD1(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalD1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-d=Blue", "--maxbrightness=80", "--speed=4" }, DisplayName = "--verbose --digital-d=Blue --maxbrightness=80 --speed=4")]
        [DataRow(new string[] { "--verbose", "--digital-d=Blue", "--maxbrightness=80", "--minbrightness=0", "--speed=4" }, DisplayName = "--verbose --digital-d=Blue --maxbrightness=80 --minbrightness=0 --speed=4")]
        [DataTestMethod]
        public void DigitalD1Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitald\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-d config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bblue\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b80\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b0\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b4(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes speed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalD1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-d=Crimson", "--maxbrightness=50", "--minbrightness=10" }, DisplayName = "--digital-d=Crimson --maxbrightness=50 --minbrightness=10")]
        [DataRow(new string[] { "--digital-d=Crimson", "--maxbrightness=50", "--minbrightness=10", "--speed=1" }, DisplayName = "--digital-d=Crimson --maxbrightness=50 --minbrightness=10 --speed=1")]
        [DataTestMethod]
        public void DigitalD2(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalD2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-d=Crimson", "--maxbrightness=50", "--minbrightness=10" }, DisplayName = "--verbose --digital-d=Crimson --maxbrightness=50 --minbrightness=10")]
        [DataRow(new string[] { "--verbose", "--digital-d=Crimson", "--maxbrightness=50", "--minbrightness=10", "--speed=1" }, DisplayName = "--verbose --digital-d=Crimson --maxbrightness=50 --minbrightness=10 --speed=1")]
        [DataTestMethod]
        public void DigitalD2Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitald\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-d config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bcrimson\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b50\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b10\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b1(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes speed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalD2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-e=Green", "--maxbrightness=60", "--minbrightness=10" }, DisplayName = "--digital-e=Green --maxbrightness=60 --minbrightness=10")]
        [DataRow(new string[] { "--digital-e=Green", "--maxbrightness=60", "--minbrightness=10", "--cycletime=1" }, DisplayName = "--digital-e=Green --maxbrightness=60 --minbrightness=10 --cycletime=1")]
        [DataTestMethod]
        public void DigitalE1(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalE1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-e=Green", "--maxbrightness=60", "--minbrightness=10" }, DisplayName = "--verbose --digital-e=Green --maxbrightness=60 --minbrightness=10")]
        [DataRow(new string[] { "--verbose", "--digital-e=Green", "--maxbrightness=60", "--minbrightness=10", "--cycletime=1" }, DisplayName = "--verbose --digital-e=Green --maxbrightness=60 --minbrightness=10 --cycletime=1")]
        [DataTestMethod]
        public void DigitalE1Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitale\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-d config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bgreen\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b60\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b10\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b1(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes cycletime");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalE1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-e=Blue", "--maxbrightness=70", "--minbrightness=20", "--cycletime=3" }, DisplayName = "--digital-e=Blue --maxbrightness=70 --minbrightness=20 --cycletime=3")]
        [DataTestMethod]
        public void DigitalE2(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalE2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-e=Blue", "--maxbrightness=70", "--minbrightness=20", "--cycletime=3" }, DisplayName = "--verbose --digital-e=Blue --maxbrightness=70 --minbrightness=20 --cycletime=3")]
        [DataTestMethod]
        public void DigitalE2Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitale\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-d config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bblue\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b70\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b20\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b3(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes cycletime");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalE2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-f", "--maxbrightness=90", "--minbrightness=30", "--speed=2" }, DisplayName = "--digital-f --maxbrightness=90 --minbrightness=30 --speed=2")]
        [DataTestMethod]
        public void DigitalF1(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalF1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-f", "--maxbrightness=90", "--minbrightness=30", "--speed=2" }, DisplayName = "--verbose --digital-f --maxbrightness=90 --minbrightness=30 --speed=2")]
        [DataTestMethod]
        public void DigitalF1Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalf\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-d config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b90\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b30\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b2(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes speed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalF1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-f", "--minbrightness=10", "--speed=6" }, DisplayName = "--digital-f --minbrightness=10 --speed=6")]
        [DataRow(new string[] { "--digital-f", "--maxbrightness=100", "--minbrightness=10", "--speed=6" }, DisplayName = "--digital-f --maxbrightness=100 --minbrightness=10 --speed=6")]
        [DataTestMethod]
        public void DigitalF2(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalF2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-f", "--minbrightness=10", "--speed=6" }, DisplayName = "--verbose --digital-f --minbrightness=10 --speed=6")]
        [DataRow(new string[] { "--verbose", "--digital-f", "--maxbrightness=100", "--minbrightness=10", "--speed=6" }, DisplayName = "--verbose --digital-f --maxbrightness=100 --minbrightness=10 --speed=6")]
        [DataTestMethod]
        public void DigitalF2Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalf\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-d config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b100\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b10\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b6(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes speed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalF2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-g=Red", "--maxbrightness=90", "--minbrightness=20", "--interval=2" }, DisplayName = "--digital-g=Red --maxbrightness=90 --minbrightness=20 --interval=2")]
        [DataRow(new string[] { "--digital-g=Red", "--maxbrightness=90", "--minbrightness=20", "--interval=2", "--dimspeed=0" }, DisplayName = "--digital-g=Red --maxbrightness=90 --minbrightness=20 --interval=2 --dimspeed=0")]
        [DataTestMethod]
        public void DigitalG1(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalG1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-g=Red", "--maxbrightness=90", "--minbrightness=20", "--interval=2" }, DisplayName = "--verbose --digital-g=Red --maxbrightness=90 --minbrightness=20 --interval=2")]
        [DataRow(new string[] { "--verbose", "--digital-g=Red", "--maxbrightness=90", "--minbrightness=20", "--interval=2", "--dimspeed=0" }, DisplayName = "--verbose --digital-g=Red --maxbrightness=90 --minbrightness=20 --interval=2 --dimspeed=0")]
        [DataTestMethod]
        public void DigitalG1Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalg\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-g config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bred\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b90\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b20\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b2(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes interval");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b0\\b", RegexOptions.IgnoreCase),"Expect stdout includes dimspeed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalG1,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-g=Lime", "--minbrightness=30", "--interval=6", "--dimspeed=50" }, DisplayName = "--digital-g=Lime --minbrightness=30 --interval=6 --dimspeed=50")]
        [DataRow(new string[] { "--digital-g=Lime", "--maxbrightness=100", "--minbrightness=30", "--interval=6", "--dimspeed=50" }, DisplayName = "--digital-g=Lime --maxbrightness=100 --minbrightness=30 --interval=6 --dimspeed=50")]
        [DataTestMethod]
        public void DigitalG2(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalG2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-g=Lime", "--minbrightness=30", "--interval=6", "--dimspeed=50" }, DisplayName = "--verbose --digital-g=Lime --minbrightness=30 --interval=6 --dimspeed=50")]
        [DataRow(new string[] { "--verbose", "--digital-g=Lime", "--maxbrightness=100", "--minbrightness=30", "--interval=6", "--dimspeed=50" }, DisplayName = "--verbose --digital-g=Lime --maxbrightness=100 --minbrightness=30 --interval=6 --dimspeed=50")]
        [DataTestMethod]
        public void DigitalG2Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalg\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-g config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\blime\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b100\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b30\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b6(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes interval");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b50\\b", RegexOptions.IgnoreCase),"Expect stdout includes dimspeed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalG2,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--digital-g=Blue", "--maxbrightness=70", "--interval=10", "--dimspeed=100" }, DisplayName = "--digital-g=Blue --maxbrightness=70 --interval=10 --dimspeed=100")]
        [DataRow(new string[] { "--digital-g=Blue", "--maxbrightness=70", "--minbrightness=0", "--interval=10", "--dimspeed=100" }, DisplayName = "--digital-g=Blue --maxbrightness=70 --minbrightness=0 --interval=10 --dimspeed=100")]
        [DataTestMethod]
        public void DigitalG3(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalG3,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--digital-g=Blue", "--maxbrightness=70", "--interval=10", "--dimspeed=100" }, DisplayName = "--verbose --digital-g=Blue --maxbrightness=70 --interval=10 --dimspeed=100")]
        [DataRow(new string[] { "--verbose", "--digital-g=Blue", "--maxbrightness=70", "--minbrightness=0", "--interval=10", "--dimspeed=100" }, DisplayName = "--verbose --digital-g=Blue --maxbrightness=70 --minbrightness=0 --interval=10 --dimspeed=100")]
        [DataTestMethod]
        public void DigitalG3Verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bdigitalg\\b", RegexOptions.IgnoreCase),"Expect stdout includes digital-g config");
            StringAssert.Matches(stdout.ToString(), new Regex("\\bblue\\b", RegexOptions.IgnoreCase), "Expect stdout includes color");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b70\\b", RegexOptions.IgnoreCase),"Expect stdout includes max brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b0\\b", RegexOptions.IgnoreCase),"Expect stdout includes min brightness");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b10(\\.0*)?\\s?s\\b", RegexOptions.IgnoreCase),"Expect stdout includes interval");
            StringAssert.Matches(stdout.ToString(), new Regex("\\b100\\b", RegexOptions.IgnoreCase),"Expect stdout includes dimspeed");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.DigitalG3,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--off" }, DisplayName = "--off")]
        [DataTestMethod]
        public void Off(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.Off,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--verbose", "--off" }, DisplayName = "--verbose --off")]
        [DataTestMethod]
        public void Off_verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("\\boff\\b", RegexOptions.IgnoreCase),"Expect stdout includes off");

            TestHelper.AssertAllLeds(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.Off,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

        [DataRow(new string[] { "--list" }, DisplayName = "--list")]
        [DataRow(new string[] { "-l" }, DisplayName = "-l")]
        // List and set
        [DataRow(new string[] { "--list", "--static=Green" }, DisplayName = "--list --static=Green")]
        [DataTestMethod]
        public void List(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), new Regex("zone \\d+", RegexOptions.IgnoreCase), "Expect stdout lists zones");

            Assert.IsTrue(mock.IsInitialized, "Expect initialized");
        }

        [DataRow(new string[] { "-z1", "--color=DodgerBlue" }, DisplayName = "-z1")]
        [DataRow(new string[] { "-z", "1", "--color=DodgerBlue" }, DisplayName = "-z 1")]
        [DataRow(new string[] { "-z 1", "--color=DodgerBlue" }, DisplayName = "-z 1 (OneWord)")]
        [DataRow(new string[] { "--zone=1", "--color=DodgerBlue" }, DisplayName = "--zone 1")]
        [DataRow(new string[] { "--zone", "1", "--color=DodgerBlue" }, DisplayName = "--zone 1")]
        [DataTestMethod]
        public void Zone1(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertLedDivision(mock, GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.StaticDodgerBlue, GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS, 1);

            Assert.AreEqual(2, mock.LastApply, "Expect applied division 1 only");

            Assert.IsTrue(mock.IsInitialized, "Expect initialized");
        }

        [DataRow(new string[] { "--zone=0", "--color=Red", "--brightness=50" }, 0, DisplayName = "--zone=0 --color=Red --brightness=50")]
        [DataRow(new string[] { "--zone=1", "--color=Red", "--brightness=50" }, 1, DisplayName = "--zone=1 --color=Red --brightness=50")]
        [DataRow(new string[] { "--zone=2", "--color=Red", "--brightness=50" }, 2, DisplayName = "--zone=2 --color=Red --brightness=50")]
        [DataRow(new string[] { "--zone=3", "--color=Red", "--brightness=50" }, 3, DisplayName = "--zone=3 --color=Red --brightness=50")]
        [DataRow(new string[] { "--zone=4", "--color=Red", "--brightness=50" }, 4, DisplayName = "--zone=4 --color=Red --brightness=50")]
        [DataTestMethod]
        public void ZoneN(string[] args, int zone)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertLedDivision(mock, GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.StaticRed50, GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS, zone);

            Assert.AreEqual(0x1 << zone, mock.LastApply, string.Format("Expect applied division {0} only", zone));

            Assert.IsTrue(mock.IsInitialized, "Expect initialized");
        }

        [DataRow(new string[]{ "--zone=0", "--color=Red", "--brightness=50", "--zone=1", "--colorcycle", "--zone=2", "--color=DodgerBlue" }, DisplayName = "--zone=0 --color=Red --brightness=50 --zone=1 --colorcycle --zone=2 --color=DodgerBlue")]
        [DataTestMethod]
        public void ZoneAB(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.DoesNotMatch(stdout.ToString(), ANY, "Expect stdout is empty");

            TestHelper.AssertLedDivision(mock, GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.StaticRed50, GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS, 0);
            TestHelper.AssertLedDivision(mock, GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleA_1s, GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS, 1);
            TestHelper.AssertLedDivision(mock, GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.StaticDodgerBlue, GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS, 2);

            Assert.AreEqual(7, mock.LastApply, string.Format("Expect applied divisions 0, 1, and 2"));

            Assert.IsTrue(mock.IsInitialized, "Expect initialized");
        }

        [DataRow(new string[] { "-v", "--zone=2", "--colorcycle" }, DisplayName = "-v --zone=2 --colorcycle")]
        [DataRow(new string[] { "-vv", "--zone=2", "--colorcycle" }, DisplayName = "-vv --zone=2 --colorcycle")]
        [DataRow(new string[] { "--verbose", "--zone=2", "--colorcycle" }, DisplayName = "--verbose --zone=2 --colorcycle")]
        [DataRow(new string[] { "--zone=2", "--colorcycle", "--verbose" }, DisplayName = "--colorcycle --zone=2 --verbose")]
        [DataTestMethod]
        public void Zone_verbose(string[] args)
        {
            rgbFusionTool.Main(args);

            StringAssert.DoesNotMatch(stderr.ToString(), ANY, "Expect stderr is empty");
            StringAssert.Matches(stdout.ToString(), COLORCYCLE, "Expect stdout includes mode and time");
            StringAssert.Matches(stdout.ToString(), new Regex("zone\\b.*\\b2\\b", RegexOptions.IgnoreCase), "Expect stdout includes zone");

            TestHelper.AssertLedDivision(mock,
                GLedApiDotNetTests.Tests.LedSettingTests.SettingByteArrays.ColorCycleA_1s,
                GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS, 2);

            Assert.AreEqual(4, mock.LastApply, string.Format("Expect applied division 2 only"));
        }
    }
}
