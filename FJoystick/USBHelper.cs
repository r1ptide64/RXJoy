using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;

namespace FJoystick
{
    class USBHelper : IDisposable
    {

        #region Fields

        private Axis[] _axes;
        private byte[] _inBuf;
        private byte[] _outBuf;
        private SerialPort _sp;
        private MainWindow _mainWindow;
        private Timer _timer;

        #endregion Fields

        #region Constructors

        public USBHelper(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _sp = new SerialPort("COM6", 115200);
            _sp.Open();
            _outBuf = new byte[6];
            _outBuf[0] = 36;
            _outBuf[1] = 77;
            _outBuf[2] = 60;
            _outBuf[3] = 0;
            _outBuf[4] = 105;
            _outBuf[5] = Convert.ToByte(_outBuf[3] ^ _outBuf[4]);
            _inBuf = new byte[64];
            _sp.DataReceived += new SerialDataReceivedEventHandler(OnSerialDataReceived);
            _timer = new Timer(new TimerCallback(this.Refresh), null, 0, 50);
        }

        #endregion Constructors

        #region Events

        public event EventHandler AxesSetup;
        public event EventHandler Polled;

        #endregion Events

        #region Properties

        public Axis[] Axes
        {
            get { return _axes; }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
            if (_sp != null && _sp.IsOpen)
            {
                _sp.Close();
            }  
        }

        public void Refresh()
        {
            _sp.Write(_outBuf, 0, _outBuf.Length);
        }

        public void Refresh(object unused)
        {
            Refresh();
        }

        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Bare minimum
            while (_sp.BytesToRead <= 5)
            {
                Thread.Sleep(1);
            }
            _sp.Read(_inBuf, 0, 5);

            // Not an RC data frame, or 0-length data
            if (_inBuf[4] != 105 || _inBuf[3] <= 0)
            {
                _sp.DiscardInBuffer();
                return;
            }

            // Await full data payload
            while (_sp.BytesToRead < _inBuf[3] + 1)
            {
                Thread.Sleep(1);
            }

            _sp.Read(_inBuf, 5, _sp.BytesToRead);
            if (_axes == null)
            {
                _axes = new Axis[_inBuf[3] / 2];
                for (int i = 0; i < _axes.Length; i++)
                {
                    _axes[i] = new Axis(i + 1);
                }
                AxesSetup?.Invoke(this, new EventArgs());
            }
            SetValues();
            Polled?.Invoke(this, new EventArgs());
        }
        private void SetValues()
        {
            for (int i = 0; i < _axes.Length; i++)
            {
                _axes[i].RcValue = BitConverter.ToUInt16(_inBuf, 5 + i * 2);
                //_axes[i].SetValue(BitConverter.ToUInt16(_inBuf, 5 + i * 2));
            }
        }
        

        #endregion Methods
    }
}
