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
    public class DigitalA : LedSetting
    {
        public enum Direction : byte
        {
            RightToLeft = 0,
            LeftToRight = 1
        }

        public DigitalA(byte maxBrightness, byte minBrightness, ushort speed, Direction direction = Direction.RightToLeft)
        {
            this.Mode = Modes.DigitalModeA;

            this.SetBrightness(maxBrightness, minBrightness);

            this.Time0 = speed;

            this.CtrlValue0 = (byte)direction;
        }

        public DigitalA(byte maxBrightness, byte minBrightness, TimeSpan speed, Direction direction = Direction.RightToLeft)
            : this(maxBrightness, minBrightness, 0, direction)
        {
            this.TimeSpan0 = speed;
        }

        protected TimeSpan Speed => TimeSpan0;
        protected Direction _Direction => (Direction)CtrlValue0;

        public override string ToString()
        {
            return string.Format("DigitalA: MaxBrightness={0}, MinBrightness={1}, Speed={2}s, Direction={3}", Color, MaxBrightness, MinBrightness, Speed.TotalSeconds, _Direction);
        }
    }
}
