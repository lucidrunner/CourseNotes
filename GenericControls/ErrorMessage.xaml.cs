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

namespace GenericControls
{
    /// <summary>
    /// Interaction logic for ErrorMessage.xaml
    /// </summary>
    public partial class ErrorMessage: Window
    {
        public ErrorMessage(string aTitle, string aHeader, string aMessage)
        {
            InitializeComponent();

            Title = aTitle;
            Header.Text = aHeader;
            Message.Text = aMessage;
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
