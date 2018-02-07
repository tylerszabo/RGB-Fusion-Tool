// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using GLedApiDotNet.Raw;
using GLedApiDotNet.LedSettings;

namespace GLedApiDotNetTests.Tests
{
    [TestClass]
    public class GLedApiTests
    {
        GLedApiv1_0_0Mock mock;
        GLedAPIv1_0_0Wrapper api;

        [TestInitialize]
        public void Setup()
        {
            mock = new GLedApiv1_0_0Mock();
            api = new GLedAPIv1_0_0Wrapper(mock);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void MetaApplyNotCalled()
        {
            Assert.AreEqual(0, mock.LastApply);
        }

        [TestMethod]
        public void ApplyAll()
        {
            api.Apply(-1);
            Assert.AreEqual(-1, mock.LastApply);
        }

        [TestMethod]
        [ExpectedException(typeof(GLedAPIv1_0_0Exception))]
        public void GetMaxDivisionFailure()
        {
            mock.MaxDivisions = -1;
            api.GetMaxDivision();
        }

        [DataRow(1)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        [DataTestMethod]
        public void GetMaxDivision(int maxDivisions)
        {
            mock.MaxDivisions = maxDivisions;
            Assert.AreEqual(maxDivisions, api.GetMaxDivision());
        }

        [TestMethod]
        [ExpectedException(typeof(GLedAPIv1_0_0Exception))]
        public void GetSdkVersionFailure()
        {
            mock.NextReturn = GLedApiv1_0_0Mock.Status.ERROR_INSUFFICIENT_BUFFER;
			api.GetSdkVersion();
        }

        [TestMethod]
        [ExpectedException(typeof(GLedAPIv1_0_0Exception))]
        public void InitAPIFailure()
        {
            mock.NextReturn = GLedApiv1_0_0Mock.Status.ERROR_INVALID_OPERATION;
			api.Initialize();
        }

		[DataRow(GLedApiv1_0_0Mock.Status.ERROR_INSUFFICIENT_BUFFER)]
		[DataRow(GLedApiv1_0_0Mock.Status.ERROR_INVALID_OPERATION)]
		[DataRow(GLedApiv1_0_0Mock.Status.ERROR_NOT_SUPPORTED)]
		[DataTestMethod]
		[ExpectedException(typeof(GLedAPIv1_0_0Exception))]
		public void GetLedLayouFailure(uint nextReturn)
		{
			mock.NextReturn = nextReturn;
			api.GetLedLayout(GLedApiv1_0_0Mock.DEFAULT_MAXDIVISIONS);
        }

		[TestMethod]
		[ExpectedException(typeof(GLedAPIv1_0_0Exception))]
		public void SetLedDataFailure()
		{
            mock.NextReturn = GLedApiv1_0_0Mock.Status.ERROR_INVALID_OPERATION;
			api.SetLedData( new LedSetting[] {
				new OffLedSetting(),
				new OffLedSetting(),
				new OffLedSetting(),
				new OffLedSetting(),
				new OffLedSetting(),
				new OffLedSetting(),
				new OffLedSetting(),
				new OffLedSetting(),
				new OffLedSetting() }
			);
        }

		[TestMethod]
		[ExpectedException(typeof(GLedAPIv1_0_0Exception))]
		public void ApplyFailure()
		{
            mock.NextReturn = GLedApiv1_0_0Mock.Status.ERROR_INVALID_OPERATION;
			api.Apply(-1);
        }

		[TestMethod]
		[ExpectedException(typeof(GLedAPIv1_0_0Exception))]
		public void IT8295_ResetFailure()
		{
            mock.NextReturn = GLedApiv1_0_0Mock.Status.ERROR_INVALID_OPERATION;
			api.IT8295_Reset();
        }
    }
}
