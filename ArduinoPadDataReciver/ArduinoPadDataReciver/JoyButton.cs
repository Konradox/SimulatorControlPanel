using vJoyInterfaceWrap;

namespace ArduinoPadDataReciver
{
    public class JoyButton : JoyControl
    {
        public override void HandleControl(string line, vJoy joystick, uint deviceId)
        {
            if (line == string.Format("{0}\r", PushMsg))
                joystick.SetBtn(true, deviceId, Button);
            if (line == string.Format("{0}\r", ReleaseMsg))
                joystick.SetBtn(false, deviceId, Button);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushMsg"></param>
        /// <param name="releaseMsg"></param>
        /// <param name="controlType/param>
        /// <param name="button"></param>
        public JoyButton(string pushMsg, string releaseMsg, uint button)
        {
            PushMsg = pushMsg;
            ReleaseMsg = releaseMsg;
            Button = button;
        }

        public override string ToString()
        {
            return string.Format("Button {0}: push msg: {1}; release msg: {2}", Button, PushMsg, ReleaseMsg);
        }

        public JoyButton()
        {

        }
    }
}
