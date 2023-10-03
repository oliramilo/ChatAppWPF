using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ChatDLL;
namespace Client
{
    /// <summary>
    /// Interaction logic for PrivateMessageBox.xaml
    /// </summary>
    public partial class PrivateMessageBox : Window
    {
        public PrivateMessageBox()
        {
            InitializeComponent();
        }

        public void setMessages(List<TextMessage> privateMessages)
        {
            foreach(TextMessage message in privateMessages) 
            {
                string msg = message.getSender() + ": " + message.getMessage() + ". Sent at: " + message.getTimeStamp() + "\n";
                PrivateMessages.Text += msg;
            }
            if( PrivateMessages.Text.Equals("")) 
            {
                PrivateMessages.Text = "No Private Messages.";
            }
        }
        
        private void OnClickClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
