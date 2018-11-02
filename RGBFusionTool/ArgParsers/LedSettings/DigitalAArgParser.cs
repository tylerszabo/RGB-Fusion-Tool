// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GLedApiDotNet.LedSettings;
using Mono.Options;
using System;
using System.Collections.Generic;

namespace RGBFusionTool.ArgParsers.LedSettings
{
    class DigitalAArgParser : LedSettingArgParser<LedSetting>
    {
        private class DigitalAParserContext : ArgParserContext
        {
            public byte MaxBrightness { get; set; }
            public byte MinBrightness { get; set; }
            public double Speed { get; set; }
            public DigitalA.Direction Direction { get; set; }

            public void Set()
            {
                Valid = true;
            }

            protected override void SetDefaults()
            {
                MaxBrightness = 100;
                MinBrightness = 0;
                Speed = 1;
                Direction = DigitalA.Direction.RightToLeft;
            }
        }

        DigitalAParserContext context;

        private DigitalAArgParser(DigitalAParserContext context) : base(context)
        {
            this.context = context;
        }

        public DigitalAArgParser() : this(new DigitalAParserContext ())
        {
            RequiredOptions = new OptionSet
            {
                { "Digital A" },
                { "digital-a", "Digital A", v => context.Set() },
            };
            ExtraOptions = new OptionSet
            {
                { "maxbrightness=", "(optional) max brightness (0-100)", (byte b) => context.MaxBrightness = b },
                { "minbrightness=", "(optional) min brightness (0-100)", (byte b) => context.MinBrightness = b },
                { "speed=", "(optional) speed ({SECONDS})", (double d) => context.Speed = d },
                { "rtl|right-to-left", "(optional) right to left", v => context.Direction = DigitalA.Direction.RightToLeft },
                { "ltr|left-to-right", "(optional) left to right", v => context.Direction = DigitalA.Direction.LeftToRight },
                { "<>", v => throw new InvalidOperationException(string.Format("Unsupported option {0}", v)) }
            };
        }

        public override LedSetting TryParse(IEnumerable<string> args)
        {
            if (!PopulateContext(args))
            {
                return null;
            }

            TimeSpan speed = TimeSpan.FromSeconds(context.Speed);

            return new DigitalA(context.MaxBrightness, context.MinBrightness, speed, context.Direction);
        }
    }
}
