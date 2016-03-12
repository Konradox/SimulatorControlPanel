using System;
using System.Collections.Generic;
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
        private ObservableCollection<JoyControl> _lstJoyButtons;
        private bool _started = false;
        private Stack<int> _freeButtonNumbers;

        public MainWindow()
        {
            InitializeComponent();

            Deserialize();
            TbxBaud.Text = Properties.Settings.Default.BaudRate.ToString();
            TbxDevice.Text = Properties.Settings.Default.Device.ToString();
            TbxPort.Text = Properties.Settings.Default.SerialPort;
            LbxControls.ItemsSource = _lstJoyButtons;
            _ar = new ArduinoReciver();
            _freeButtonNumbers = new Stack<int>();
            for (int i = _ar.Buttons(); i > 0; i--)
            {
                var add = true;
                foreach (JoyControl lstJoyButton in _lstJoyButtons)
                {
                    if (lstJoyButton.Button == i)
                        add = false;
                }
                if (add)
                    _freeButtonNumbers.Push(i);
            }
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
                if (string.IsNullOrEmpty(TbxReleaseMsg.Text))
                    _lstJoyButtons.Add(new JoySwitch(TbxPushMsg.Text, TbxPushMsg.Text, (uint)_freeButtonNumbers.Pop()));
                else
                    _lstJoyButtons.Add(new JoySwitch(TbxPushMsg.Text, TbxReleaseMsg.Text, (uint)_freeButtonNumbers.Pop()));

            }
            if (CbControl.SelectedIndex == 0)
            {
                _lstJoyButtons.Add(new JoyButton(TbxPushMsg.Text, TbxReleaseMsg.Text, (uint)_freeButtonNumbers.Pop()));
            }
            LbxControls.ItemsSource = _lstJoyButtons;
        }

        private bool AddValidation()
        {
            bool validationSucces = true;
            if (_freeButtonNumbers.Count == 0)
            {
                this.ShowMessageAsync("You can't add new button!", "All buttons are in use.");
                return false;
            }
            if (string.IsNullOrEmpty(TbxPushMsg.Text))
            {
                TbxPushMsg.BorderBrush = Brushes.Red;
                validationSucces = false;
            }
            if (string.IsNullOrEmpty(TbxReleaseMsg.Text) && CbControl.SelectedIndex != 1 && CbControl.SelectedIndex != 3)
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
                _freeButtonNumbers.Push((int) selected.Button);
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
                var xs = new XmlSerializer(typeof(ObservableCollection<JoyControl>));
                using (TextReader tr = new StringReader(Properties.Settings.Default.ConfiguredButtons))
                {
                    try
                    {
                        _lstJoyButtons = (ObservableCollection<JoyControl>)xs.Deserialize(tr);
                    }
                    catch (Exception)
                    {
                        _lstJoyButtons = new ObservableCollection<JoyControl>();
                    }
                }
            }
            else
            {
                _lstJoyButtons = new ObservableCollection<JoyControl>();
            }
        }

        private void CbControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LblMsg2.Visibility = Visibility.Visible;
            TbxReleaseMsg.Visibility = Visibility.Visible;
            if (CbControl.SelectedIndex == 0)
            {
                LblMsg1.Content = "Push message";
                LblMsg2.Content = "Release message";
            }
            if (CbControl.SelectedIndex == 1)
            {
                LblMsg1.Content = "Message 1";
                LblMsg2.Content = "Message 2 (optional)";
            }
            if (CbControl.SelectedIndex == 2)
            {
                LblMsg1.Content = "Increment message";
                LblMsg2.Content = "Decrement message";
            }
            if (CbControl.SelectedIndex == 3)
            {
                LblMsg1.Content = "Value changed message";
                LblMsg2.Visibility = Visibility.Hidden;
                TbxReleaseMsg.Visibility = Visibility.Hidden;
            }
        }
    }
}
