//Björn Rundquist 16-11-2020
using System;
using System.Collections.Generic;
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Display_Interfaces
{
    /// <summary>
    /// Interface for a control that displays a list of INotes
    /// </summary>
    public interface INotesDisplay : IDisplay
    {
        /// <summary>
        /// Raised by the control when a selected note should be viewed, parameter is note Id
        /// </summary>
        Action<int> ViewNote { get; set; }

        /// <summary>
        /// Raised by the control when a note should be removed, parameter is note Id
        /// </summary>
        Action<int> RemoveNote { get; set; }

        /// <summary>
        /// Invoked by the library to fill the Note List, the INoteConverter interface provides additional conversion methods for the INote collection
        /// </summary>
        Action<IEnumerable<INote>> FillNotesList { get; }
    }
}