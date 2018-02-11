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

namespace RGBFusionToolTests.Tests
{
    [TestClass]
    public class RGBFusionToolExeTests
    {
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

        GLedApiv1_0_0Mock mock;
        RGBFusionTool.Application rgbFusionTool;
        StringWriter stdout;
        StringWriter stderr;

        [TestInitialize]
        public void Setup()
        {
            mock = new GLedApiv1_0_0Mock(true);
            stdout = new StringWriter();
            stderr = new StringWriter();
            rgbFusionTool = new RGBFusionTool.Application(new LazyTestMotherboard(mock), stdout, stderr);
        }

        [TestMethod]
        public void Help()
        {
            rgbFusionTool.Main(new string[]{"--help"});

            Assert.IsTrue(string.IsNullOrEmpty(stderr.GetStringBuilder().ToString()));
            Assert.IsTrue(stdout.GetStringBuilder().ToString().StartsWith("Usage:"));
        }
    }
}
