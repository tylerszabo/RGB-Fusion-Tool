// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GLedApiDotNet.LedSettings;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GLedApiDotNet.Raw
{
    public class GLedAPIv1_0_0Wrapper
    {
        private IGLedAPIv1_0_0 rawApi;

        internal GLedAPIv1_0_0Wrapper(IGLedAPIv1_0_0 rawApi)
        {
            this.rawApi = rawApi;
        }

        public GLedAPIv1_0_0Wrapper() : this(new GLedAPIv1_0_0Impl()) {}

        private void CheckReturn(string apiFunction, uint result)
        {
            if (result != 0)
            {
                throw new GLedAPIv1_0_0Exception(string.Format("dllexp_{0}", apiFunction), result);
            }
        }

        public string GetSdkVersion()
        {
            int length = 16; // API requires at least 16
            StringBuilder buffer = new StringBuilder(length);
            CheckReturn("GetSdkVersion", rawApi.GetSdkVersion(buffer, length));
            return buffer.ToString();
        }

        public void Initialize()
        {
            CheckReturn("InitAPI", rawApi.InitAPI());
        }

        public int GetMaxDivision()
        {
            int result = rawApi.GetMaxDivision();
            if (result < 0)
            {
                throw new GLedAPIv1_0_0Exception("dllexp_GetMaxDivision", unchecked ((uint)result));
            }
            return result;
        }

        public byte[] GetLedLayout(int maxDivs)
        {
            byte[] arr = new byte[maxDivs];
            CheckReturn("GetLedLayout", rawApi.GetLedLayout(arr, maxDivs));
            return arr;
        }

        public void SetLedData(IEnumerable<LedSetting> settings)
        {
            MemoryStream buffer = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                foreach (LedSetting setting in settings)
                {
                    writer.Write(setting.ToByteArray());
                }
            }
            byte[] ledData = buffer.ToArray();
            CheckReturn("SetLedData", rawApi.SetLedData(ledData, ledData.Length));
        }

        public void Apply(int divisions)
        {
            CheckReturn("Apply", rawApi.Apply(divisions));
        }

        public void IT8295_Reset()
        {
            CheckReturn("IT8295_Reset", rawApi.IT8295_Reset());
        }
    }
}