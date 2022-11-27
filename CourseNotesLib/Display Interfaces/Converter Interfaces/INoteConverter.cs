
//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using CourseNotesLib.Core;
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Display_Interfaces
{
    /// <summary>
    /// Provides Note display conversion delegates that will be bound by the library on startup
    /// </summary>
    /// The name is a bit bad since it technically isn't a Converter but rather a ConversionReciever or something like that
    public interface INoteConverter
    {
        Func<INote, FormattableString, string> NoteToStringConverter { get; set; }
        Func<IEnumerable<INote>, FormattableString, IEnumerable<string>> NotesToStringConverter { get; set; }
    }
}