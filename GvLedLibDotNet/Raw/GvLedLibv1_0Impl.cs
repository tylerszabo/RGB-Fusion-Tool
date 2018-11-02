// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Runtime.InteropServices;

namespace GvLedLibDotNet.Raw
{
    public class GvLedLibv1_0Impl : IGvLedLibv1_0
    {
        [DllImport("GvLedLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint dllexp_GvLedInitial(out int iDeviceCount, [In, Out] int[] iDeviceIdArray);
        public uint GvLedInitial(out int iDeviceCount, int[] iDeviceIdArray) => dllexp_GvLedInitial(out iDeviceCount, iDeviceIdArray);

        [DllImport("GvLedLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint dllexp_GvLedGetVersion(out int iMajorVersion, out int iMinorVersion);
        public uint GvLedGetVersion(out int iMajorVersion, out int iMinorVersion) => dllexp_GvLedGetVersion(out iMajorVersion, out iMinorVersion);

        [DllImport("GvLedLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint dllexp_GvLedSave(int nIndex, GVLED_CFG config);
        public uint GvLedSave(int nIndex, GVLED_CFG config) => dllexp_GvLedSave(nIndex, config);

        [DllImport("GvLedLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint dllexp_GvLedSet(int nIndex, GVLED_CFG config);
        public uint GvLedSet(int nIndex, GVLED_CFG config) => dllexp_GvLedSet(nIndex, config);

        [DllImport("GvLedLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint dllexp_GvLedGetVgaModelName(out byte[] pVgaModelName);
        public uint GvLedGetVgaModelName(out byte[] pVgaModelName) => dllexp_GvLedGetVgaModelName(out pVgaModelName);
    }
}
