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
    class OffGvArgParser : LedSettingArgParser<GvLedSetting>
    {
        private class GvOffArgParserContext : ArgParserContext
        {
            public void Set()
            {
                Valid = true;
            }

            protected override void SetDefaults() {}
        }

        GvOffArgParserContext context;

        private OffGvArgParser(GvOffArgParserContext context) : base(context)
        {
            this.context = context;
        }

        public OffGvArgParser() : this(new GvOffArgParserContext ())
        {
            RequiredOptions = new OptionSet
            {
                { "Off" },
                { "off", "turn off", v => context.Set() },
            };
            ExtraOptions = new OptionSet
            {
                { "<>", v => throw new InvalidOperationException(string.Format("Unsupported option {0}", v)) }
            };
        }

        public override GvLedSetting TryParse(IEnumerable<string> args)
        {
            if (!PopulateContext(args))
            {
                return null;
            }

            return new OffGvLedSetting();
        }
    }
}
