//Björn Rundquist 16/11-2020
using System.Collections.Generic;
using CourseNotesLib.Domain_Classes;

namespace CourseNotesLib.Repository_Interfaces
{
    internal interface INotesRepository
    {
        /// <summary>
        /// Returns the latest unique Id for a note, will increment when a new note is added
        /// </summary>
        int NewNoteId { get; }

        /// <summary>
        /// The list of notes in repository
        /// </summary>
        List<Note> Notes { get; }

        /// <summary>
        /// Removes a note with the provided id from the repository ad cleans up any lingering tags
        /// </summary>
        bool RemoveNote(int aId);
    }
}