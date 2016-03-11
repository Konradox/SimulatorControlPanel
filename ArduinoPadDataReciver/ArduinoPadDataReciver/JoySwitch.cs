using System.Configuration;
using System.Xml.Serialization;
using vJoyInterfaceWrap;

namespace ArduinoPadDataReciver
{
    public class JoySwitch : JoyControl
    {
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [XmlElement("Button")]
        public uint Button { get; set; }
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [XmlElement("PushMsg")]
        public string PushMsg { get; set; }
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [XmlElement("ReleaseMsg")]
        public string ReleaseMsg { get; set; }

        public override void HandleButton(string line, vJoy joystick, uint deviceId)
        {
            if (line == string.Format("{0}\r", PushMsg) || line == string.Format("{0}\r", ReleaseMsg))
            {
                joystick.SetBtn(true, deviceId, Button);
                joystick.SetBtn(false, deviceId, Button);
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(ReleaseMsg))
                return string.Format("Switch {0}: msg: {1}", Button, PushMsg);
            return string.Format("Switch {0}: msg1: {1}; msg2: {2}", Button, PushMsg, ReleaseMsg);
        }

        public JoySwitch()
        {
        }

        public JoySwitch(string msg1, string msg2, uint button)
        {
            Button = button;
            PushMsg = msg1;
            ReleaseMsg = msg2;
        }
    }
}
