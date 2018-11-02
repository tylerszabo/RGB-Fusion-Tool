// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GvLedLibDotNet.GvLedSettings;
using Mono.Options;
using System;
using System.Collections.Generic;

namespace RGBFusionTool.ArgParsers.GvLedSettings
{
    class StaticColorGvArgParser : LedSettingArgParser<GvLedSetting>
    {
        private class StaticColorArgParserContext : ArgParserContext
        {
            public byte Brightness { get; set; }

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
                Brightness = 10;
            }
        }

        StaticColorArgParserContext context;

        private StaticColorGvArgParser(StaticColorArgParserContext context) : base(context)
        {
            this.context = context;
        }

        public StaticColorGvArgParser() : this(new StaticColorArgParserContext ())
        {
            RequiredOptions = new OptionSet
            {
                { "Static color" },
                { "c|color|static=", "set static color to {COLOR}", v => context.ColorString = v },
            };
            ExtraOptions = new OptionSet
            {
                { "b|brightness=", "(optional) brightness (0-100)", (byte b) => context.Brightness = b },
                { "<>", v => throw new InvalidOperationException(string.Format("Unsupported option {0}", v)) }
            };
        }

        public override GvLedSetting TryParse(IEnumerable<string> args)
        {
            if (!PopulateContext(args))
            {
                return null;
            }

            return new StaticGvLedSetting(GetColor(context.ColorString), context.Brightness);
        }
    }
}
