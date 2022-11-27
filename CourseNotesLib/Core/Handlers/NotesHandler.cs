//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using System.Linq;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Domain_Classes;
using CourseNotesLib.Repository_Interfaces;
using CourseNotesLib.Utility;
using UtilityLibrary.Strings;

namespace CourseNotesLib.Core
{
    /// <summary>
    /// Handles Note interaction and manipulation
    /// </summary>
    internal class NotesHandler
    {
        #region Fields

        private readonly INotesPrinter notePrinter;

        private readonly INotesCreator notesCreator;

        private readonly INotesDisplay notesDisplay;

        private readonly INoteViewer noteViewer;

        private readonly INotesRepository repository;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Invoked when we want the id of the (possibly) currently displayed single module
        /// </summary>
        internal Func<int?> GetDisplayedModuleId { get; set; }

        /// <summary>
        /// Invoked when we want to convert a delimited tag input string to a collection of Tag Ids
        /// </summary>
        internal Converter<string, IEnumerable<int>> GetTagsFromInput { get; set; }

        /// <summary>
        /// Predicate for filtering notes, each note that don't match the predicate will be removed from display
        /// </summary>
        internal Predicate<Note> NoteFilterPredicate { get; set; }

        /// <summary>
        /// Invoked when we want to convert a series of tag IDs to a single delimited string
        /// </summary>
        internal Converter<IEnumerable<int>, string> TagIdConverter { get; set; }

        #endregion Properties

        #region Constructors

        //Considering the number of relevant interfaces this constructor is a bit messy
        internal NotesHandler(INotesDisplay notesDisplay, INotesCreator notesCreator, INoteViewer noteViewer, INotesPrinter notePrinter, INotesRepository repository)
        {
            this.notesDisplay = notesDisplay;
            this.notesCreator = notesCreator;
            this.repository = repository;
            this.noteViewer = noteViewer;
            this.notePrinter = notePrinter;


            LinkDisplay();
            LinkCreator();
            LinkViewer();
        }

        #endregion Constructors

        #region Events

        internal event EventHandler NoteModified;

        #endregion Events

        #region Methods

        internal void DisplayNotes(IEnumerable<int> aModuleIds)
        {
            var _moduleIdCollection = aModuleIds.ToList();

            //Get our base selection of notes
            var _moduleNotes = repository.Notes.Where(note => _moduleIdCollection.Contains(note.ModuleId)).ToList();

            //Then go through the optional filter predicates
            if(NoteFilterPredicate != null)
            {
                //For each predicate, only keep the notes that match it
                foreach(var _delegate in NoteFilterPredicate.GetInvocationList())
                {
                    var _predicate = (Predicate<Note>)_delegate;
                    _moduleNotes = _moduleNotes.Where(note => _predicate(note)).ToList();
                }
            }

            //Finally, attempt to fill the display with them
            notesDisplay.FillNotesList?.Invoke(_moduleNotes);
        }

        private bool CreateNewNote(bool aDisplayConnectionErrorMessage)
        {
            //We only display error messages for validation if the display layer is explicitly calling the ValidateInputFunc with
            //This is to avoid multiple error messages for the same externally / internal checks
            if(!ValidateNoteInput(false))
                return false;

            //Get the module id via the internal connection
            int? _moduleId = GetDisplayedModuleId?.Invoke();

            //If we don't have a module selected, or we don't have our internal collection setup
            //we automatically fail validation
            if(!_moduleId.HasValue)
            {
                if(aDisplayConnectionErrorMessage)
                    ErrorMessage.DisplayFor(notesCreator, "Connection Error", "No Module Selected", "A single module must be selected for a note to be added to it.");
                return false;
            }

            //Create and add the new note to the repository
            Note _newNote = new Note(repository.NewNoteId, _moduleId.Value);
            repository.Notes.Add(_newNote);
            //And fill the text fields and tags of it with creator properties
            FillNote(_newNote);

            return true;
        }

