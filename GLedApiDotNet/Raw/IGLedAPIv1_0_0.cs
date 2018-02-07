// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Text;

namespace GLedApiDotNet.Raw
{
    public interface IGLedAPIv1_0_0
    {
        uint GetSdkVersion(StringBuilder lpBuf, int bufSize);
        uint InitAPI();
        int GetMaxDivision();
        uint GetLedLayout(byte[] bytArray, int arySize);
        uint SetLedData(byte[] bytArray, int arySize);
        uint Apply(int iApplyBit);
        uint BeatInput(int iCtrl);
        uint IT8295_Reset();
        uint Get_IT8295_FwVer(byte[] bytArray, int arySize);
        uint RGBCalibration_Step1(int cal_div);
        void RGBCalibration_Step2();
        void RGBCalibration_Step3();
        uint RGBCalibration_Done(int cal_div);
        uint GetCalibrationValue();
        int MonocLedCtrlSupport();
        int GetRGBPinType();
        bool SetMonocLedMode(int mnoLedMode);
        uint RGBPin_Type1(int pin1, int pin2, int pin3);
    }
}
