using vJoyInterfaceWrap;

namespace ArduinoPadDataReciver
{
    public class JoyDigitalAxis : JoyControl
    {
        private int pos = 1;
        private HID_USAGES selectedAxis;
        public override void HandleControl(string line, vJoy joystick, uint deviceId)
        {
            if (line == string.Format("{0}\r", PushMsg))
            {
                pos += 654;
                if (pos > 32767)
                    pos = 32767;
                joystick.SetAxis(pos, deviceId, (HID_USAGES)Axis);
            }
            if (line == string.Format("{0}\r", ReleaseMsg))
            {
                pos -= 654;
                if (pos < 1)
                    pos = 1;
                joystick.SetAxis(pos, deviceId, (HID_USAGES)Axis);
            }
        }

        public override string ToString()
        {
            return string.Format("Axis {0}: inc msg: {1}; dec msg: {2}", Axis, PushMsg, ReleaseMsg);
        }

        public JoyDigitalAxis()
        {
        }

        public JoyDigitalAxis(string incMessage, string decMessage, string axis)
        {
            if (axis == "X")
                Axis = 0x30;
            if (axis == "Y")
                Axis = 0x31;
            if (axis == "Z")
                Axis = 0x32;
            if (axis == "RX")
                Axis = 0x33;
            if (axis == "RY")
                Axis = 0x34;
            if (axis == "RZ")
                Axis = 0x35;
            if (axis == "Slider 0")
                Axis = 0x36;
            if (axis == "Slider 1")
                Axis = 0x37;
            PushMsg = incMessage;
            ReleaseMsg = decMessage;

        }
    }
}
