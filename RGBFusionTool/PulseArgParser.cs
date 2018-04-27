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

namespace RGBFusionTool
{
    class PulseArgParser : LedSettingArgParser
    {
        private class PulseArgParserContext : ArgParserContext
        {
            public byte MaxBrightness { get; set; }
            public byte MinBrightness { get; set; }
            public double FadeOn { get; set; }
            public double FadeOff { get; set; }

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
                FadeOn = 8;
                FadeOff = 4;
            }
        }

        PulseArgParserContext context;

        private PulseArgParser(PulseArgParserContext context) : base(context)
        {
            this.context = context;
        }

        public PulseArgParser() : this(new PulseArgParserContext ())
        {
            RequiredOptions = new OptionSet
            {
                { "Pulse" },
                { "pulse=", "pulse color {COLOR}", v => context.ColorString = v },
            };
            ExtraOptions = new OptionSet
            {
                { "maxbrightness=", "(optional) max brightness (0-100)", (byte b) => context.MaxBrightness = b },
                { "minbrightness=", "(optional) min brightness (0-100)", (byte b) => context.MinBrightness = b },
                { "fadeon=", "(optional) fade on time ({SECONDS})", (double d) => context.FadeOn = d },
                { "fadeoff=", "(optional) fade off time ({SECONDS})", (double d) => context.FadeOff = d },
                { "<>", v => throw new InvalidOperationException("Unsupported option") }
            };
        }

        public override LedSetting TryParse(IEnumerable<string> args)
        {
            if (!PopulateContext(args))
            {
                return null;
            }

            TimeSpan fadeOn = TimeSpan.FromSeconds(context.FadeOn);
            TimeSpan fadeOff = TimeSpan.FromSeconds(context.FadeOff);

            return new PulseLedSetting(GetColor(context.ColorString), context.MaxBrightness, context.MinBrightness, fadeOn, fadeOff);
        }
    }
}
