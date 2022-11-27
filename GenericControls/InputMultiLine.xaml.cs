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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenericControls
{
    /// <summary>
    /// Interaction logic for InputMultiLine.xaml
    /// </summary>
    public partial class InputMultiLine: UserControl
    {
        public InputMultiLine()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public string InputText => InputBox?.Text ?? "";

        public bool SetText(string aTextToSet)
        {
            if(InputBox == null) return false;
            InputBox.Text = aTextToSet;
            return true;
        }
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(InputMultiLine), new PropertyMetadata("Input Multiline Label"));



        public int Lines
        {
            get
            {
                return (int)GetValue(LinesProperty);
            }
            set
            {
                SetValue(LinesProperty, value);
            }
        }



        public static readonly DependencyProperty LinesProperty =
            DependencyProperty.Register("Lines", typeof(int), typeof(InputMultiLine), new PropertyMetadata(5));

    }
}
