//Björn Rundquist 16/11-2020
using System;
using CourseNotesLib.Domain_Classes;
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Display_Interfaces
{
    /// <summary>
    /// Interface for a control that shows a manual selection of notes. Invoked from the INoteViewer.
    /// </summary>
    /// This is a bit uneccesary since it could have been fully handled via actions in the presentation layer instead. This is a leftover from before i used INote to pass along
    /// notes which meant the INoteViewer needed to send a View command with Id attached for this to get a printable object
    public interface INotesPrinter
    {
        Action<INote> AddNoteToPrinter { get;  }
    }
}