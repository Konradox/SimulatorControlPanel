using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using vJoyInterfaceWrap;

namespace ArduinoPad
{
    internal class Joystick
    {
        private readonly uint _deviceId;
        private SerialPort _arduinoSerialPort;
        private readonly vJoy _joystick;
        private ObservableCollection<JoyButton> _lstJoyButtons;

        public Joystick(uint deviceId)
        {
            _deviceId = deviceId;
            _joystick = new vJoy();
        }

        public void HandlePushButton(JoyButton button, string line)
        {
            if (button.IsReset)
            {
                if (line == string.Format("{0}\r", button.PushMsg))
                    _joystick.SetBtn(true, _deviceId, button.Button);
                if (line == string.Format("{0}\r", button.ReleaseMsg))
                    _joystick.SetBtn(false, _deviceId, button.Button);
            }
            else
            {
                if (line == string.Format("{0}\r", button.PushMsg) || line == string.Format("{0}\r", button.ReleaseMsg))
                {
                    _joystick.SetBtn(true, _deviceId, button.Button);
                    _joystick.SetBtn(false, _deviceId, button.Button);
                }
            }
        }

        public bool OpenSerialPort(int bRate, string pName)
        {
            _arduinoSerialPort = new SerialPort
            {
                BaudRate = bRate,
                PortName = pName,
                DtrEnable = true
            };
            try
            {
                _arduinoSerialPort.Close();
                _arduinoSerialPort.Open();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checking VJoy - copied from documentation
        /// </summary>
        /// <returns></returns>
        public void CheckVJoy()
        {
            if (!_joystick.vJoyEnabled())
            {
                throw new Exception( "vJoy driver not enabled: Failed Getting vJoy attributes.");
            }

            // Get the state of the requested device
            VjdStat status = _joystick.GetVJDStatus(_deviceId);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    break;
                case VjdStat.VJD_STAT_FREE:
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    throw new Exception(string.Format("vJoy Device {0} is already owned by another feeder. Cannot continue.", _deviceId));
                case VjdStat.VJD_STAT_MISS:
                    throw new Exception(string.Format("vJoy Device {0} is not installed or disabled. Cannot continue.", _deviceId));
                default:
                    throw new Exception( string.Format("vJoy Device {0} general error. Cannot continue.", _deviceId));
            }

            UInt32 dllVer = 0, drvVer = 0;
            bool match = _joystick.DriverMatch(ref dllVer, ref drvVer);


            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!_joystick.AcquireVJD(_deviceId))))
            {
                throw new Exception(string.Format("Failed to acquire vJoy device number {0}.", _deviceId));
            }

            _joystick.ResetAll();
        }

        public string ReadSerialLine()
        {
            if (_arduinoSerialPort.IsOpen)
                return _arduinoSerialPort.ReadLine();
            return "";
        }

        public void CloseSerialPort()
        {
            if (_arduinoSerialPort.IsOpen)
            {
                _arduinoSerialPort.Close();
            }
        }

        public void Start(ObservableCollection<JoyButton> lstJoyButtons)
        {
            _lstJoyButtons = lstJoyButtons;
            _arduinoSerialPort.DataReceived += DataRecived;
        }

        private void DataRecived(object sender, SerialDataReceivedEventArgs e)
        {
            string line = ReadSerialLine();
            foreach (JoyButton lstJoyButton in _lstJoyButtons)
            {
                HandlePushButton(lstJoyButton, line);
            }
        }
    }
}
