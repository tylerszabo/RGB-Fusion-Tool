// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GLedApiDotNet;
using GLedApiDotNet.LedSettings;
using System;
using System.IO;
using System.Drawing;
using Mono.Options;

namespace RGBFusionTool
{
    public class Application
    {
        IRGBFusionMotherboard motherboardLEDs;
        TextWriter stdout;
        TextWriter stderr;

        public Application(IRGBFusionMotherboard motherboardLEDs, TextWriter stdout, TextWriter stderr)
        {
            this.motherboardLEDs = motherboardLEDs;
            this.stdout = stdout;
            this.stderr = stderr;
        }

        static void ShowHelp(OptionSet options, TextWriter o)
        {
            o.WriteLine(string.Format("Usage: {0} [OPTION]...\nSet RGB Fusion motherboard LEDs\n\nOptions:", AppDomain.CurrentDomain.FriendlyName));
            options.WriteOptionDescriptions(o);
        }

        public void Main(string[] args)
        {
            int opt_Verbose = 0;
            string opt_Color = null;
            string opt_ColorCycle = null;
            bool flag_DoCycle = false;
            bool flag_Help = false;
            string opt_Brightness = null;

            OptionSet options = new OptionSet
            {
                {"v|verbose", v => opt_Verbose++ },

                {"c|color|static=", "set static color", v => opt_Color = v },
                {"cycle|colorcycle:", "cycle colors, changing color every {SECONDS}", v => { flag_DoCycle = true; opt_ColorCycle = v; } },
                {"b|brightness=", "brightness (0-100)", v => opt_Brightness = v },

                {"?|h|help", "show help and exit", v => flag_Help = true },

                {"<>", v => { throw new OptionException(string.Format("Unexpected option \"{0}\"", v),"default"); } }
            };

            try
            {
                byte brightness = 100;
                LedSetting setting = null;

                options.Parse(args);

                if (flag_Help)
                {
                    ShowHelp(options, stdout);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(opt_Brightness))
                {
                    brightness = byte.Parse(opt_Brightness);
                }

                if (flag_DoCycle)
                {
                    TimeSpan cycleTime = TimeSpan.FromSeconds(1);
                    if (!string.IsNullOrWhiteSpace(opt_ColorCycle))
                    {
                        cycleTime = TimeSpan.FromSeconds(Double.Parse(opt_ColorCycle));
                    }
                    if (opt_Verbose > 0) { stdout.WriteLine("Color cycle, rotating every {0} seconds", cycleTime.TotalSeconds); }
                    setting = new ColorCycleLedSetting(brightness, 0, cycleTime);
                }
                else if (!string.IsNullOrEmpty(opt_Color))
                {
                    Color realColor = Color.FromName(opt_Color.Trim());
                    if (realColor.A == 0)
                    {
                        realColor = Color.FromArgb(0xff, Color.FromArgb(Int32.Parse(opt_Color, System.Globalization.NumberStyles.HexNumber)));
                    }
                    if (opt_Verbose > 0) { stdout.WriteLine("Static color: {0}", realColor.ToString()); }
                    setting = new StaticLedSetting(realColor, brightness);
                }

                if (setting != null)
                {
                    motherboardLEDs.SetAll(setting);
                }
            }
            catch (Exception e)
            {
                ShowHelp(options, stderr);
                stderr.WriteLine("Error: {0}", e.Message);
                throw;
            }
            return;
        }
    }
}
