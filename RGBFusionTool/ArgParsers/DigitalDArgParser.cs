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

namespace RGBFusionTool.ArgParsers
{
    class DigitalDArgParser : LedSettingArgParser
    {
        private class DigitalDParserContext : ArgParserContext
        {
            public byte MaxBrightness { get; set; }
            public byte MinBrightness { get; set; }
            public double Speed { get; set; }

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
                Speed = 1;
            }
        }

        DigitalDParserContext context;

        private DigitalDArgParser(DigitalDParserContext context) : base(context)
        {
            this.context = context;
        }

        public DigitalDArgParser() : this(new DigitalDParserContext ())
        {
            RequiredOptions = new OptionSet
            {
                { "Digital D" },
                { "digital-d=", "Digital D {COLOR}", v => context.ColorString = v },
            };
            ExtraOptions = new OptionSet
            {
                { "maxbrightness=", "(optional) max brightness (0-100)", (byte b) => context.MaxBrightness = b },
                { "minbrightness=", "(optional) min brightness (0-100)", (byte b) => context.MinBrightness = b },
                { "speed=", "(optional) speed ({SECONDS})", (double d) => context.Speed = d },
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

            return new DigitalD(GetColor(context.ColorString), context.MaxBrightness, context.MinBrightness, speed);
        }
    }
}
