// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GvLedLibDotNet.GvLedSettings;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GvLedLibDotNet
{
    public class RGBFusionPeripherals : IRGBFusionPeripherals
    {
        private class PeripheralDevicesImpl : IPeripheralDevices
        {
            private readonly DeviceType[] devices;

            internal PeripheralDevicesImpl(DeviceType[] devices)
            {
                this.devices = devices;
            }

            public DeviceType this[int i] => devices[i];

            public int Length => devices.Length;

            public IEnumerator<DeviceType> GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => devices.GetEnumerator();
        }

        private class GvLedSettingsImpl : IGvLedSettings
        {
            private GvLedSetting[] settings;
            private Raw.GvLedLibv1_0Wrapper api;

            internal GvLedSettingsImpl(int size, Raw.GvLedLibv1_0Wrapper wrapperAPI)
            {
                api = wrapperAPI;

                settings = new GvLedSetting[size];
                for (int i = 0; i < settings.Length; i++)
                {
                    settings[i] = new OffGvLedSetting();
                }
            }

            public GvLedSetting this[int i]
            {
                get => settings[i];
                set
                {
                    api.LedSave(i, value);
                    settings[i] = value;
                }
            }

            public int Length => settings.Length;

            public IEnumerator<GvLedSetting> GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => settings.GetEnumerator();
        }


        private Raw.GvLedLibv1_0Wrapper api;
        internal RGBFusionPeripherals(Raw.GvLedLibv1_0Wrapper wrapperAPI)
        {
            api = wrapperAPI;

            devices = new Lazy<PeripheralDevicesImpl>(() => new PeripheralDevicesImpl(api.Initialize()));
            settings = new Lazy<GvLedSettingsImpl>(() => new GvLedSettingsImpl(devices.Value.Length, api));
        }

        public RGBFusionPeripherals() : this(new Raw.GvLedLibv1_0Wrapper())
        {
        }

        private Lazy<PeripheralDevicesImpl> devices;
        public IPeripheralDevices Devices => devices.Value;

        private Lazy<GvLedSettingsImpl> settings;
        public IGvLedSettings LedSettings => settings.Value;

        public void SetAll(GvLedSetting ledSetting)
        {
            if (settings.Value.Length > 0)
            {
                api.Save(ledSetting);
            }
        }
    }
}
