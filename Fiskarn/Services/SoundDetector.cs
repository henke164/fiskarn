using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Linq;

namespace Fiskarn.Services
{
    public class SoundDetector
    {
        private MMDevice _device;

        public SoundDetector(int deviceIndex)
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            _device = devices[deviceIndex];
        }

        public bool HasVolume()
        {
            return _device.AudioMeterInformation.MasterPeakValue > 0.3;
        }

        public float GetVolume()
        {
            return _device.AudioMeterInformation.MasterPeakValue;
        }
    }
}
