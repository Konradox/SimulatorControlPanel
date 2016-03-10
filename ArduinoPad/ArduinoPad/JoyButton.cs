using System;
using System.Configuration;

namespace ArduinoPad
{
    [Serializable]
    public class JoyButton
    {
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public uint Button { get; set; }
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public int Pin { get; set; }
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string PushMsg { get; set; }
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string ReleaseMsg { get; set; }
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public bool IsReset { get; set; }

        public JoyButton(int pin, string pushMsg, string releaseMsg, bool isReset, uint button)
        {
            Pin = pin;
            PushMsg = pushMsg;
            ReleaseMsg = releaseMsg;
            IsReset = isReset;
            Button = button;
        }

        public override string ToString()
        {
            if (IsReset)
                return string.Format("Reset button: pin {0}; push msg: {1}; release msg: {2}", Pin, PushMsg, ReleaseMsg);
            return string.Format("Lasting button: pin {0}; push msg: {1}; release msg: {2}", Pin, PushMsg, ReleaseMsg);
        }

        public JoyButton()
        {
            
        }
    }
}
