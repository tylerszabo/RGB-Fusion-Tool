// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GLedApiDotNet.LedSettings;
using System;
using Mono.Options;
using System.Collections.Generic;

namespace RGBFusionTool
{
    class ColorCycleArgParser : LedSettingArgParser
    {
        private class ColorCycleArgParserContext : ArgParserContext
        {
            public byte Brightness { get; set; }

            private double? seconds;
            public double? Seconds
            {
                get => seconds; set
                {
                    Valid = true;
                    seconds = value;
                }
            }

            protected override void SetDefaults()
            {
                seconds = null;
                Brightness = 100;
            }
        }

        ColorCycleArgParserContext context;
        
        private ColorCycleArgParser(ColorCycleArgParserContext context) : base(context)
        {
            this.context = context;
        }

        public ColorCycleArgParser() : this (new ColorCycleArgParserContext())
        {
            RequiredOptions = new OptionSet
            {
                { "Color cycle" },
                { "cycle|colorcycle:", "cycle colors, changing color every {SECONDS}", (double? d) => context.Seconds = d },
            };
            ExtraOptions = new OptionSet
            {
                { "b|brightness=", "(optional) brightness (0-100)", (byte b) => context.Brightness = b },
                { "<>", v => throw new InvalidOperationException("Unsupported option") }
            };
        }

        public override LedSetting TryParse(IEnumerable<string> args)
        {
            if (!PopulateContext(args))
            {
                return null;
            }

            TimeSpan cycleTime = TimeSpan.FromSeconds(context.Seconds ?? 1);

            return new ColorCycleLedSetting(context.Brightness, 0, cycleTime);
        }
    }
}
