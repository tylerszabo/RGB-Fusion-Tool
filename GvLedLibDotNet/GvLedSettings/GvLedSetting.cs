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

namespace GvLedLibDotNet.GvLedSettings
{
    public class GvLedSetting
    {
        public enum Modes : uint
        {
            Null = 0,
            Static = 1,
            Pulse = 2,
            Flash = 3,
            DFlash = 4,
            ColorCycle = 5
        }

        protected Modes Mode { get; set; } = Modes.Null;
        protected Color Color { get; set; } = Color.Empty;
        protected uint Speed { get; set; } = 0;

        private uint minBrightness = 0;
        private uint maxBrightness = 0;

        protected uint MaxBrightness { get => maxBrightness;
            set
            {
                if (value > 11)
                {
                    throw new ArgumentOutOfRangeException("value",  value, "must be between 0 and 11");
                }
                maxBrightness = value;
            }
        }
        protected uint MinBrightness { get => minBrightness;
            set
            {
                if (value > 11)
                {
                    throw new ArgumentOutOfRangeException("value", value, "must be between 0 and 11");
                }
                minBrightness = value;
            }
        }

        protected void SetBrightness(uint maxBrightness, uint minBrightness)
        {
            if (maxBrightness < minBrightness)
            {
                throw new ArgumentOutOfRangeException("minBrightness", minBrightness, "minBrightness must not be greater than maxBrightness");
            }
            this.MaxBrightness = maxBrightness;
            this.MinBrightness = minBrightness;
        }

        internal GvLedSetting()
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
                writer.Write((uint)Mode);
                writer.Write((uint)Speed);
                writer.Write((uint)0);
                writer.Write((uint)0);
                writer.Write((uint)0);
                writer.Write((uint)MinBrightness);
                writer.Write((uint)MaxBrightness);

                writer.Write((byte)Color.B);
                writer.Write((byte)Color.G);
                writer.Write((byte)Color.R);
                writer.Write((byte)0);

                writer.Write((uint)0);
                writer.Write((uint)1);
                writer.Write((uint)1);
            }
            return buffer.ToArray();
        }

        public GVLED_CFG ToStruct()
        {
            GVLED_CFG result;
            result.nType = (uint)Mode;
            result.nSpeed = Speed;
            result.dwTime1 = 0;
            result.dwTime2 = 0;
            result.dwTime3 = 0;
            result.nMinBrightness = MinBrightness;
            result.nMaxBrightness = MaxBrightness;
            result.dwColor = (uint)(Color.ToArgb() & 0x00FFFFFF);
            result.nAngle = 0;
            result.nOn = 1;
            result.nSync = 1;
            return result;
        }
    }
}
