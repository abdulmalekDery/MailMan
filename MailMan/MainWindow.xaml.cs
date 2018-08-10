using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace MailMan
{
    public partial class MainWindow : MetroWindow
    {
        MailManager mailManager;
        public MainWindow()
        {
            InitializeComponent();
            Initialize(); 
        }

        private void Initialize()
        {
            mailManager = new MailManager(portNameComboBox, massegesPanel, massegeTextBox,
             connectionStatusTextBlock,this,buadRateComboBox,parityCaseComboBox,dataBitsComboBox,stopBitsComboBox);
        }
       

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            if (s.Content.Equals("Connect"))
            {
                mailManager.Connect();
                s.Content = "Disconnect";
            }
            else
            {
                mailManager.Disconnect();
                s.Content = "Connect";
            }

        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            mailManager.Send();
        }

        private void sendImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(op.FileName)); 
                mailManager.sendImage(bitmap,Path.GetExtension(op.FileName));
            }
        }
        
    }
}
