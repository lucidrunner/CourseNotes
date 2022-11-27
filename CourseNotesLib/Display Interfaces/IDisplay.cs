
//Björn Rundquist 16/11-2020
using System;
using CourseNotesLib.Utility.Interfaces;

namespace CourseNotesLib.Display_Interfaces
{
    /// <summary>
    /// Top level interface for display components, provides a delegate for the library to send an IErrorMessage on errors
    /// </summary>
    public interface IDisplay
    {
        Action<IErrorMessage> DisplayErrorMessageAction { get; }

    }
}