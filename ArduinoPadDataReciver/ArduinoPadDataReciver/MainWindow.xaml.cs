using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using MahApps.Metro.Controls.Dialogs;

namespace ArduinoPadDataReciver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ArduinoReciver _ar;
        private ObservableCollection<JoyButton> _lstJoyButtons;
        private bool _started = false;

        public MainWindow()
        {
            InitializeComponent();

            Deserialize();
            TbxBaud.Text = Properties.Settings.Default.BaudRate.ToString();
            TbxDevice.Text = Properties.Settings.Default.Device.ToString();
            TbxPort.Text = Properties.Settings.Default.SerialPort;
            LbxControls.ItemsSource = _lstJoyButtons;
            _ar = new ArduinoReciver();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _ar.Stop();
            Serialize();
            int baud, device;
            int.TryParse(TbxBaud.Text, out baud);
            int.TryParse(TbxDevice.Text, out device);
            Properties.Settings.Default.BaudRate = baud;
            Properties.Settings.Default.Device = device;
            Properties.Settings.Default.SerialPort = TbxPort.Text;
            Properties.Settings.Default.Save();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!AddValidation())
            {
                return;
            }
            TbxPushMsg.BorderBrush = Brushes.White;
            TbxReleaseMsg.BorderBrush = Brushes.White;
            CbControl.BorderBrush = Brushes.White;
            if (CbControl.SelectedIndex == 1)
            {
                _lstJoyButtons.Add(new JoyButton(TbxPushMsg.Text, TbxReleaseMsg.Text, true, (uint)(_lstJoyButtons.Count + 1)));
            }
            if (CbControl.SelectedIndex == 0)
            {
                _lstJoyButtons.Add(new JoyButton(TbxPushMsg.Text, TbxReleaseMsg.Text, true, (uint)(_lstJoyButtons.Count + 1)));
            }
            LbxControls.ItemsSource = _lstJoyButtons;
        }

        private bool AddValidation()
        {
            bool validationSucces = true;
            //if (_lstJoyButtons.Count >= _ar.Buttons())
            //{
            //    this.ShowMessageAsync("You can't add new button!", "All buttons are in use.");
            //    return false;
            //}
            if (string.IsNullOrEmpty(TbxPushMsg.Text))
            {
                TbxPushMsg.BorderBrush = Brushes.Red;
                validationSucces = false;
            }
            if (string.IsNullOrEmpty(TbxReleaseMsg.Text))
            {
                TbxReleaseMsg.BorderBrush = Brushes.Red;
                validationSucces = false;
            }
            if (CbControl.SelectedIndex == -1)
            {
                CbControl.BorderBrush = Brushes.Red;
                validationSucces = false;
            }

            return validationSucces;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_started)
            {
                int device, baud;
                if (!StartValidation(out baud, out device))
                {
                    return;
                }
                TbxPort.BorderBrush = Brushes.White;
                TbxBaud.BorderBrush = Brushes.White;
                TbxDevice.BorderBrush = Brushes.White;
                try
                {
                    _ar.Start(_lstJoyButtons, baud, device, TbxPort.Text);
                }
                catch (Exception ex)
                {
                    this.ShowMessageAsync("Something went wrong :(", "Please, check a serial port settings.");
                    return;
                }
                _started = true;
                LblStarted.Visibility = Visibility.Visible;
            }
            else
            {
                _ar.Stop();
                _started = false;
                LblStarted.Visibility = Visibility.Hidden;
            }
        }

        private bool StartValidation(out int baud, out int device)
        {
            bool validationSucces = true;
            if (string.IsNullOrEmpty(TbxPort.Text))
            {
                TbxPort.BorderBrush = Brushes.Red;
                validationSucces = false;
            }
            if (!int.TryParse(TbxBaud.Text, out baud))
            {
                TbxBaud.BorderBrush = Brushes.Red;
                validationSucces = false;
            }
            if (!int.TryParse(TbxDevice.Text, out device))
            {
                TbxDevice.BorderBrush = Brushes.Red;
                validationSucces = false;
            }

            return validationSucces;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (LbxControls.SelectedIndex != -1)
            {
                var selected = (JoyButton)LbxControls.SelectedItems[0];
                _lstJoyButtons.Remove(selected);
            }
        }

        private void Serialize()
        {
            var xmlSerializer = new XmlSerializer(_lstJoyButtons.GetType());

            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, _lstJoyButtons);
                string x = textWriter.ToString();
                Properties.Settings.Default.ConfiguredButtons = x;
                Properties.Settings.Default.Save();
            }
        }

        private void Deserialize()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ConfiguredButtons))
            {
                var xs = new XmlSerializer(typeof(ObservableCollection<JoyButton>));
                using (TextReader tr = new StringReader(Properties.Settings.Default.ConfiguredButtons))
                {
                    try
                    {
                        _lstJoyButtons = (ObservableCollection<JoyButton>)xs.Deserialize(tr);
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
    }
}
