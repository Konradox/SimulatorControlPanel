using System;
using System.Configuration;
using System.Xml.Serialization;
using vJoyInterfaceWrap;

namespace ArduinoPadDataReciver
{
    [System.Xml.Serialization.XmlInclude(typeof(JoyButton))]
    [System.Xml.Serialization.XmlInclude(typeof(JoySwitch))]
    public abstract class JoyControl
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

        public abstract void HandleButton(string line, vJoy joystick, uint deviceId);
    }
}
