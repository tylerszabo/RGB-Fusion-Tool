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
    public class FlashLedSetting : LedSetting
    {
        private void CheckRanges()
        {
            // NB: use a type that's larger than ushort here to avoid overflow errors

            ulong count = this.CtrlValue0;

            ulong lightOnOffTime = this.Time0;
            ulong intervalTime = this.Time1;
            ulong cycleTime = this.Time2;

            if (!(lightOnOffTime < intervalTime))
            {
                throw new ArgumentOutOfRangeException("intervalTime", intervalTime, "condition (lightOnOffTime < intervalTime) must be met");
            }

            if (!(count * intervalTime <= cycleTime))
            {
                throw new ArgumentOutOfRangeException("cycleTime", cycleTime, "condition (count * intervalTime <= cycleTime) must be met");
            }
        }

        private FlashLedSetting(Color color, byte maxBrightness, byte minBrightness, byte count)
        {
            this.Mode = Modes.Flash;
            this.Color = color;

            this.SetBrightness(maxBrightness, minBrightness);

            if (count < 1 || count > 255)
            {
                throw new ArgumentOutOfRangeException("count", count, "must be between 1 and 255");
            }

            this.CtrlValue0 = count;
        }

        public FlashLedSetting(Color color, byte maxBrightness, byte minBrightness, ushort lightOnOffTime, ushort intervalTime, ushort cycleTime, byte count)
            : this(color, maxBrightness, minBrightness, count)
        {
            this.Time0 = lightOnOffTime;
            this.Time1 = intervalTime;
            this.Time2 = cycleTime;

            CheckRanges();
        }

        public FlashLedSetting(Color color, byte maxBrightness, byte minBrightness, TimeSpan lightOnOffTime, TimeSpan intervalTime, TimeSpan cycleTime, byte count)
            : this(color, maxBrightness, minBrightness, count)
        {
            this.TimeSpan0 = lightOnOffTime;
            this.TimeSpan1 = intervalTime;
            this.TimeSpan2 = cycleTime;

            CheckRanges();
        }

        protected TimeSpan OnOffTime => TimeSpan0;
        protected TimeSpan IntervalTime => TimeSpan1;
        protected TimeSpan CycleTime => TimeSpan2;
        protected byte Count => CtrlValue0;

        public override string ToString()
        {
            return string.Format("Flash: Color={0}, MaxBrightness={1}, MinBrightness={2}, OnOffTime={3}s, IntervalTime={4}s, CycleTime={5}s, Count={6}",
                Color, MaxBrightness, MinBrightness, OnOffTime.TotalSeconds, IntervalTime.TotalSeconds, CycleTime.TotalSeconds, Count);
        }
    }
}
