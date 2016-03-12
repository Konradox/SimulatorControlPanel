using System;
using vJoyInterfaceWrap;

namespace ArduinoPadDataReciver
{
    internal class Joystick
    {
        private uint _deviceId;
        private readonly vJoy _joystick;


        public Joystick(uint deviceId)
        {
            _deviceId = deviceId;
            _joystick = new vJoy();
        }

        public int MaxButtons()
        {
            return _joystick.GetVJDButtonNumber(_deviceId);
        }

        public void Stop()
        {
            _joystick.RelinquishVJD(_deviceId);
        }

        public void HandlePushButton(JoyControl control, string line)
        {
            control.HandleControl(line, _joystick, _deviceId);
        }

        /// <summary>
        /// Checking VJoy - copied from documentation
        /// </summary>
        /// <returns></returns>
        public void CheckVJoy()
        {
            if (!_joystick.vJoyEnabled())
            {
                throw new Exception("vJoy driver not enabled: Failed Getting vJoy attributes.");
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
                    throw new Exception(string.Format("vJoy Device {0} general error. Cannot continue.", _deviceId));
            }

            UInt32 dllVer = 0, drvVer = 0;
            _joystick.DriverMatch(ref dllVer, ref drvVer);


            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!_joystick.AcquireVJD(_deviceId))))
            {
                throw new Exception(string.Format("Failed to acquire vJoy device number {0}.", _deviceId));
            }

            _joystick.ResetAll();
        }

        public void Start(int device)
        {
            _deviceId = (uint) device;
            CheckVJoy();
        }
    }
}
