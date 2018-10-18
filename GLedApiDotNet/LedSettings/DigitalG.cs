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
    public class DigitalG : LedSetting
    {
        public DigitalG(Color color, byte maxBrightness, byte minBrightness, ushort stepInterval, byte dimSpeed)
        {
            this.Mode = Modes.DigitalModeG;

            this.Color = color;

            this.SetBrightness(maxBrightness, minBrightness);

            this.Time0 = stepInterval;

            if (dimSpeed < 0 || dimSpeed > 100)
            {
                throw new ArgumentOutOfRangeException("dimSpeed", dimSpeed, "must be between 0 and 100");
            }

            this.CtrlValue0 = dimSpeed;
        }

        public DigitalG(Color color, byte maxBrightness, byte minBrightness, TimeSpan stepInterval, byte dimSpeed)
            : this(color, maxBrightness, minBrightness, 0, dimSpeed)
        {
            this.TimeSpan0 = stepInterval;
        }

        protected TimeSpan StepInterval => TimeSpan0;

        protected byte DimSpeed => CtrlValue0;

        public override string ToString()
        {
            return string.Format("DigitalG: Color={0}, MaxBrightness={1}, MinBrightness={2}, StepInterval={3}s, DimSpeed={4}", Color, MaxBrightness, MinBrightness, StepInterval.TotalSeconds, DimSpeed);
        }
    }
}
