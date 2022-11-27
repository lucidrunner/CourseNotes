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
    /// Interaction logic for LabeledCheckBox.xaml
    /// </summary>
    public partial class LabeledCheckBox: UserControl
    {
        public CheckBox CheckBoxState => CheckBox;
        


        public LabeledCheckBox()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
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
            DependencyProperty.Register("Label", typeof(string), typeof(LabeledCheckBox), new PropertyMetadata("CheckBox"));
    }
}
