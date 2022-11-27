//Björn Rundquist 16/11-2020
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
    /// Interaction logic for InputField.xaml
    /// </summary>
    public partial class InputField: UserControl
    {
        //Fairly straightforward TextBox + label control, using a registered dependency property to be able to set the label in xaml
        #region Fields

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(InputField), new PropertyMetadata("Input Label"));

        #endregion Fields

        #region Constructors

        public InputField()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;

            InputBox.TextChanged += (sender, args) =>
            {
                InputChanged?.Invoke(this, args);
            };
        }

        #endregion Constructors

        #region Events

        public event EventHandler InputChanged;

        #endregion Events

        #region Properties

        public string InputText => InputBox?.Text ?? "";

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        #endregion Properties

        #region Methods

        public void ClearInput()
        {
            if(InputBox != null)
                InputBox.Text = "";
        }

        public bool SetText(string aTextToSet)
        {
            if(InputBox == null) return false;
            InputBox.Text = aTextToSet;
            return true;
        }

        #endregion Methods
    }
}