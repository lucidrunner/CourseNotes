//Björn Rundquist 16/11-2020
using System;

namespace CourseNotesLib.Display_Interfaces
{
    /// <summary>
    /// Provides different search filtering / display properties
    /// </summary>
    public interface IDisplayFilterer : IDisplay
    {
        /// <summary>
        /// If we should currently display all notes, or only the ones from the selected modules
        /// </summary>
        bool DisplayAll { get; }

        /// <summary>
        /// The current input of text to the word search
        /// </summary>
        string WordSearch { get; }

        /// <summary>
        /// The current input to the tag search. Should be provided as a delimited string (ie a;b;c)
        /// </summary>
        string TagSearch { get; }

        /// <summary>
        /// If we should include link text in the word searches
        /// </summary>
        bool IncludeLinks { get; }

        /// <summary>
        /// Should be raised whenever the display / search options have changed.
        /// </summary>
        event EventHandler FilterChanged;
    }
}