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
using Mono.Options;
using System.Collections.Generic;
using RGBFusionTool.ArgParsers;

namespace RGBFusionTool
{
    public class Application
    {
        private IRGBFusionMotherboard motherboardLEDs;
        private TextWriter stdout;
        private TextWriter stderr;

        private class ApplicationContext
        {
            public bool flag_Help;
            public bool flag_List;
            public bool flag_Version;
            public int verbosity;

            public ApplicationContext()
            {
                SetDefaults();
            }

            public void SetDefaults()
            {
                flag_Help = false;
                flag_List = false;
                flag_Version = false;
                verbosity = 0;
            }
        }
        ApplicationContext context;

        private Dictionary<int, List<string>> zones = new Dictionary<int, List<string>>();
        private List<string> currentZone;
        private List<string> defaultZone;

        OptionSet genericOptions;
        OptionSet zoneOptions;
        List<LedSettingArgParser> ledSettingArgParsers;

        List<OptionSet> helpOptionSets;

        public Application(IRGBFusionMotherboard motherboardLEDs, TextWriter stdout, TextWriter stderr)
        {
            this.motherboardLEDs = motherboardLEDs;
            this.stdout = stdout;
            this.stderr = stderr;

            context = new ApplicationContext();

            defaultZone = new List<string>();
            currentZone = defaultZone;

            genericOptions = new OptionSet
            {
                { string.Format("Usage: {0} [OPTION]... [[LEDSETTING] | [ZONE LEDSETTING]...]", AppDomain.CurrentDomain.FriendlyName) },
                { "Set RGB Fusion motherboard LEDs" },
                { "" },
                { "Options:" },
                { "v|verbose", v => context.verbosity++ },
                { "l|list", "list zones", v => context.flag_List = true },
                { "?|h|help", "show help and exit", v => context.flag_Help = true },
                { "version", "show version information and exit", v => context.flag_Version = true },
                { "" }
            };

            zoneOptions = new OptionSet
            {
                { "ZONE:" },
                { "z|zone=", "set zone", (int zone) => {
                    if (zone < 0)
                    {
                        throw new InvalidOperationException("Zones must be positive integers");
                    }
                    if (zones.ContainsKey(zone))
                    {
                        throw new InvalidOperationException(string.Format("Zone {0} already specified", zone));
                    }
                    if (zone >= motherboardLEDs.Layout.Length)
                    {
                        throw new InvalidOperationException(string.Format("Zone is {0}, max supported is {1}", zone, motherboardLEDs.Layout.Length));
                    }
                    currentZone = new List<string>();
                    zones.Add(zone, currentZone);
                } },
                { "<>", v => currentZone.Add(v) },
            };

            ledSettingArgParsers = new List<LedSettingArgParser>
            {
                new StaticColorArgParser(),
                new ColorCycleArgParser(),
                new PulseArgParser(),
                new FlashArgParser(),
                new DigitalAArgParser(),
                new DigitalBArgParser(),
                new DigitalCArgParser(),
                new DigitalDArgParser(),
                new OffArgParser(),
            };

            helpOptionSets = new List<OptionSet>
            {
                genericOptions,
                zoneOptions,
                new OptionSet { "" },
                new OptionSet { "LEDSETTING options:" }
            };
            foreach (LedSettingArgParser argParser in ledSettingArgParsers)
            {
                helpOptionSets.Add(new OptionSet { "" });
                helpOptionSets.Add(argParser.RequiredOptions);
                helpOptionSets.Add(argParser.ExtraOptions);
            }
        }

        private void ShowHelp(TextWriter o)
        {
            foreach (OptionSet option in helpOptionSets)
            {
                option.WriteOptionDescriptions(o);
            }
        }

        private void ShowVersion(TextWriter o)
        {
            string gplNotice =
@"This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.";

            o.WriteLine("RGB Fusion Tool {0}", System.Reflection.Assembly.GetAssembly(this.GetType()).GetName().Version);
            o.WriteLine("Copyright (C) 2018  Tyler Szabo");
            o.WriteLine();
            o.WriteLine(gplNotice);
            o.WriteLine();
            o.WriteLine("Source: https://github.com/tylerszabo/RGB-Fusion-Tool");
        }

        public void Main(string[] args)
        {
            context.SetDefaults();

            try
            {
                List<string> afterGeneric = genericOptions.Parse(args);

                if (context.flag_Help)
                {
                    ShowHelp(stdout);
                    return;
                }

                if (context.flag_Version)
                {
                    ShowVersion(stdout);
                    return;
                }

                zoneOptions.Parse(afterGeneric);

                if (context.flag_List || context.verbosity > 0)
                {
                    for (int i = 0; i < motherboardLEDs.Layout.Length; i++)
                    {
                        stdout.WriteLine("Zone {0}: {1}", i, motherboardLEDs.Layout[i]);
                    }
                }

                if (defaultZone.Count == 0 && zones.Count == 0)
                {
                    return;
                }
                else if (defaultZone.Count > 0 && zones.Count > 0)
                {
                    throw new InvalidOperationException(string.Format("Unexpected options {0} before zone-specific options", string.Join(" ", defaultZone.ToArray())));
                }

                foreach (int zone in zones.Keys)
                {
                    LedSetting setting = null;
                    foreach (LedSettingArgParser parser in ledSettingArgParsers)
                    {
                        setting = parser.TryParse(zones[zone]);
                        if (setting != null) { break; }
                    }
                    motherboardLEDs.LedSettings[zone] = setting ?? throw new InvalidOperationException(string.Format("No LED mode specified for zone {0}", zone));
                }
                if (context.verbosity > 0)
                {
                    foreach (int zone in zones.Keys)
                    {
                        stdout.WriteLine("Set zone {0}: {1}", zone, motherboardLEDs.LedSettings[zone]);
                    }
                }
                if (zones.Count > 0)
                {
                    motherboardLEDs.Set(zones.Keys);
                }

                if (defaultZone.Count > 0)
                {
                    LedSetting setting = null;
                    foreach (LedSettingArgParser parser in ledSettingArgParsers)
                    {
                        setting = parser.TryParse(defaultZone);
                        if (setting != null) { break; }
                    }

                    if (setting == null) { throw new InvalidOperationException("No LED mode specified"); }
                    if (context.verbosity > 0)
                    {
                        stdout.WriteLine("Set All: {0}", setting);
                    }
                    motherboardLEDs.SetAll(setting);
                    return;
                }
            }
            catch (Exception e)
            {
                ShowHelp(stderr);
                stderr.WriteLine();
                stderr.WriteLine("Error: {0}", e.ToString());
                throw;
            }
            return;
        }
    }
}
