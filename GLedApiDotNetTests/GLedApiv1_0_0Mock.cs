// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace GLedApiDotNetTests
{
    class GLedApiv1_0_0Mock : GLedApiDotNet.Raw.IGLedAPIv1_0_0
    {
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
            DoneInit,
            DoneGetMaxDivision,
            DoneGetLedLayout,
            DoneSetLedData
        }

		public const int DEFAULT_MAXDIVISIONS = 9;

        private bool stateTrackingEnabled;
        private ControlState state;
        public ControlState State { get => state; set => state = value; }

        private uint nextReturn = Status.ERROR_SUCCESS;
        public uint NextReturn { set => nextReturn = value; }

        private int maxDivisions = DEFAULT_MAXDIVISIONS;
        public int MaxDivisions { set => maxDivisions = value; }

        private byte[] ledSettings = null;
        public byte[] ConfiguredLeds { get => ledSettings; }

        private bool applyCalled = false;
        private int lastApply = 0;
        public int LastApply {
            get {
                if (!applyCalled)
                {
                    throw new Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException("Apply was not called");
                }
                return lastApply;
            }
        }

		private string version = "1.0.0";
		public string Version { get => version; set => version = value; }

        private void AssertState(params ControlState[] expected)
        {
            if (stateTrackingEnabled)
            {
                if (expected.Length == 1)
                {
                    Assert.AreEqual(expected[0], state);
                }
                else
                {
                    CollectionAssert.Contains(expected, state);
                }
            }
        }

        public GLedApiv1_0_0Mock(bool trackState = false, ControlState initialState = ControlState.Uninitialized)
        {
            stateTrackingEnabled = trackState;
            state = initialState;
        }

        public uint Apply(int iApplyBit)
        {
            AssertState(ControlState.DoneSetLedData);

            applyCalled = true;
            lastApply = iApplyBit;
            return nextReturn;
        }

        public uint BeatInput(int iCtrl)
        {
            throw new System.NotImplementedException();
        }

        public uint GetCalibrationValue()
        {
            throw new System.NotImplementedException();
        }

        public uint GetLedLayout(byte[] bytArray, int arySize)
        {
            AssertState(ControlState.DoneGetMaxDivision);
            state = ControlState.DoneGetLedLayout;

            Assert.AreEqual(arySize, bytArray.Length, "arySize == bytArray.Length");
            for (int i = 0; i< bytArray.Length; i++)
            {
                bytArray[i] = 1; // A_LED
            }
            return nextReturn;
        }

        public int GetMaxDivision()
        {
            AssertState(ControlState.DoneInit);
            state = ControlState.DoneGetMaxDivision;
            return maxDivisions;
        }

        public int GetRGBPinType()
        {
            throw new System.NotImplementedException();
        }

        public uint GetSdkVersion(StringBuilder lpBuf, int bufSize)
        {
            Assert.IsTrue(bufSize >= 16, "bufSize >= 16");

            lpBuf.Append(version);
            return nextReturn;
        }

        public uint Get_IT8295_FwVer(byte[] bytArray, int arySize)
        {
            throw new System.NotImplementedException();
        }

        public uint InitAPI()
        {
            AssertState(ControlState.Uninitialized);
            state = ControlState.DoneInit;
            return nextReturn;
        }

        public uint IT8295_Reset()
        {
            AssertState(ControlState.DoneGetLedLayout);
            return nextReturn;
        }

        public int MonocLedCtrlSupport()
        {
            throw new System.NotImplementedException();
        }

        public uint RGBCalibration_Done(int cal_div)
        {
            throw new System.NotImplementedException();
        }

        public uint RGBCalibration_Step1(int cal_div)
        {
            throw new System.NotImplementedException();
        }

        public void RGBCalibration_Step2()
        {
            throw new System.NotImplementedException();
        }

        public void RGBCalibration_Step3()
        {
            throw new System.NotImplementedException();
        }

        public uint RGBPin_Type1(int pin1, int pin2, int pin3)
        {
            throw new System.NotImplementedException();
        }

        public uint SetLedData(byte[] bytArray, int arySize)
        {
            AssertState(ControlState.DoneGetLedLayout, ControlState.DoneSetLedData);
            state = ControlState.DoneSetLedData;

            Assert.AreEqual(arySize, bytArray.Length, "arySize == bytArray.Length");
            ledSettings = bytArray;

            return nextReturn;
        }

        public bool SetMonocLedMode(int mnoLedMode)
        {
            throw new System.NotImplementedException();
        }
    }
}
