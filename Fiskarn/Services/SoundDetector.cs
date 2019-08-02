using NAudio.CoreAudioApi;

namespace Fiskarn.Services
{
    public class SoundDetector
    {
        private MMDevice _device;

        public SoundDetector()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            _device = devices[1];
        }

        public bool HasVolume()
        {
            return _device.AudioMeterInformation.MasterPeakValue > 0.3;
        }
    }
}
