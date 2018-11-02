// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GvLedLibDotNet
{
    public enum DeviceType : int
    {
        VGA                 = 0x1001,

        Keyboard_XK700      = 0x2001,
        Keyboard_AORUS_K7   = 0x2002,
        Keyboard_AORUS_K9   = 0x2003,

        Mouse_XM300         = 0x3001,
        Mouse_AORUS_M3      = 0x3002,

        VGA_BOX_1070IXEB    = 0x4001,
        VGA_BOX_1080IXEB    = 0x4002,
        Case_XTC700         = 0x4003,
        Case_XC300W         = 0x4004,
        Case_XC700W         = 0x4005,
        Earphone_XH300      = 0x4006,
        Earphone_AORUS_H5   = 0x4007,
        Case_AC300W         = 0x4008,
        CPU_Cooler_ATC700   = 0x4009,
        Mouse_Pad_AORUS_P7  = 0x400A
    }
}
