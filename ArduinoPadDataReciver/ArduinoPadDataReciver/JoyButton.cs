using System;
using System.Configuration;

namespace ArduinoPadDataReciver
{
    [Serializable]
    public class JoyButton
    {
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public uint Button { get; set; }
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public string PushMsg { get; set; }
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public string ReleaseMsg { get; set; }
        [UserScopedSetting]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public bool IsSwitch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushMsg"></param>
        /// <param name="releaseMsg"></param>
        /// <param name="isSwitch">If not - it is button (moment buton)</param>
        /// <param name="button"></param>
        public JoyButton(string pushMsg, string releaseMsg, bool isSwitch, uint button)
        {
            PushMsg = pushMsg;
            ReleaseMsg = releaseMsg;
            IsSwitch = isSwitch;
            Button = button;
        }

        public override string ToString()
        {
            if (IsSwitch)
                return string.Format("Button {0}: push msg: {1}; release msg: {2}", Button, PushMsg, ReleaseMsg);
            return string.Format("Switch {0}: push msg: {1}; release msg: {2}", Button, PushMsg, ReleaseMsg);
        }

        public JoyButton()
        {

        }
    }
}
