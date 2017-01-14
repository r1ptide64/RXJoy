using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using vJoyInterfaceWrap;


namespace FJoystick
{
    class vJoyHelper
    {
        private vJoy _joystick;
        private USBHelper _usb;
        public vJoy.JoystickState _jState;
        private long _maxVal;
        private Dictionary<HID_USAGES, bool> _axisAry;

        public vJoyHelper(USBHelper usb)
        {
            _joystick = new vJoy();
            bool tmp = _joystick.vJoyEnabled();
            _jState = new vJoy.JoystickState();
            _joystick.AcquireVJD(1);
            _joystick.ResetVJD(1);
            _maxVal = 0;
            _axisAry = new Dictionary<HID_USAGES, bool>();
            foreach (HID_USAGES hidUsage in Enum.GetValues(typeof(HID_USAGES)))
            {
                _axisAry.Add(hidUsage, _joystick.GetVJDAxisExist(1, hidUsage));
            }

            _joystick.GetVJDAxisMax(1, HID_USAGES.HID_USAGE_X, ref _maxVal);
            _usb = usb;
            usb.Polled += new EventHandler(OnUSBPolled);
        }

        public void OnUSBPolled(object sender, EventArgs e)
        {
            //_joystick.SetAxis(Convert.ToInt32(_usb.Axes[0].Value / 100.0 * _maxVal), 1, HID_USAGES.HID_USAGE_X);
            _jState.bDevice = 1;
            _jState.AxisX = _usb.Axes[0].JoystickValue;
            _jState.AxisY = _usb.Axes[1].JoystickValue;
            _jState.AxisZ = _usb.Axes[2].JoystickValue;
            _jState.AxisXRot = _usb.Axes[3].JoystickValue;
            //_jState.AxisX = _usb.Axes[0].Value;
            //_jState.AxisY = _usb.Axes[1].Value;
            //_jState.AxisZ = _usb.Axes[2].Value;
            //_jState.AxisXRot = _usb.Axes[3].Value;
            bool tmp = _joystick.UpdateVJD(1, ref _jState);
        }
    }
}
