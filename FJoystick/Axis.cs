using System;
using System.ComponentModel;

namespace FJoystick
{
    public class Axis : INotifyPropertyChanged
    {
        #region Fields

        private int _channel;
        private int _joystickValue;
        private int _maxJoystickValue;
        private ushort _maxRcValue;
        private int _minJoystickValue;
        private ushort _minRcValue;
        private ushort _rcValue;

        #endregion Fields

        #region Constructors

        public Axis(int channel, ushort minRcValue = 1000, ushort maxRcValue = 2000, int minJoystickValue = 0, int maxJoystickValue = 32767)
        {
            _channel = channel;
            _minRcValue = minRcValue;
            _maxRcValue = maxRcValue;
            _minJoystickValue = minJoystickValue;
            _maxJoystickValue = maxJoystickValue;
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public int Channel
        {
            get { return _channel; }
        }

        public int JoystickValue
        {
            get { return _joystickValue; }
        }

        public int MaxJoystickValue
        {
            get { return _maxJoystickValue; }
            set { _maxJoystickValue = value; }
        }

        public int MinJoystickValue
        {
            get { return _minJoystickValue; }
            set { _minJoystickValue = value; }
        }

        public ushort RcValue
        {
            get { return _rcValue; }
            set
            {
                _rcValue = Math.Min(Math.Max(value, _minRcValue), _maxRcValue);
                _joystickValue = ((_rcValue - _minRcValue) * _maxJoystickValue) / (_maxRcValue - _minRcValue);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("JoystickValue"));
            }
        }

        public int Value
        {
            get { return _joystickValue; }
        }

        #endregion Properties

        #region Methods

        public void SetValue(UInt16 rcValue)
        {
            if (rcValue <= 1000)
            {
                _joystickValue = 0;
            }
            else if (rcValue >= 2000)
            {
                _joystickValue = 100;
            }
            else
            {
                _joystickValue = (rcValue - 1000) / 10;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }

        #endregion Methods
    }
}