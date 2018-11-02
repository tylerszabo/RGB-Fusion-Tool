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
    class OffArgParser : LedSettingArgParser<LedSetting>
    {
        private class OffArgParserContext : ArgParserContext
        {
            public void Set()
            {
                Valid = true;
            }

            protected override void SetDefaults() {}
        }

        OffArgParserContext context;

        private OffArgParser(OffArgParserContext context) : base(context)
        {
            this.context = context;
        }

        public OffArgParser() : this(new OffArgParserContext ())
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

        public override LedSetting TryParse(IEnumerable<string> args)
        {
            if (!PopulateContext(args))
            {
                return null;
            }

            return new OffLedSetting();
        }
    }
}
