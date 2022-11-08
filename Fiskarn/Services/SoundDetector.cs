using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fiskarn.Services
{
    public class SoundDetector
    {
        public static float Sensitivity = 0.04f;

        public int DeviceIndex { get; }

        private MMDevice _device;

        public SoundDetector(int deviceIndex)
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            _device = devices[deviceIndex];
            DeviceIndex = deviceIndex;
        }

        public static List<string> GetAllDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            return devices.Select(d => d.FriendlyName).ToList();
        }

        public bool HasVolume()
        {
            return _device.AudioMeterInformation.MasterPeakValue > Sensitivity;
        }

        public string GetDeviceName()
        {
            return _device.FriendlyName;
        }

        public float GetVolume()
        {
            return _device.AudioMeterInformation.MasterPeakValue;
        }
    }
}
