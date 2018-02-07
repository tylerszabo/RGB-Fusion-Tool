// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Runtime.InteropServices;
using System.Text;

namespace GLedApiDotNet.Raw
{
    public class GLedAPIv1_0_0Impl : IGLedAPIv1_0_0
    {
        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_GetSdkVersion", CharSet = CharSet.Unicode)]
        public static extern uint dllexp_GetSdkVersion(StringBuilder lpBuf, int bufSize);
        public uint GetSdkVersion(StringBuilder lpBuf, int bufSize) { return dllexp_GetSdkVersion(lpBuf, bufSize); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_InitAPI")]
        public static extern uint dllexp_InitAPI();
        public uint InitAPI() { return dllexp_InitAPI(); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_GetMaxDivision")]
        public static extern int dllexp_GetMaxDivision();
        public int GetMaxDivision() { return dllexp_GetMaxDivision(); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_GetLedLayout")]
        public static extern uint dllexp_GetLedLayout(byte[] bytArray, int arySize);
        public uint GetLedLayout(byte[] bytArray, int arySize) { return dllexp_GetLedLayout(bytArray, arySize); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_SetLedData")]
        public static extern uint dllexp_SetLedData(byte[] bytArray, int arySize);
        public uint SetLedData(byte[] bytArray, int arySize) { return dllexp_SetLedData(bytArray, arySize); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_Apply")]
        public static extern uint dllexp_Apply(int iApplyBit);
        public uint Apply(int iApplyBit) { return dllexp_Apply(iApplyBit); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_BeatInput")]
        public static extern uint dllexp_BeatInput(int iCtrl);
        public uint BeatInput(int iCtrl) { return dllexp_BeatInput(iCtrl); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_IT8295_Reset")]
        public static extern uint dllexp_IT8295_Reset();
        public uint IT8295_Reset() { return dllexp_IT8295_Reset(); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_Get_IT8295_FwVer")]
        public static extern uint dllexp_Get_IT8295_FwVer(byte[] bytArray, int arySize);
        public uint Get_IT8295_FwVer(byte[] bytArray, int arySize) { return dllexp_Get_IT8295_FwVer(bytArray, arySize); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_RGBCalibration_Step1")]
        public static extern uint dllexp_RGBCalibration_Step1(int cal_div);
        public uint RGBCalibration_Step1(int cal_div) { return dllexp_RGBCalibration_Step1(cal_div); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_RGBCalibration_Step2")]
        public static extern void dllexp_RGBCalibration_Step2();
        public void RGBCalibration_Step2() { dllexp_RGBCalibration_Step2(); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_RGBCalibration_Step3")]
        public static extern void dllexp_RGBCalibration_Step3();
        public void RGBCalibration_Step3() { dllexp_RGBCalibration_Step3(); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_RGBCalibration_Done")]
        public static extern uint dllexp_RGBCalibration_Done(int cal_div);
        public uint RGBCalibration_Done(int cal_div) { return dllexp_RGBCalibration_Done(cal_div); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_GetCalibrationValue")]
        public static extern uint dllexp_GetCalibrationValue();
        public uint GetCalibrationValue() { return dllexp_GetCalibrationValue(); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_MonocLedCtrlSupport")]
        public static extern int dllexp_MonocLedCtrlSupport();
        public int MonocLedCtrlSupport() { return dllexp_MonocLedCtrlSupport(); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_GetRGBPinType")]
        public static extern int dllexp_GetRGBPinType();
        public int GetRGBPinType() { return dllexp_GetRGBPinType(); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_SetMonocLedMode")]
        public static extern bool dllexp_SetMonocLedMode(int mnoLedMode);
        public bool SetMonocLedMode(int mnoLedMode) { return dllexp_SetMonocLedMode(mnoLedMode); }

        [DllImport("GLedApi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "dllexp_RGBPin_Type1")]
        public static extern uint dllexp_RGBPin_Type1(int pin1, int pin2, int pin3);
        public uint RGBPin_Type1(int pin1, int pin2, int pin3) { return dllexp_RGBPin_Type1(pin1, pin2, pin3); }

        // Undocumented

        // dllexp_Apply2
        // dllexp_IT8295_Reset2
        // dllexp_SetLedData2
        // dllexp_SoftwareBeatModeSupport
    }
}