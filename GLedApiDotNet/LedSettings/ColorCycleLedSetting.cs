// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;

namespace GLedApiDotNet.LedSettings
{
    public class ColorCycleLedSetting : LedSetting
    {
        public ColorCycleLedSetting(byte maxBrightness, byte minBrightness, ushort transitionTime, byte numColors = 7, bool pulse = false)
        {
            this.Mode = Modes.ColorCycle;

            this.SetBrightness(maxBrightness, minBrightness);

            this.Time0 = transitionTime;

            if (numColors < 1 || numColors > 7)
            {
                throw new ArgumentOutOfRangeException("numColors", numColors, "must be between 1 and 7");
            }
            this.CtrlValue0 = numColors;

            this.CtrlValue1 = (byte)(pulse ? 1 : 0);
        }

        public ColorCycleLedSetting(byte maxBrightness, byte minBrightness, TimeSpan transitionTime, byte numColors = 7, bool pulse = false)
            : this(maxBrightness, minBrightness, 0, numColors, pulse)
        {
            // Undocumented behavior - each transition takes 10 seconds
            transitionTime = TimeSpan.FromMilliseconds(transitionTime.TotalMilliseconds / 10);

            this.SetTime0(transitionTime);
        }

        public override string ToString()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(((double)Time0) * 10);
            return string.Format("Color Cycle: Brightness={0}, Transition time={1}s", MaxBrightness, t.TotalSeconds, CtrlValue1 == 1 ? ", Pulse" : "");
        }
    }
}
