// Copyright (C) 2019 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GLedApiDotNet.LedSettings;
using GvLedLibDotNet.GvLedSettings;
using Mono.Options;
using RGBFusionTool.ArgParsers.GvLedSettings;
using RGBFusionTool.ArgParsers.LedSettings;
using System;
using System.Collections.Generic;

namespace RGBFusionTool.ArgParsers
{
    class ApplicationArgParser
    {
        private List<LedSettingArgParser<LedSetting>> ledSettingArgParsers = new List<LedSettingArgParser<LedSetting>>
        {
            new StaticColorArgParser(),
            new ColorCycleArgParser(),
            new PulseArgParser(),
            new FlashArgParser(),
            new DigitalAArgParser(),
            new DigitalBArgParser(),
            new DigitalCArgParser(),
            new DigitalDArgParser(),
            new DigitalEArgParser(),
            new DigitalFArgParser(),
            new DigitalGArgParser(),
            new DigitalHArgParser(),
            new DigitalIArgParser(),
            new OffArgParser(),
        };

        private List<LedSettingArgParser<GvLedSetting>> gvLedSettingArgParsers = new List<LedSettingArgParser<GvLedSetting>>
        {
            new OffGvArgParser(),
            new StaticColorGvArgParser(),
            new ColorCycleGvArgParser()
        };

        private T ParseLedSetting<T>(IEnumerable<string> args, IEnumerable<LedSettingArgParser<T>> parsers)
        {
            foreach (LedSettingArgParser<T> parser in parsers)
            {
                T setting = parser.Parse(args);
                if (setting != default) { return setting; }
            }

            throw new InvalidOperationException("No mode specified");
        }

        private List<OptionSet> helpOptionSets;

        public ApplicationContext Parse(IEnumerable<string> args)
        {
            ApplicationContext context = new ApplicationContext();

            List<string> defaultZoneArgs = new List<string>();
            List<string> currentZone = defaultZoneArgs;
            List<string> peripheralsArgs = new List<string>();

            OptionSet genericOptions = new OptionSet
            {
                { string.Format("Usage: {0} [OPTION]... [[LEDSETTING] | [ZONE LEDSETTING]...] [peripherals GVSETTING]", AppDomain.CurrentDomain.FriendlyName) },
                { "Set RGB Fusion motherboard LEDs" },
                { "" },
                { "Options:" },
                { "v|verbose", v => context.Verbosity++ },
                { "l|list", "list zones", v => context.ListZones = true },
                { "list-peripherals", "list peripherals", v => context.ListPeripherals = true },
                { "la|list-all", "list peripherals", v => context.ListAll() },
                { "?|h|help", "show help and exit", v => context.ShowHelp = true  },
                { "version", "show version information and exit", v => context.ShowVersion = true },
                { "" }
            };

            Dictionary<int, List<string>> zoneArgs = new Dictionary<int, List<string>>();
            OptionSet zoneOptions = new OptionSet
            {
                { "ZONE:" },
                { "z|zone=", "set zone", (int zone) => {
                    if (zone < 0)
                    {
                        throw new InvalidOperationException("Zones must be positive integers");
                    }
                    if (zoneArgs.ContainsKey(zone))
                    {
                        throw new InvalidOperationException(string.Format("Zone {0} already specified", zone));
                    }
                    currentZone = new List<string>();
                    zoneArgs.Add(zone, currentZone);
                } },
                { "PERIPHERALS:" },
                { "peripherals", "set peripherals", v => {
                    currentZone = new List<string>();
                    peripheralsArgs = currentZone;
                } },
                { "<>", v => currentZone.Add(v) },
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
            helpOptionSets.Add(new OptionSet { "" });
            helpOptionSets.Add(new OptionSet { "GVSETTING options:" });
            foreach (LedSettingArgParser argParser in gvLedSettingArgParsers)
            {
                helpOptionSets.Add(new OptionSet { "" });
                helpOptionSets.Add(argParser.RequiredOptions);
                helpOptionSets.Add(argParser.ExtraOptions);
            }

            List<string> afterGeneric = genericOptions.Parse(args);

            if (context.ShowHelp)
            {
                return context;
            }

            if (context.ShowVersion)
            {
                return context;
            }

            zoneOptions.Parse(afterGeneric);

            if (defaultZoneArgs.Count > 0 && zoneArgs.Count > 0)
            {
                throw new InvalidOperationException(string.Format("Unexpected options {0} before zone-specific options", string.Join(" ", defaultZoneArgs.ToArray())));
            }
            else if (defaultZoneArgs.Count > 0)
            {
                context.DefaultSetting = ParseLedSetting(defaultZoneArgs, ledSettingArgParsers);
            }
            else if (zoneArgs.Count > 0)
            {
                context.ZoneSettings = new Dictionary<int, LedSetting>();

                foreach (int zone in zoneArgs.Keys)
                {
                    context.ZoneSettings.Add(zone, ParseLedSetting(zoneArgs[zone], ledSettingArgParsers));
                }
            }

            if (peripheralsArgs.Count > 0)
            {
                context.PeripheralsSetting = ParseLedSetting(peripheralsArgs, gvLedSettingArgParsers);
            }

            return context;
        }

        public void WriteHelp(System.IO.TextWriter o)
        {
            foreach (OptionSet option in helpOptionSets)
            {
                option.WriteOptionDescriptions(o);
            }
        }
    }
}
