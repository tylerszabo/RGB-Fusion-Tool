// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GvLedLibDotNet.GvLedSettings;

namespace GvLedLibDotNet.Raw
{
    class GvLedLibv1_0Wrapper
    {
        private IGvLedLibv1_0 rawApi;

        internal GvLedLibv1_0Wrapper(IGvLedLibv1_0 rawApi)
        {
            this.rawApi = rawApi;
        }

        public GvLedLibv1_0Wrapper() : this(new GvLedLibv1_0Impl()) {}

        private void CheckReturn(string apiFunction, uint result)
        {
            if (result != 0)
            {
                throw new GvLedLibv1_0Exception(string.Format("dllexp_{0}", apiFunction), result);
            }
        }

        public DeviceType[] Initialize()
        {
            // The following is as of 2018-08-10
            // iDeviceIdArray isn't bounds checked and iDeviceCount is overwritten without being read.
            // 0x100 is used for other buffers in GvLedLib so use that here just in case.
            // Even thought iDeviceCount isn't read set it to equal the buffer size in case behaviour is fixed.

            int[] iDeviceIdArray = new int[0x100];
            int iDeviceCount = iDeviceIdArray.Length;

            CheckReturn("GvLedInitial", rawApi.GvLedInitial(out iDeviceCount, iDeviceIdArray));

            if (iDeviceCount < 0)
            {
                throw new GvLedLibv1_0Exception("GvLedInitial", string.Format("iDeviceCount < 0 (was {0})", iDeviceCount));
            }

            if (iDeviceCount > iDeviceIdArray.Length)
            {
                throw new GvLedLibv1_0Exception("GvLedInitial", string.Format("iDeviceCount > {0} (was {1})", iDeviceIdArray.Length, iDeviceCount));
            }

            DeviceType[] result = new DeviceType[iDeviceCount];

            for(int curDevice = 0; curDevice < iDeviceCount; curDevice++)
            {
                result[curDevice] = (DeviceType)iDeviceIdArray[curDevice];
            }

            return result;
        }

        public void Save(GvLedSetting config)
        {
            LedSave(-1, config);
        }

        public void LedSave(int nIndex, GvLedSetting config)
        {
            CheckReturn("GvLedSave", rawApi.GvLedSave(nIndex, config.ToStruct()));
        }
    }
}
