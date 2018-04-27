// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using GLedApiDotNet.LedSettings;
using Mono.Options;

namespace RGBFusionTool
{
    abstract class LedSettingArgParser
    {
        public OptionSet RequiredOptions { get; protected set; }
        public OptionSet ExtraOptions { get; protected set; }

        protected abstract class ArgParserContext
        {
            public ArgParserContext()
            {
                Reset();
            }

            public bool Valid { get; protected set; }

            public void Reset()
            {
                Valid = false;
                SetDefaults();
            }

            protected abstract void SetDefaults();
        }

        private ArgParserContext context;

        protected LedSettingArgParser(ArgParserContext context)
        {
            this.context = context;
        }

        protected bool PopulateContext(IEnumerable<string> args)
        {
            context.Reset();

            List<string> extra = RequiredOptions.Parse(args);
            if (!context.Valid)
            {
                return false;
            }
            ExtraOptions.Parse(extra);

            return true;
        }

        public abstract LedSetting TryParse(IEnumerable<string> args);

        protected static Color GetColor(string input)
        {
            Color realColor = Color.FromName(input.Trim());
            if (realColor.A == 0)
            {
                realColor = Color.FromArgb(0xff, Color.FromArgb(Int32.Parse(input, System.Globalization.NumberStyles.HexNumber)));
            }
            return realColor;
        }
    }
}