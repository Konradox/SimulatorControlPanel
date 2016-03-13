using System.Configuration;
using System.Xml.Serialization;
using vJoyInterfaceWrap;

namespace ArduinoPadDataReciver
{
    [XmlInclude(typeof(JoyButton))]
    [XmlInclude(typeof(JoySwitch))]
    [XmlInclude(typeof(JoyDigitalAxis))]
    public abstract class JoyControl
    {
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [XmlElement("Button")]
        public uint Button{ get; set; }
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [XmlElement("PushMsg")]
        public string PushMsg { get; set; }
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [XmlElement("ReleaseMsg")]
        public string ReleaseMsg { get; set; }
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        [XmlElement("Axis")]
        public int Axis { get; set; }

        public abstract void HandleControl(string line, vJoy joystick, uint deviceId);
    }
}
