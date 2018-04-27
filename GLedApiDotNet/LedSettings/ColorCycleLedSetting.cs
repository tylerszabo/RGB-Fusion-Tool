// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace GLedApiDotNet.LedSettings
{
    public class ColorCycleLedSetting : LedSetting
    {
        private static readonly string[] COLORNAMES = { "Red", "Orange", "Yellow", "Green", "Blue", "Indigo", "Violet" };

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
            this.TimeSpan0 = transitionTime;
        }

        public TimeSpan TransitionTime => TimeSpan0;
        public bool Pulse => (CtrlValue1 == 1);
        public byte NumColors => CtrlValue0;

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("Color Cycle: Brightness={0}, Transition time={1}s", MaxBrightness, TransitionTime.TotalSeconds);
            if (Pulse)
            {
                sb.AppendFormat(", Pulse, MinBrightness={0}", MinBrightness);
            }
            if (NumColors != 7)
            {
                sb.AppendFormat(", Colors={0}", string.Join(",", COLORNAMES, 0, NumColors));
            }
            return sb.ToString();
        }
    }
}
