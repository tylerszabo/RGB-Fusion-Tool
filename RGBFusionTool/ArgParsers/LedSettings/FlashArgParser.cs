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
    class FlashArgParser : LedSettingArgParser<LedSetting>
    {
        private class FlashArgParserContext : ArgParserContext
        {
            public byte MaxBrightness { get; set; }
            public byte MinBrightness { get; set; }
            public double OnOff { get; set; }
            public double Interval { get; set; }
            public double Cycle { get; set; }
            public byte Count { get; set; }

            private string colorString;
            public string ColorString
            {
                get => colorString; set
                {
                    Valid = true;
                    colorString = value;
                }
            }

            protected override void SetDefaults()
            {
                colorString = null;
                MaxBrightness = 100;
                MinBrightness = 0;
                OnOff = 0.25;
                Interval = 1;
                Cycle = 5;
                Count = 3;
            }
        }

        FlashArgParserContext context;

        private FlashArgParser(FlashArgParserContext context) : base(context)
        {
            this.context = context;
        }

        public FlashArgParser() : this(new FlashArgParserContext ())
        {
            RequiredOptions = new OptionSet
            {
                { "Flash" },
                { "flash=", "flash color {COLOR}", v => context.ColorString = v },
            };
            ExtraOptions = new OptionSet
            {
                { "maxbrightness=", "(optional) max brightness (0-100)", (byte b) => context.MaxBrightness = b },
                { "minbrightness=", "(optional) min brightness (0-100)", (byte b) => context.MinBrightness = b },
                { "time=", "(optional) {SECONDS} to flash for", (double d) => context.OnOff = d },
                { "interval=", "(optional) {SECONDS} in a flash interval", (double d) => context.Interval = d },
                { "flashcycle=", "(optional) {SECONDS} in a cycle", (double d) => context.Cycle = d },
                { "count=", "(optional) flash {COUNT} intervals in a cycle", (byte b) => context.Count = b },
                { "<>", v => throw new InvalidOperationException(string.Format("Unsupported option {0}", v)) }
            };
        }

        public override LedSetting TryParse(IEnumerable<string> args)
        {
            if (!PopulateContext(args))
            {
                return null;
            }

            TimeSpan onoff = TimeSpan.FromSeconds(context.OnOff);
            TimeSpan interval = TimeSpan.FromSeconds(context.Interval);
            TimeSpan cycle = TimeSpan.FromSeconds(context.Cycle);

            return new FlashLedSetting(GetColor(context.ColorString), context.MaxBrightness, context.MinBrightness, onoff, interval, cycle, context.Count);
        }
    }
}
