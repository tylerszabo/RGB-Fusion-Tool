// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using GvLedLibDotNet;
using GvLedLibDotNet.Raw;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GvLedLibDotNetTests.Tests
{
    [TestClass]
    public class GvLedApiTests
    {
        GvLedLibv1_0Mock mock;
        GvLedLibv1_0Wrapper api;

        [TestInitialize]
        public void Setup()
        {
            mock = new GvLedLibv1_0Mock();
            api = new GvLedLibv1_0Wrapper(mock);
        }

        [TestMethod]
        public void Initialize()
        {
            DeviceType[] results = api.Initialize();
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual(DeviceType.VGA, results[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(GvLedLibv1_0Exception))]
        public void InitializeBadArray()
        {
            mock.DeviceCountOverride = -1;
            api.Initialize();
        }

        [TestMethod]
        [ExpectedException(typeof(GvLedLibv1_0Exception))]
        public void InitializeBadArray2()
        {
            mock.DeviceCountOverride = 9999;
            api.Initialize();
        }

        [TestMethod]
        [ExpectedException(typeof(GvLedLibv1_0Exception))]
        public void InitializeFailure()
        {
            mock.NextReturn = GvLedLibv1_0Mock.Status.ERROR_Fake;
            api.Initialize();
        }

        [TestMethod]
        public void TestSaveAll()
        {
            api.Initialize();
            api.Save(new GvLedLibDotNet.GvLedSettings.GvLedSetting());
        }

        [TestMethod]
        [ExpectedException(typeof(GvLedLibv1_0Exception))]
        public void TestSaveAllFailure()
        {
            api.Initialize();
            mock.NextReturn = GvLedLibv1_0Mock.Status.GV_LED_API_DEVICE_NOT_AVAILABLE;
            api.Save(new GvLedLibDotNet.GvLedSettings.GvLedSetting());
        }
    }
}
