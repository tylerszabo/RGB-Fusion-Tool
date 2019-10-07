// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GLedApiDotNet;
using GvLedLibDotNet;
using System;
using System.IO;
using RGBFusionTool.ArgParsers;

namespace RGBFusionTool
{
    public class Application
    {
        private ApplicationArgParser parser = new ApplicationArgParser();

        private Func<IRGBFusionMotherboard> motherboardFactory;
        private Func<IRGBFusionPeripherals> peripheralsFactory;
        private TextWriter stdout;
        private TextWriter stderr;

        public Application(Func<IRGBFusionMotherboard> motherboardFactory, Func<IRGBFusionPeripherals> peripheralsFactory, TextWriter stdout, TextWriter stderr)
        {
            this.motherboardFactory = motherboardFactory;
            this.peripheralsFactory = peripheralsFactory;
            this.stdout = stdout;
            this.stderr = stderr;
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
            o.WriteLine("Copyright (C) 2018-2019  Tyler Szabo");
            o.WriteLine();
            o.WriteLine(gplNotice);
            o.WriteLine();
            o.WriteLine("Source: https://github.com/tylerszabo/RGB-Fusion-Tool");
        }

        public void Main(string[] args)
        {
            try
            {
                ApplicationContext context = parser.ParseArgs(args);

                if (context.ShowHelp)
                {
                    parser.WriteHelp(stdout);
                    return;
                }

                if (context.ShowVersion)
                {
                    ShowVersion(stdout);
                    return;
                }

                Lazy<IRGBFusionMotherboard> motherboardLEDs = new Lazy<IRGBFusionMotherboard>(motherboardFactory);
                Lazy<IRGBFusionPeripherals> peripheralLEDs = new Lazy<IRGBFusionPeripherals>(peripheralsFactory);

                if (context.ListPeripherals || (context.Verbosity > 0 && context.PeripheralsSetting != null))
                {
                    for (int i = 0; i < peripheralLEDs.Value.Devices.Length; i++)
                    {
                        stdout.WriteLine("Peripheral {0}: {1}", i, peripheralLEDs.Value.Devices[i]);
                    }
                }
                if (context.ListZones || (context.Verbosity > 0 && (context.DefaultSetting != null || context.ZoneSettings?.Count > 0)))
                {
                    for (int i = 0; i < motherboardLEDs.Value.Layout.Length; i++)
                    {
                        stdout.WriteLine("Zone {0}: {1}", i, motherboardLEDs.Value.Layout[i]);
                    }
                }

                if (context.DefaultSetting != null)
                {
                    if (context.Verbosity > 0)
                    {
                        stdout.WriteLine("Set All: {0}", context.DefaultSetting);
                    }
                    motherboardLEDs.Value.SetAll(context.DefaultSetting);
                }
                else if (context.ZoneSettings?.Count > 0)
                {
                    foreach (int zone in context.ZoneSettings.Keys)
                    {
                        if (zone >= motherboardLEDs.Value.Layout.Length)
                        {
                            throw new InvalidOperationException(string.Format("Zone is {0}, max supported is {1}", zone, motherboardLEDs.Value.Layout.Length));
                        }

                        motherboardLEDs.Value.LedSettings[zone] = context.ZoneSettings[zone] ?? throw new InvalidOperationException(string.Format("No LED mode specified for zone {0}", zone));
                        if (context.Verbosity > 0)
                        {
                            stdout.WriteLine("Set zone {0}: {1}", zone, motherboardLEDs.Value.LedSettings[zone]);
                        }
                    }

                    motherboardLEDs.Value.Set(context.ZoneSettings.Keys);
                }

                if (context.PeripheralsSetting != null)
                {
                    if (context.Verbosity > 0)
                    {
                        stdout.WriteLine("Set All Peripherals: {0}", context.PeripheralsSetting);
                    }
                    peripheralLEDs.Value.SetAll(context.PeripheralsSetting);
                }
            }
            catch (Exception e)
            {
                parser.WriteHelp(stderr);
                stderr.WriteLine();
                stderr.WriteLine("Error: {0}", e.ToString());
                throw;
            }
            return;
        }
    }
}
