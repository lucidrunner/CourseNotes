//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CourseNotesLib.Core;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotes.UserControls
{
    /// <summary>
    /// Interaction logic for PrintControl.xaml
    /// </summary>
    public partial class PrintControl: UserControl, INotesPrinter, INoteConverter
    {
        #region Fields

        //Since we're adding to this print list over time we use an observable collection as our backing list and continually add new strings to it
        private readonly ObservableCollection<string> currentPrint = new ObservableCollection<string>();

        #endregion Fields

        #region Constructors

        public PrintControl()
        {
            InitializeComponent();
            NotesList.ItemsSource = currentPrint;

            //On the add action, use the conversion func to get a string we can add to the pring window
            AddNoteToPrinter += note =>
            {
                if(note == null) return;

                FormattableString _printFormat = $"{NoteConversionParts.Text}\nTags: {NoteConversionParts.Tags}\n{NoteConversionParts.Link}";

                string _stringConversion = NoteToStringConverter?.Invoke(note, _printFormat);

                if(!string.IsNullOrWhiteSpace(_stringConversion))
                {
                    //If we had no link we trim it to remove an empty line from the print window
                    _stringConversion = _stringConversion.TrimEnd('\n');
                    currentPrint.Add(_stringConversion);
                }
            };
        }

        #endregion Constructors

        #region Properties

        public Action<INote> AddNoteToPrinter { get; }
        public Func<IEnumerable<INote>, FormattableString, IEnumerable<string>> NotesToStringConverter { get; set; }
        public Func<INote, FormattableString, string> NoteToStringConverter { get; set; }

        #endregion Properties

        #region Methods

        public void ResetState()
        {
            currentPrint.Clear();
        }

        private void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            ResetState();
        }

        private void Copy_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = currentPrint.Count > 0;
        }

        private void Copy_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, string.Join("\n", currentPrint));
        }

        #endregion Methods
    }
}