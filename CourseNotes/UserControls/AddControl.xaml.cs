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
using CourseNotesLib.Domain_Interfaces;
using CourseNotesLib.Utility;
using CourseNotesLib.Utility.Interfaces;

namespace CourseNotes.UserControls
{
    /// <summary>
    /// Interaction logic for AddControl.xaml
    /// </summary>
    public partial class AddControl: UserControl, INotesCreator
    {
        #region Fields

        /// <summary>
        /// The currently modified note, if set the AddControl acts as a modifier for that rather than a creator of new notes
        /// </summary>
        private INote modifiedNote;

        #endregion Fields

        #region Constructors

        public AddControl()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public Func<bool, bool> CreateNote { get; set; }

        public Func<int, string> GetTagString { get; set; }

        public Func<int, bool> ModifyNote { get; set; }

        public string NoteLink => LinkInput?.InputText ?? "";

        public string NoteTags => TagsInput?.InputText ?? "";

        public string NoteText => NoteInput?.InputText ?? "";

        public Func<bool,bool> ValidateInputFunc { get; set; }

        public Action<IErrorMessage> DisplayErrorMessageAction { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Internally called when we want to modify a note
        /// </summary>
        public void SetModifiedNote(INote aNoteToModify)
        {
            //Set the backing note as well as the different input fields
            modifiedNote = aNoteToModify;
            NoteInput.SetText(aNoteToModify.Text);
            LinkInput.SetText(aNoteToModify.Link);
            TagsInput.SetText(GetTagString?.Invoke(aNoteToModify.Id) ?? "");
            //and change the header to read modify instead of add
            HeaderBox.Header = "Modify";
        }
        private void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            ResetState();
        }

        public void ResetState()
        {
            modifiedNote = null;
            NoteInput.SetText("");
            LinkInput.SetText("");
            TagsInput.SetText("");

            HeaderBox.Header = "Add";
        }

        private void Save_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ValidateInputFunc?.Invoke(true) ?? false)
                return;

            //Depending on if we have a backing note or not, we modify that or attempt to create a new note
            bool _saved;
            if(modifiedNote != null)
            {
                _saved = ModifyNote?.Invoke(modifiedNote.Id) ?? false;
            }
            else
            {
                _saved = CreateNote?.Invoke(true) ?? false;
            }

            //If we successfully modified / created, reset the control
            if(_saved)
                ResetState();
        }

        #endregion Methods

    }
}