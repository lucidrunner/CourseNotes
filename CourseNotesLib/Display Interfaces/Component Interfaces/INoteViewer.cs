//Björn Rundquist 16/11-2020
using System;
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Display_Interfaces
{
    public interface INoteViewer : IDisplay
    {
        /// <summary>
        /// Invoked by the implementer when a note should be passed to the INotesPrinter, parameter is Note Id
        /// </summary>
        Action<int> AddNoteToPrintAction { get; set; }

        /// <summary>
        /// Invoked by the library when a note should be displayed
        /// </summary>
        Action<INote> ViewNoteAction { get; }

        /// <summary>
        /// Invoked by the library when the display needs to be cleared
        /// </summary>
        Action Reset { get; }
    }
}