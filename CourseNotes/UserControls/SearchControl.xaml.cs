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
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Utility.Interfaces;

namespace CourseNotes.UserControls
{
    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl: UserControl, IDisplayFilterer
    {
        #region Constructors

        public SearchControl()
        {
            InitializeComponent();

            //Set each checkbox to call the FilterChanged event when they're checked / unchecked
            SubscribeToCheckBox(ShowAllCheckBox.CheckBoxState);
            SubscribeToCheckBox(LinkSearchCheckBox.CheckBoxState);


            //For better speed this should only be updated when input changes, but to keep it simple we call the event on each InputChanged
            WordSearchInput.InputChanged += (sender, args) => FilterChanged?.Invoke(this, EventArgs.Empty);
            TagSearchInput.InputChanged += (sender, args) => FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion Constructors

        #region Events

        public event EventHandler FilterChanged;

        #endregion Events

        #region Properties

        public bool DisplayAll => ShowAllCheckBox?.CheckBoxState.IsChecked.GetValueOrDefault(false) ?? false;

        public Action<IErrorMessage> DisplayErrorMessageAction { get; set; }

        public bool IncludeLinks => LinkSearchCheckBox?.CheckBoxState.IsChecked.GetValueOrDefault(false) ?? false;

        public string TagSearch => TagSearchInput?.InputText ?? "";

        public string WordSearch => WordSearchInput?.InputText ?? "";

        #endregion Properties

        #region Methods

        public void ResetState()
        {
            WordSearchInput.SetText("");
            TagSearchInput.SetText("");
            ShowAllCheckBox.CheckBoxState.IsChecked = false;
            LinkSearchCheckBox.CheckBoxState.IsChecked = false;
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            ResetState();
        }

        private void SubscribeToCheckBox(CheckBox aCheckBox)
        {
            aCheckBox.Checked += (sender, args) => FilterChanged?.Invoke(this, EventArgs.Empty);
            aCheckBox.Unchecked += (sender, args) => FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion Methods
    }
}