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
    public class DigitalE : LedSetting
    {
        public DigitalE(Color color, byte maxBrightness, byte minBrightness, ushort cycleTime)
        {
            this.Mode = Modes.DigitalModeE;

            this.SetBrightness(maxBrightness, minBrightness);

            this.Time0 = cycleTime;
        }

        public DigitalE(Color color, byte maxBrightness, byte minBrightness, TimeSpan cycleTime)
            : this(color, maxBrightness, minBrightness, 0)
        {
            this.TimeSpan0 = cycleTime;
        }

        protected TimeSpan CycleTime => TimeSpan0;

        public override string ToString()
        {
            return string.Format("DigitalE: Color={0}, MaxBrightness={1}, MinBrightness={2}, CycleTime={3}s", Color, MaxBrightness, MinBrightness, CycleTime.TotalSeconds);
        }
    }
}
