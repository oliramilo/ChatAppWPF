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
    /// Interaction logic for Modal.xaml
    /// </summary>
    public partial class Modal : Window
    {
        public Modal()
        {
            InitializeComponent();
        }


        public void CloseModal_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        public void setString(string stringDescription)
        {
            ErrorDesc.Text = stringDescription;
        } 

    }
}