        private void FillNote(Note newNote)
        {
            //Set the texts to match the input strings
            newNote.Text = notesCreator.NoteText;
            newNote.Link = notesCreator.NoteLink;

            //Create or get the tag id's based on the input string
            var _tagIds = GetTagsFromInput?.Invoke(notesCreator.NoteTags);
            //On a valid return, add them to the note
            if(_tagIds != null)
                newNote.SetTags(_tagIds);

            NoteModified?.Invoke(this, EventArgs.Empty);
        }

        private Note GetNote(int noteId)
        {
            Note _modifiedNote = repository.Notes.FirstOrDefault(t => t.Id == noteId);
            return _modifiedNote;
        }

        private string GetTagStringFromId(int noteId)
        {
            Note _note = GetNote(noteId);
            if(_note == null)
                return "";

            //Use the internal converter to get the tag string representation
            string _tagString = TagIdConverter?.Invoke(_note.TagIds);
            //return either the converterd string or an empty string if conversion failed
            return _tagString ?? "";
        }

        private void LinkCreator()
        {
            if(notesCreator == null) return;

            notesCreator.ValidateInputFunc += ValidateNoteInput;
            notesCreator.CreateNote += CreateNewNote;
            notesCreator.GetTagString += GetTagStringFromId;
            notesCreator.ModifyNote += noteId =>
            {
                var _modifiedNote = GetNote(noteId);
                //Validate the input fields and make sure we've found the existing note
                if(!ValidateNoteInput(false) || _modifiedNote == null)
                    return false;
                //Fill the text and tags of the note with the creator properties
                FillNote(_modifiedNote);

                //Finally, tell the view window to display the updated note
                noteViewer?.ViewNoteAction?.Invoke(_modifiedNote);
                return true;
            };
        }

        private void LinkDisplay()
        {
            if(notesDisplay == null) return;

            notesDisplay.RemoveNote += id =>
            {
                if (repository.RemoveNote(id))
                {
                    notesDisplay.FillNotesList?.Invoke(repository.Notes);

                    //and clear the viewer
                    noteViewer?.Reset?.Invoke();
                }

            };
            notesDisplay.ViewNote += id =>
            {
               var _selectedNote = GetNote(id);
                //Attempt to display in the notes viewer as long as it's set and the action is hooked up correctly
                if(_selectedNote != null)
                   noteViewer?.ViewNoteAction?.Invoke(_selectedNote);
            };
        }
        private void LinkViewer()
        {
            //We need to make sure we also have a printer set before linking the viewer and printer together
            if(noteViewer == null || notePrinter == null) return;

            noteViewer.AddNoteToPrintAction += id =>
            {
                var _note = GetNote(id);
                if(_note != null)
                    notePrinter?.AddNoteToPrinter?.Invoke(_note);
            };
        }
        private bool ValidateNoteInput(bool aDisplayErrorMessage)
        {
            //If no note text is entered, invalidate due to that
            if(string.IsNullOrWhiteSpace(notesCreator.NoteText))
            {
                if(aDisplayErrorMessage)
                    ErrorMessage.DisplayFor(notesCreator, "Validation Error", "No Text Entered", "A text must be entered in the Note field.");
                return false;
            }

            //If tags have been entered, make sure they're correct
            if(!string.IsNullOrWhiteSpace(notesCreator.NoteTags) && Delimiters.DelimitType(InternalConstants.DelimitChar, notesCreator.NoteTags) != Delimiters.DelimitedType.Strings)
            {
                if(aDisplayErrorMessage)
                    ErrorMessage.DisplayFor(notesCreator, "Validation Error", "Invalid format for Tags", "Tags should only be entered as a series of strings like 'Tag 1; tag2; TAG 3' etc.");
                return false;
            }

            //Since links are optional we don't perform any validation on those
            return true;
        }

        #endregion Methods
    }
}