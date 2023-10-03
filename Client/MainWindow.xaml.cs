
using ChatBusinessTier;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using System.Windows.Shapes;
using System.Threading;

using System.Text.RegularExpressions;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatBusinessInterface server;
        public MainWindow()
        {

            InitializeComponent();
        }

        private async void OnClickLogin(object sender, RoutedEventArgs e)
        {
            string username = LoginInput.Text.ToString().Trim();



            if (validateStringInput(username))
            {
                string address = "net.tcp://localhost:8102/BusinessTier";
                ChatBusinessInterface server;
                ChannelFactory<ChatBusinessInterface> channelFactory;
                NetTcpBinding tcp = new NetTcpBinding();
                channelFactory = new ChannelFactory<ChatBusinessInterface>(tcp, address);
                server = channelFactory.CreateChannel();

                LoginText.Text = "Validating username...";

                bool validated = await Task.Run(() => server.Login(username));


                if (validated)
                { 
                    ChatroomScreen chatroom= new ChatroomScreen(username, server);
                    chatroom.Show();
                    Close();
                }
                else
                {
                    Modal modal = new Modal();
                    modal.setString("User already in session");
                    //Visibility = Visibility.Hidden;
                    modal.ShowDialog();
                }
            }
            else
            {
                Modal modal = new Modal();
                modal.setString("Username contains invalid characters.");
                modal.ShowDialog();
            }
        }


        private bool validateStringInput(string username)
        {
            const string pattern = @"^[a-zA-Z][a-zA-Z0-9]*$";
            return Regex.IsMatch(username, pattern);
        }

    }
}
