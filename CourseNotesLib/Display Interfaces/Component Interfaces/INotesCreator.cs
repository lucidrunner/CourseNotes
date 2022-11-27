//Björn Rundquist 16/11-2020
using System;

namespace CourseNotesLib.Display_Interfaces
{
    /// <summary>
    /// Provides delegates to create or modifiy notes, as well as properties so the library can read the different input fields
    /// </summary>
    public interface INotesCreator : IDisplay
    {
        string NoteText { get; }
        string NoteLink { get; }

        string NoteTags { get; }

        
        /// <summary>
        /// Validates the current intput properties. Takes a bool to determine if it should attempt to display an error message on invalid result.
        /// </summary>
        Func<bool,bool> ValidateInputFunc { get; set; }

        /// <summary>
        /// Creates a new note based on the current input. Takes a bool to determine if it should show an error message if no or several modules are selected.
        /// </summary>
        Func<bool, bool> CreateNote { get; set; }

        /// <summary>
        /// Modifies the note based on the provided note Id.
        /// </summary>
        Func<int, bool> ModifyNote { get; set; }

        /// <summary>
        /// Returns a delimited string of the tags based on the provided note Id.
        /// </summary>
        Func<int, string> GetTagString { get; set; }
    }
}