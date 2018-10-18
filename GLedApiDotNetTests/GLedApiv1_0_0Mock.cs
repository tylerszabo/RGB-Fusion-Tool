// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GLedApiDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace GLedApiDotNetTests
{
    public class GLedApiv1_0_0Mock : GLedApiDotNet.Raw.IGLedAPIv1_0_0
    {
        public static RGBFusionMotherboard RGBFusionMotherboardFactory(GLedApiv1_0_0Mock mock)
        {
            return new RGBFusionMotherboard(new GLedApiDotNet.Raw.GLedAPIv1_0_0Wrapper(mock));
        }

        public class Status
        {
            // Known status codes
            public const uint ERROR_SUCCESS = 0x0;
            public const uint ERROR_INVALID_OPERATION = 0x10DD;
            public const uint ERROR_INSUFFICIENT_BUFFER = 0x7A;
            public const uint ERROR_NOT_SUPPORTED = 0x32;

            // May be a real status code, but not documented
            public const uint ERROR_Fake = 0x1234;
        }

        public enum ControlState
        {
            Uninitialized = 1,
            DoneInitAPI,
            DoneGetMaxDivision,
            DoneGetLedLayout,
            DoneSetLedData
        }

		public const int DEFAULT_MAXDIVISIONS = 9;

        private bool stateTrackingEnabled;
        public ControlState State { get; set; }

        public bool IsInitialized
        {
            get
            {
                if (!stateTrackingEnabled)
                {
                    throw new InvalidOperationException("State tracking not enabled");
                }
                switch (State)
                {
                    case ControlState.Uninitialized:
                    case ControlState.DoneInitAPI:
                    case ControlState.DoneGetMaxDivision:
                        return false;
                    case ControlState.DoneGetLedLayout:
                    case ControlState.DoneSetLedData:
                        return true;
                    default:
                        throw new InvalidOperationException(string.Format("Unexpected state {0}", State));
                }
            }
        }
        public uint NextReturn { get; set; } = Status.ERROR_SUCCESS;
        public int MaxDivisions { get; set; } = DEFAULT_MAXDIVISIONS;
        public byte[] ConfiguredLeds { get; private set; } = null;

        private bool applyCalled = false;
        private int lastApply = 0;
        public int LastApply
        {
            get
            {
                Assert.IsTrue(applyCalled, "Expect apply called");
                return lastApply;
            }
        }

        public string Version { get; set; } = "1.0.0";

        private void AssertState(params ControlState[] expected)
        {
            if (stateTrackingEnabled)
            {
                if (expected.Length == 1)
                {
                    Assert.AreEqual(expected[0], State);
                }
                else
                {
                    CollectionAssert.Contains(expected, State);
                }
            }
        }

        public GLedApiv1_0_0Mock(bool trackState = false, ControlState initialState = ControlState.Uninitialized)
        {
            stateTrackingEnabled = trackState;
            State = initialState;
        }

        public uint Apply(int iApplyBit)
        {
            AssertState(ControlState.DoneSetLedData);

            applyCalled = true;
            lastApply = iApplyBit;
            return NextReturn;
        }

        public uint BeatInput(int iCtrl) => throw new NotImplementedException();

        public uint GetCalibrationValue() => throw new NotImplementedException();

        public uint GetLedLayout(byte[] bytArray, int arySize)
        {
            AssertState(ControlState.DoneGetMaxDivision);
            State = ControlState.DoneGetLedLayout;

            Assert.AreEqual(arySize, bytArray.Length, "arySize == bytArray.Length");
            for (int i = 0; i< bytArray.Length; i++)
            {
                bytArray[i] = 1; // A_LED
            }
            return NextReturn;
        }

        public int GetMaxDivision()
        {
            AssertState(ControlState.DoneInitAPI);
            State = ControlState.DoneGetMaxDivision;
            return MaxDivisions;
        }

        public int GetRGBPinType() => throw new NotImplementedException();

        public uint GetSdkVersion(StringBuilder lpBuf, int bufSize)
        {
            Assert.IsTrue(bufSize >= 16, "bufSize >= 16");

            lpBuf.Append(Version);
            return NextReturn;
        }

        public uint Get_IT8295_FwVer(byte[] bytArray, int arySize) => throw new NotImplementedException();

        public uint InitAPI()
        {
            AssertState(ControlState.Uninitialized);
            State = ControlState.DoneInitAPI;
            return NextReturn;
        }

        public uint IT8295_Reset()
        {
            AssertState(ControlState.DoneGetLedLayout);
            return NextReturn;
        }

        public int MonocLedCtrlSupport() => throw new NotImplementedException();

        public uint RGBCalibration_Done(int cal_div) => throw new NotImplementedException();

        public uint RGBCalibration_Step1(int cal_div) => throw new NotImplementedException();

        public void RGBCalibration_Step2() => throw new NotImplementedException();

        public void RGBCalibration_Step3() => throw new NotImplementedException();

        public uint RGBPin_Type1(int pin1, int pin2, int pin3) => throw new NotImplementedException();

        public uint SetLedData(byte[] bytArray, int arySize)
        {
            AssertState(ControlState.DoneGetLedLayout, ControlState.DoneSetLedData);
            State = ControlState.DoneSetLedData;

            Assert.AreEqual(arySize, bytArray.Length, "arySize == bytArray.Length");
            ConfiguredLeds = bytArray;

            return NextReturn;
        }

        public bool SetMonocLedMode(int mnoLedMode) => throw new NotImplementedException();
    }
}
