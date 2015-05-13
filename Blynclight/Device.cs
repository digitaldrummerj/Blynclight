using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.UI;

namespace BlyncLight
{
    public class Device : IDisposable
    {
        private readonly Color DEFAULT_COLOR = Colors.Black;

        private readonly Tuple<DeviceInformation, HidDevice> _store;
        private Color _color;

        public Device(DeviceInformation info, HidDevice device)
        {
            _store = new Tuple<DeviceInformation, HidDevice>(info, device);
            _color = DEFAULT_COLOR;
        }

        public DeviceInformation DeviceInformation
        {
            get { return _store.Item1; }
        }

        public HidDevice HIDDevice
        {
            get { return _store.Item2; }
        }

        public Color StatusColor
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    SendColorCommand(_color);
                }
            }
        }

        private void SendColorCommand(Color color)
        {
            Byte red = color.R;
            Byte green = color.G;
            Byte blue = color.B;

            var commandBuffer = new Byte[9];
            commandBuffer[0] = 0x00;
            commandBuffer[1] = red;
            commandBuffer[2] = blue;
            commandBuffer[3] = green;
            commandBuffer[4] = 0; // 0 is stable, 70 is fast blink, 100 is medium blink 
            commandBuffer[5] = 0x00;
            commandBuffer[6] = 0x40;
            commandBuffer[7] = 0x02;
            commandBuffer[8] = 0xFF; // Did this turn it off? controlCode & 0xFF 

            WriteOutputReport(commandBuffer);
        }

        private async void WriteOutputReport(Byte[] data)
        {
            var report = _store.Item2.CreateOutputReport();

            // Only grab the byte we need            
            Byte[] bytesToModify = data;

            WindowsRuntimeBufferExtensions.CopyTo(bytesToModify, 0, report.Data, 0, bytesToModify.Length);

            await _store.Item2.SendOutputReportAsync(report);
        }

        public void Dispose()
        {
            _color = DEFAULT_COLOR;
        }
    }
}