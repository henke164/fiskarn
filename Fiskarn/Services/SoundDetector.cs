using NAudio.CoreAudioApi;

namespace Fiskarn.Services
{
    public class SoundDetector
    {
        public int DeviceIndex { get; }

        private MMDevice _device;

        public SoundDetector(int deviceIndex)
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            _device = devices[deviceIndex];
            DeviceIndex = deviceIndex;
        }

        public bool HasVolume()
        {
            return _device.AudioMeterInformation.MasterPeakValue > 0.2;
        }

        public float GetVolume()
        {
            return _device.AudioMeterInformation.MasterPeakValue;
        }
    }
}
