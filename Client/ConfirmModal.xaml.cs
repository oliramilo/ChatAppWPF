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

namespace Client
{
    /// <summary>
    /// Interaction logic for ConfirmModal.xaml
    /// </summary>
    public partial class ConfirmModal : Window
    {

        public bool confirmed;
        public string displayMessage;
        public ConfirmModal(string displayMessage)
        {
            this.displayMessage = displayMessage;
            InitializeComponent();
            MessageDialogBox.Text = displayMessage;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            confirmed = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            confirmed = false;
            Close();
        }
    }
}
