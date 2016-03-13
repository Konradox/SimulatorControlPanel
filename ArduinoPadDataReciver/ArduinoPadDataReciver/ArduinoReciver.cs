using System;
using System.CodeDom;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace ArduinoPadDataReciver
{
    internal class ArduinoReciver
    {
        private ObservableCollection<JoyControl> _lstJoyButtons;
        private Joystick _joy;
        private SerialPort _mySerialPort;

        public ArduinoReciver()
        {
            _mySerialPort = new SerialPort();
            _joy = new Joystick(1);
        }

        public void Start(ObservableCollection<JoyControl> lstJoyButtons, int baud, int device, string port)
        {
            _lstJoyButtons = lstJoyButtons;
            
            _joy.Start(device);

            _mySerialPort.BaudRate = baud;
            _mySerialPort.PortName = port;
            _mySerialPort.DataReceived += DataReceivedHandler;

            _mySerialPort.Open();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var sp = (SerialPort) sender;
                string indata = sp.ReadLine();

                foreach (JoyControl button in _lstJoyButtons)
                {
                    _joy.HandlePushButton(button, indata);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Stop()
        {
            for (var i=0; i<5; i++)
            {
                try
                {
                    if (_mySerialPort.IsOpen)
                    {
                        _mySerialPort.Close();
                        _joy.Stop();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
        }

        public int Buttons()
        {
            return _joy.MaxButtons();
        }

        public IEnumerable Axes()
        {
            return _joy.Axes();
        }

    }
}
