//Björn Rundquist 16/11-2020
using System;
using System.IO;

namespace CourseNotesLib.Display_Interfaces
{
    public interface INoteTakerInterface
    {
        #region NoteTaker Delegates

        /// <summary>
        /// Invoked when we want to load our collections from the selected XML file
        /// </summary>
        Func<FileInfo, bool> LoadFromFile { get; set; }

        /// <summary>
        /// Invoked when we want to save to a provided XML file
        /// </summary>
        Func<FileInfo, bool> SaveToFile { get; set; }

        /// <summary>
        /// Invoked when we want to save a new file (with the provided name) to the provided Directory
        /// </summary>
        Func<DirectoryInfo, string, FileInfo> SaveAs { get; set; }

        /// <summary>
        /// Invoked by the library when we need to clear all controls & selections (ie after a successful load)
        /// </summary>
        Action ResetDisplay { get; }

        #endregion

        #region Implementing Components

        //All these provide access for the library to setup links with the different components on creation and need to be set for full functionality to work

        ICourseCreator CourseCreator { get; }
        ICourseDisplay CourseDisplay { get; }
        IModuleDisplay ModuleDisplay { get; }
        INotesDisplay NotesDisplay { get; }
        INotesCreator NotesCreator { get; }

        INoteViewer NoteViewer { get; }

        INotesPrinter NotePrinter { get; }

        IDisplayFilterer DisplayFilterer { get; }

        #endregion
    }
}