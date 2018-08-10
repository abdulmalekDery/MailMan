using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MailMan
{
    public class MailManager
    {
        public SerialPort port;
        private ComboBox portNameComboBox;
        private StackPanel massegePanel;
        private TextBox massegeTextBox;
        private TextBlock connectStatusTextBlock;
        private MainWindow mainWindow;
        private ComboBox buadRate;
        private ComboBox parity;
        private ComboBox dataBits;
        private ComboBox stopBits;

        public MailManager(ComboBox c, StackPanel mp, TextBox m, TextBlock s, MainWindow main
              ,ComboBox boudRate,ComboBox parity,ComboBox dataBits,ComboBox stopBits)
        {
            portNameComboBox = c;
            massegePanel = mp;
            massegeTextBox = m;
            connectStatusTextBlock = s;
            mainWindow = main;
            this.buadRate = boudRate;
            this.parity = parity;
            this.dataBits = dataBits;
            this.stopBits = stopBits;
            Initialize();
        }

        private void Initialize()
        {
            port = new SerialPort();
            string[] portNames = SerialPort.GetPortNames();
            for (int i = 0; i < portNames.Length; i++)
            {
                portNameComboBox.Items.Add(portNames[i]);
            }
            buadRate.Items.Add("2400");
            buadRate.Items.Add("4800");
            buadRate.Items.Add("9600");
            buadRate.Items.Add("19200");
            buadRate.Items.Add("38400");
            buadRate.Items.Add("115200");

            parity.Items.Add("None");
            parity.Items.Add("Odd");
            parity.Items.Add("Even");
            parity.Items.Add("Mark");
            parity.Items.Add("Space");

            dataBits.Items.Add("5");
            dataBits.Items.Add("6");
            dataBits.Items.Add("7");
            dataBits.Items.Add("8");

            stopBits.Items.Add("One");
            stopBits.Items.Add("Two");
            stopBits.Items.Add("OnePointFive");

            portNameComboBox.SelectedIndex = 0;
            buadRate.SelectedIndex = 2;
            parity.SelectedIndex = 0;
            dataBits.SelectedIndex = 3;
            stopBits.SelectedIndex = 1;
            port.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataReceived);

        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string dataReceived = port.ReadLine();
                if (dataReceived.Substring(0, 8).Equals("notImage"))
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        CreateTextBlockAndAddItToTheGrid(dataReceived.Substring(8),1);
                    });
                else {
                    byte[] byteBuffer = Convert.FromBase64String(dataReceived);

                    ImageSource imageSource = ToImage(byteBuffer);
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        CreateImageAndAddItToTheGrid(imageSource);
                    });
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }



        private ImageSource ToImage(byte[] array)
        {
            ImageSource im = (ImageSource)new ImageSourceConverter().ConvertFrom(array);
            return im;
        }

        private void CreateTextBlockAndAddItToTheGrid(string dataReceived,int dir)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = dataReceived;
            textBlock.FontSize = 16;
            if (dir == 1)
                textBlock.TextAlignment = TextAlignment.Left;
            else
                textBlock.TextAlignment = TextAlignment.Right;
            massegePanel.Children.Add(textBlock);
        }

        public void CreateImageAndAddItToTheGrid(ImageSource imageSource)
        {
            Image image = new Image();
            image.Source = imageSource;
            image.Height = 250;
            image.Width = 350;
            image.Margin = new Thickness(12, 12, 16, 71);
            massegePanel.Children.Add(image);
        }

        public void CreateSendImageAndAddItToTheGrid(BitmapImage imageSource)
        {
            Image image = new Image();
            image.Source = imageSource;
            image.Height = 250;
            image.Width = 350;
            image.Margin = new Thickness(12, 12, 16, 71);
            massegePanel.Children.Add(image);
        }

        public void sendImage(BitmapImage bitmapImage, string ext)
        {
            byte[] data;
            if (String.Compare(ext, ".png", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                PngBitmapEncoder encoder1 = new PngBitmapEncoder();
                encoder1.Frames.Add(BitmapFrame.Create(bitmapImage));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder1.Save(ms);
                    data = ms.ToArray();
                }
            }
            else
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
            }



            CreateSendImageAndAddItToTheGrid(bitmapImage);
            string base64String = Convert.ToBase64String(data);
            port.WriteLine(base64String);


        }
    

        public void Send()
        {
            try
            {
                port.WriteLine(Convert.ToString("notImage"+massegeTextBox.Text));
                CreateTextBlockAndAddItToTheGrid(massegeTextBox.Text,2);
                massegeTextBox.Text = String.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Connect()
        {
           
            if (port.IsOpen)
            {
                port.Close();
            }
            try
            {
                port.PortName = portNameComboBox.Text;
                port.BaudRate = int.Parse(buadRate.Text);

               string str = "";
                str = stopBits.Text.ToLower();
                if (str.Equals("one"))
                    port.StopBits = System.IO.Ports.StopBits.One;
                else if (str.Equals("two"))
                    port.StopBits = System.IO.Ports.StopBits.Two;
                else if (str.Equals("onepointfive"))
                    port.StopBits = System.IO.Ports.StopBits.OnePointFive;

                port.DataBits = int.Parse(dataBits.Text);

                string str1 = parity.Text.ToLower();
                if (str1.Equals("none"))
                    port.Parity = System.IO.Ports.Parity.None;
                else if (str1.Equals("even"))
                    port.Parity = System.IO.Ports.Parity.Even;
                else if (str1.Equals("odd"))
                    port.Parity = System.IO.Ports.Parity.Odd;
                else if (str1.Equals("mark"))
                    port.Parity = System.IO.Ports.Parity.Mark;
                else if (str1.Equals("space"))
                    port.Parity = System.IO.Ports.Parity.Space;

                port.Encoding = System.Text.Encoding.Unicode;
                port.RtsEnable = false;
                port.DiscardNull = false;
                port.DtrEnable = false;
                port.Handshake = Handshake.None;
                port.ParityReplace = 63;
                port.ReadBufferSize = 4096;
                port.ReadTimeout = -1;
                port.ReceivedBytesThreshold = 1;
                port.WriteBufferSize = 2098;
                port.WriteTimeout = -1;
                port.Open();
                connectStatusTextBlock.Text = portNameComboBox.Text + " Is Connected.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        public void Disconnect()
        {
            try
            {
                port.Close();
                connectStatusTextBlock.Text = portNameComboBox.Text + " Is Disconnected.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

       

    }
}
