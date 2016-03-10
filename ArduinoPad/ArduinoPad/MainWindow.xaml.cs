using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using MahApps.Metro.Controls.Dialogs;

namespace ArduinoPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool _started = false;
        private ObservableCollection<JoyButton> _lstJoyButtons;
        private Joystick _joy;

        public MainWindow()
        {
            InitializeComponent();
            Deserialize();
            LbxButtons.ItemsSource = _lstJoyButtons;
            TbxBaudRate.Text = Properties.Settings.Default.BaudRate.ToString();
            TbxDevice.Text = Properties.Settings.Default.Device.ToString();
            TbxSerialProt.Text = Properties.Settings.Default.SerialPort;
        }

        private void Deserialize()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ConfiguredButtons))
            {
                XmlSerializer xs = new XmlSerializer(typeof (ObservableCollection<JoyButton>));
                using (TextReader tr = new StringReader(Properties.Settings.Default.ConfiguredButtons))
                {
                    try
                    {
                        _lstJoyButtons = (ObservableCollection<JoyButton>) xs.Deserialize(tr);
                    }
                    catch (Exception)
                    {
                        _lstJoyButtons = new ObservableCollection<JoyButton>();
                    }
                }
            }
            else
            {
                _lstJoyButtons = new ObservableCollection<JoyButton>();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var pin = 0;
            var shouldReturn = false;
            if (string.IsNullOrEmpty(TbxPushMsg.Text))
            {
                TbxPushMsg.BorderBrush = Brushes.Red;
                shouldReturn = true;
            }
            if (string.IsNullOrEmpty(TbxReleaseMsg.Text))
            {
                TbxReleaseMsg.BorderBrush = Brushes.Red;
                shouldReturn = true;
            }
            if (string.IsNullOrEmpty(TbxPin.Text) || !int.TryParse(TbxPin.Text, out pin))
            {
                TbxPin.BorderBrush = Brushes.Red;
                shouldReturn = true;
            }
            
            if (shouldReturn)
            {
                return;
            }
            TbxPin.BorderBrush = Brushes.White;
            TbxPushMsg.BorderBrush = Brushes.White;
            TbxReleaseMsg.BorderBrush = Brushes.White;

            if ((from jb in _lstJoyButtons where jb.Pin == pin select jb).Any())
            {
                this.ShowMessageAsync("Error", "Pin is in use.");
                return;
            }

            var btn = new JoyButton(pin, TbxPushMsg.Text, TbxReleaseMsg.Text, CbReset.IsChecked != null && CbReset.IsChecked.Value, (uint) _lstJoyButtons.Count + 1);
            _lstJoyButtons.Add(btn);
            //LbxButtons.ItemsSource = _lstJoyButtons;
        }

        private void BtnRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (LbxButtons.SelectedIndex != -1)
            {
                JoyButton selected = (JoyButton) LbxButtons.SelectedItems[0];
                _lstJoyButtons.Remove(selected);
            }
        }

        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Serialize();
            int baudRate, deviceId;
            int.TryParse(TbxBaudRate.Text, out baudRate);
            Properties.Settings.Default.BaudRate = baudRate;
            int.TryParse(TbxDevice.Text, out deviceId);
            Properties.Settings.Default.Device = deviceId;
            Properties.Settings.Default.SerialPort = TbxSerialProt.Text;
            Properties.Settings.Default.Save();
        }

        private void Serialize()
        {
            var xmlSerializer = new XmlSerializer(_lstJoyButtons.GetType());
       
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, _lstJoyButtons);
                var x = textWriter.ToString();
                Properties.Settings.Default.ConfiguredButtons = x;
                Properties.Settings.Default.Save();
            }
        }

        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_started)
            {
                bool shouldReturn = false;
                int device = 0, baudRate = 0;
                if (string.IsNullOrEmpty(TbxDevice.Text) || !int.TryParse(TbxDevice.Text, out device))
                {
                    TbxDevice.BorderBrush = Brushes.Red;
                    shouldReturn = true;
                }
                if (string.IsNullOrEmpty(TbxBaudRate.Text) || !int.TryParse(TbxBaudRate.Text, out baudRate))
                {
                    TbxBaudRate.BorderBrush = Brushes.Red;
                    shouldReturn = true;
                }
                if (string.IsNullOrEmpty(TbxSerialProt.Text))
                {
                    TbxSerialProt.BorderBrush = Brushes.Red;
                    shouldReturn = true;
                }
                if (shouldReturn)
                    return;
                TbxSerialProt.BorderBrush = Brushes.White;
                TbxDevice.BorderBrush = Brushes.White;
                TbxBaudRate.BorderBrush = Brushes.White;


                _joy = new Joystick((uint) device);
                try
                {
                    _joy.CheckVJoy();
                }
                catch (Exception)
                {
                    //TODO:
                }

                if (!_joy.OpenSerialPort(baudRate, TbxSerialProt.Text))
                {
                    TbxSerialProt.BorderBrush = Brushes.Red;
                    TbxBaudRate.BorderBrush = Brushes.Red;
                    return;
                }
                _joy.Start(_lstJoyButtons);
                _started = true;
            }
            else
            {
                try
                {
                    _joy.CloseSerialPort();
                    _started = false;
                }
                catch (Exception)
                {
                    
                }
            }
        }

    }
}
