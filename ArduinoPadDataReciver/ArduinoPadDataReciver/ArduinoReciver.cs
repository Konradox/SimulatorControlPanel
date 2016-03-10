using System.Collections.ObjectModel;
using System.IO.Ports;

namespace ArduinoPadDataReciver
{
    internal class ArduinoReciver
    {
        private ObservableCollection<JoyButton> _lstJoyButtons;
        private Joystick _joy;
        private SerialPort _mySerialPort;

        public ArduinoReciver()
        {
            _mySerialPort = new SerialPort();
            _joy = new Joystick(1);
        }

        public void Start(ObservableCollection<JoyButton> lstJoyButtons, int baud, int device, string port)
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
            var sp = (SerialPort)sender;
            string indata = sp.ReadLine();

            foreach (JoyButton button in _lstJoyButtons)
            {
                _joy.HandlePushButton(button, indata);
            }
        }

        public void Stop()
        {
            if (_mySerialPort.IsOpen)
            {
                _mySerialPort.Close();
                _joy.Stop();
            }
        }

        public int Buttons()
        {
            return _joy.MaxButtons();
        }
    }
}
