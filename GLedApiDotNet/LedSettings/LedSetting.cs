// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using System.IO;

namespace GLedApiDotNet.LedSettings
{
    public class LedSetting
    {
        public enum Modes : byte
        {
            Null = 0,
            Pulse = 1,
            Music = 2,
            ColorCycle = 3,
            Static = 4,
            Flash = 5,
            Transition = 8,
            DigitalModeA = 10,
            DigitalModeB = 11,
            DigitalModeC = 12,
            DigitalModeD = 13,
            DigitalModeE = 14,
            DigitalModeF = 15,
            DigitalModeG = 16,
            DigitalModeH = 17,
            DigitalModeI = 18
        }

        protected Modes Mode { get; set; } = Modes.Null;

        private byte maxBrightness = 0;
        private byte minBrightness = 0;

        protected byte White { get; set; } = 0;
        protected Color Color { get; set; } = Color.Empty;

        protected ushort Time0 { get; set; } = 0;
        protected ushort Time1 { get; set; } = 0;
        protected ushort Time2 { get; set; } = 0;

        protected byte CtrlValue0 { get; set; } = 0;
        protected byte CtrlValue1 { get; set; } = 0;

        protected byte MaxBrightness { get => maxBrightness;
            set
            {
                if (value > 100)
                {
                    throw new ArgumentOutOfRangeException("value",  value, "must be between 0 and 100");
                }
                maxBrightness = value;
            }
        }
        protected byte MinBrightness { get => minBrightness;
            set
            {
                if (value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", value, "must be between 0 and 100");
                }
                minBrightness = value;
            }
        }

        protected void SetBrightness(byte maxBrightness, byte minBrightness)
        {
            if (maxBrightness < minBrightness)
            {
                throw new ArgumentOutOfRangeException("minBrightness", minBrightness, "minBrightness must not be greater than maxBrightness");
            }
            this.MaxBrightness = maxBrightness;
            this.MinBrightness = minBrightness;
        }

        // Undocumented behavior - API time values are 1/100ths of seconds, not milliseconds
        private static ushort SetTime(TimeSpan value)
        {
            if (!(value.TotalMilliseconds >= 0 && value.TotalMilliseconds <= 655350))
            {
                throw new ArgumentOutOfRangeException("value", value, "must be between 0 and 655350 milliseconds");
            }
            TimeSpan realValue = TimeSpan.FromMilliseconds(value.TotalMilliseconds / 10.0);
            return (ushort)realValue.TotalMilliseconds;
        }

        // Undocumented behavior - API time values are 1/100ths of seconds, not milliseconds
        private static TimeSpan GetTime(ushort value) => TimeSpan.FromMilliseconds(((double)value) * 10.0);

        protected TimeSpan TimeSpan0 { get => GetTime(Time0); set => Time0 = SetTime(value); }
        protected TimeSpan TimeSpan1 { get => GetTime(Time1); set => Time1 = SetTime(value); }
        protected TimeSpan TimeSpan2 { get => GetTime(Time2); set => Time2 = SetTime(value); }

        internal LedSetting()
        {
        }

        public byte[] ToByteArray()
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new NotSupportedException("Only little endian is supported.");
            }

            MemoryStream buffer = new MemoryStream(new Byte[16]);
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                writer.Write((byte)0); // reserved

                writer.Write((byte)Mode);

                writer.Write((byte)MaxBrightness);
                writer.Write((byte)MinBrightness);

                writer.Write((byte)Color.B);
                writer.Write((byte)Color.G);
                writer.Write((byte)Color.R);
                writer.Write((byte)White);

                writer.Write((ushort)Time0);
                // ...

                writer.Write((ushort)Time1);
                // ...

                writer.Write((ushort)Time2);
                // ...

                writer.Write((byte)CtrlValue0);
                writer.Write((byte)CtrlValue1);
            }
            return buffer.ToArray();
        }
    }
}
