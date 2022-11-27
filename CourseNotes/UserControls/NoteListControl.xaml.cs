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
using CourseNotesLib.Utility;
using CourseNotesLib.Utility.Interfaces;

namespace CourseNotes.UserControls
{
    /// <summary>
    /// Interaction logic for NotesControl.xaml
    /// </summary>
    public partial class NotesControl: UserControl, INotesDisplay, INoteConverter
    {
        #region Fields

        //The backing list for the current display, allows us to pass the correct
        private readonly List<INote> displayedNotes = new List<INote>();
        #endregion Fields

        #region Constructors

        public NotesControl()
        {
            InitializeComponent();

            FillNotesList += notes =>
            {
                if(notes == null) return;

                //Pass the output format we want to the conversion method
                FormattableString _printFormat = $"{NoteConversionParts.Text}\nTags: {NoteConversionParts.Tags}";
                //We're casting the IEnumerable to a list since we both need to pass it to the converter and (possibly) add it to the backing list
                var _collection = notes.ToList();

                var _stringConversion = NotesToStringConverter?.Invoke(_collection, _printFormat);

                //If we don't have the converter set or it returned null, don't attempt to set the collection
                if (_stringConversion == null)
                    return;

                //Replace the current backing list & set the converted strings as our display source
                displayedNotes.Clear();
                displayedNotes.AddRange(_collection);
                NotesList.ItemsSource = _stringConversion;
            };

            NotesList.SelectionChanged += (sender, args) =>
            {
                INote _selectedNote = GetSelectedNote();
                if(_selectedNote != null)
                    ViewNote?.Invoke(_selectedNote.Id);
            };
        }

        #endregion Constructors

        #region Properties

        #region BLL delegates
        public Action<IErrorMessage> DisplayErrorMessageAction { get; set; }
        public Action<IEnumerable<INote>> FillNotesList { get; set; }
        public Func<IEnumerable<INote>, FormattableString, IEnumerable<string>> NotesToStringConverter { get; set; }
        public Func<INote, FormattableString, string> NoteToStringConverter { get; set; }
        public Action<int> RemoveNote { get; set; }
        public Action<int> ViewNote { get; set; }
        #endregion BLL delegates

        /// <summary>
        /// Called when we want to modify a selected note
        /// </summary>
        internal Action<INote> ModifyNote { get; set; }

        #endregion Properties

        #region Methods

        public void ResetState()
        {
            displayedNotes.Clear();
            NotesList.ItemsSource = null;
            NotesList.SelectedItem = null;
        }

        private void DeleteNote_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Using this kind of interface / handler pattern that I'm doing in this project we basically got two modes / flows of interaction
            //We can do an action/event and then do a callback action, letting the BLL control the flow (in this case, pass the Id through the action/event and then call the FillNotesListAction with out new collection after deleting)
            //Or we can do a func/event, directly respond through that and keep the flow in the display class (using something like the HandledEventArgs and setting the handled flag or a bool return as with this func)
            //I've consciously used the different ways in different classes in this project to test them out

            int _index = NotesList.SelectedIndex;
            //If we successfully delete the note, remove it from our backing list and recreate the itemsource for the listbox
            RemoveNote?.Invoke(displayedNotes[_index].Id);
        }

        private INote GetSelectedNote()
        {
            //Since the notes itemsource is derived from the displayedNotes-list we can access the displayedNotes via the selected index
            if(NotesList.SelectedIndex >= 0 && NotesList.SelectedIndex < displayedNotes.Count)
                return displayedNotes[NotesList.SelectedIndex];

            return null;
        }

        private void ModifyNote_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //If we have a note selected, call the internally settable action to pass it on to the Modify-control
            INote _selectedNote = GetSelectedNote();
            if(_selectedNote != null)
                ModifyNote?.Invoke(_selectedNote);
        }

        //Generic OnCanExecute requiring us to have a note selected
        private void Selected_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            INote _selectedNote = GetSelectedNote();
            if(_selectedNote != null)
                e.CanExecute = true;
        }

        #endregion Methods
    }
}