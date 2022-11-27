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
using CourseNotesLib.Core;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Domain_Interfaces;
using CourseNotesLib.Utility.Interfaces;
using Microsoft.SqlServer.Server;

namespace CourseNotes.UserControls
{
    /// <summary>
    /// Interaction logic for ViewControl.xaml
    /// </summary>
    public partial class ViewControl: UserControl, INoteViewer, INoteConverter
    {
        #region Fields

        private INote currentlyViewedNote;

        #endregion Fields

        #region Constructors

        public ViewControl()
        {
            InitializeComponent();

            Reset += ResetState;
            ViewNoteAction += note =>
            {
                if(note == null) return;

                //Like for the print control, we use a conversion func to get a string representation of the passed in note
                FormattableString _printFormat = $"{NoteConversionParts.Course} - {NoteConversionParts.Module}\n{NoteConversionParts.Text}\nTags: {NoteConversionParts.Tags}\nLink: {NoteConversionParts.Link}";

                string _stringConversion = NoteToStringConverter?.Invoke(note, _printFormat);

                if(!string.IsNullOrWhiteSpace(_stringConversion))
                {
                    //Set the string to the view textbox
                    ViewTextBox.Text = _stringConversion;
                    //Save a reference to the current note so we can pass it to the printer later
                    currentlyViewedNote = note;
                }
            };
        }

        #endregion Constructors

        #region BLL Properties

        public Action<int> AddNoteToPrintAction { get; set; }
        public Action Reset { get; }
        public Action<IErrorMessage> DisplayErrorMessageAction { get; set; }
        public Func<IEnumerable<INote>, FormattableString, IEnumerable<string>> NotesToStringConverter { get; set; }
        public Func<INote, FormattableString, string> NoteToStringConverter { get; set; }
        public Action<INote> ViewNoteAction { get; set; }

        #endregion BLL Properties

        #region Methods

        public void ResetState()
        {
            currentlyViewedNote = null;
            ViewTextBox.Text = "";
        }

        private void Copy_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //We can only copy a link if we have a note with a link
            e.CanExecute = currentlyViewedNote != null && !string.IsNullOrWhiteSpace(currentlyViewedNote.Link);
        }

        private void Copy_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, (object)currentlyViewedNote.Link);
        }

        private void Print_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = currentlyViewedNote != null;
        }

        private void Print_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            AddNoteToPrintAction?.Invoke(currentlyViewedNote.Id);
        }

        #endregion Methods
    }
}