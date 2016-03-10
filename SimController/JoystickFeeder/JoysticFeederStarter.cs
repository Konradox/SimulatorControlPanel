using System;
using System.IO.Ports;
using vJoyInterfaceWrap;

namespace JoystickFeeder
{
    public class JoysticFeederStarter
    {
        private vJoy _joystick;
        private readonly uint _deviceId = 1;
        private SerialPort _arduinoSerialPort;


        static void Main(string[] args)
        {
            JoysticFeederStarter jfs = new JoysticFeederStarter();
            jfs.Start();
        }

        private void Start()
        {
            _joystick = new vJoy();

            if (!CheckVJoy()) return;

            if (!OpenSerialPort()) return;

            _joystick.ResetAll();

            MainLoop(); 
        }

        private void MainLoop()
        {
            while (true)
            {
                var line = _arduinoSerialPort.ReadLine();

                HandlePushButton(_deviceId, line, 1, 30);
                HandlePushButton(_deviceId, line, 2, 31);
                HandlePushButton(_deviceId, line, 3, 32);
                HandlePushButton(_deviceId, line, 4, 33);
            }
        }

        /// <summary>
        /// Handle button push and release
        /// </summary>
        /// <param name="deviceId">Joystick (VJoy device) ID</param>
        /// <param name="line">Serial port message</param>
        /// <param name="button">Joystick button</param>
        /// <param name="pinId">Arduino prot id - used in simple protocol. Protocol message: id + state + line end symbol
        /// For example: "301\r"; pin id = 30, state = 1, line end \r</param>
        private void HandlePushButton(uint deviceId, string line, uint button, int pinId)
        {
            if (line == string.Format("{0}1\r", pinId))
                _joystick.SetBtn(true, deviceId, button);
            if (line == string.Format("{0}0\r", pinId))
                _joystick.SetBtn(false, deviceId, button);
        }

        private bool OpenSerialPort()
        {
            var bRate = 9600;
            var pName = "COM2";

            Console.WriteLine("Load default serial port settings (COM2, 9600) ? t/n");
            var key = Console.ReadKey().Key;
            Console.Write(Environment.NewLine);
            if (key == ConsoleKey.N)
            {
                Console.WriteLine("Baud rate: ");
                while (!int.TryParse(Console.ReadLine(), out bRate)) { }

                Console.WriteLine("Port name: ");
                pName = Console.ReadLine();
            }

            _arduinoSerialPort = new SerialPort
            {
                BaudRate = bRate,
                PortName = pName,
                DtrEnable = true
            };
            try
            {
                _arduinoSerialPort.Open();
            }
            catch (Exception)
            {
                Console.WriteLine("Connection failed. Check your port");
                Console.ReadKey(true);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checking VJoy - copied from documentation
        /// </summary>
        /// <returns></returns>
        private bool CheckVJoy()
        {
            if (!_joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return false;
            }

            // Get the state of the requested device
            VjdStat status = _joystick.GetVJDStatus(_deviceId);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", _deviceId);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", _deviceId);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", _deviceId);
                    return false;
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", _deviceId);
                    return false;
                default:
                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", _deviceId);
                    return false;
            }
            // Test if DLL matches the driver
            UInt32 dllVer = 0, drvVer = 0;
            bool match = _joystick.DriverMatch(ref dllVer, ref drvVer);

            if (match)
                Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", dllVer);
            else
                Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", drvVer, dllVer);


            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!_joystick.AcquireVJD(_deviceId))))
            {
                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", _deviceId);
                return false;
            }
            Console.WriteLine("Acquired: vJoy device number {0}.\n", _deviceId);
            return true;
        }
    }
}
