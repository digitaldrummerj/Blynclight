using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;

namespace BlyncLight
{
    public class Manager : IDisposable
    {
        public Device BlyncLight { get; protected set; }
        
        public async Task Init()
        {
            this.BlyncLight = await GetBlyncLightDevice();
        }

        private async Task<Device> GetBlyncLightDevice()
        {
            HidDevice device = null;
            DeviceInformation info = null;

            ushort vendorId = 0x0E53;
            ushort productId = 0x2516;
            ushort usagePage = 0xFF00;
            ushort usageId = 0x0001;
            
            string selector = HidDevice.GetDeviceSelector(usagePage, usageId, vendorId, productId);

            var devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Any())
            {
                info = devices.Single();
                device = await HidDevice.FromIdAsync(devices.Single().Id, FileAccessMode.ReadWrite);
            }

            return new Device(info, device);
        }

        public void Dispose()
        {
            BlyncLight.Dispose();
        }
    }
}
