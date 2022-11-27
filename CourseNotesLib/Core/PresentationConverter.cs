//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Domain_Classes;
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Core
{
    public enum NoteConversionParts
    {
        Text, Tags, Link, Module, Course
    }

    /// <summary>
    /// Provides conversion capabilities from the different domain interfaces (currently only INote) to string representations. This is done via argument replacements for FormattedStrings
    /// </summary>
    internal class PresentationConverter
    {
        #region Fields

        internal Converter<IEnumerable<int>, string> TagIdConverter;
        private readonly Repository repository;

        #endregion Fields

        #region Constructors

        internal PresentationConverter(Repository aRepository)
        {
            repository = aRepository;
        }

        #endregion Constructors

        #region Methods

        internal void LinkPresentationComponent(object aComponent)
        {
            //Obviously this is where you'd also have things like an ICourseConverter, IModuleConverter etc but I don't really need them in this project so it would
            //only be a bit of extra clutter to implement

            if(aComponent is INoteConverter _noteConverter)
            {
                _noteConverter.NoteToStringConverter += NoteToStringConverter;
                _noteConverter.NotesToStringConverter += NotesToStringConverter;
            }
        }

        #endregion Methods

        #region Note Conversions

        private IEnumerable<string> NotesToStringConverter(IEnumerable<INote> aNotes, FormattableString aConversionFormat)
        {
            //For each of the provided notes, call the single converter to replace them with strings
            List<string> _convertedStrings = new List<string>();

            foreach(Note _note in aNotes.Cast<Note>())
            {
                string _convertedString = NoteToStringConverter(_note, aConversionFormat);
                _convertedStrings.Add(!string.IsNullOrWhiteSpace(_convertedString) ? _convertedString : "");
            }

            return _convertedStrings;
        }

        private string NoteToStringConverter(INote aNote, FormattableString aConversionFormat)
        {
            Note _note = (Note)aNote;
            if(_note == null)
                return "";

            //Get the arguments that we'll replace in the formattable string
            var _conversionParts = aConversionFormat.GetArguments();
            //And create a separate list for the objects we're replacing the arguments with
            List<object> _replacementStrings = new List<object>();

            //Get the module & course for the note
            Module _noteModule = GetModuleFor(_note.ModuleId);
            Course _noteCourse = _noteModule != null ? GetCourseFor(_noteModule.CourseId) : null;

            //Go through the parts, adding the correct replacement for each of them
            foreach(var _part in _conversionParts)
            {
                string _replacement = null;
                switch(_part)
                {
                    case NoteConversionParts.Text:
                        _replacement = _note.Text;
                        break;

                    case NoteConversionParts.Course when _noteCourse != null:
                        _replacement = _noteCourse.Name;
                        break;

                    case NoteConversionParts.Module when _noteModule != null:
                        _replacement = _noteModule.ModuleName;
                        break;

                    case NoteConversionParts.Link:
                        _replacement = _note.Link;
                        break;

                    case NoteConversionParts.Tags:
                        _replacement = TagIdConverter?.Invoke(_note.TagIds);
                        break;
                }

                _replacementStrings.Add(_replacement ?? "");
            }

            return FormattableStringFactory.Create(aConversionFormat.Format, _replacementStrings.ToArray()).ToString();
        }
        #endregion Note Conversions

        #region Shorthand Properties

        private Course GetCourseFor(int aCourseId) => repository.Courses.FirstOrDefault(t => t.Id == aCourseId);

        private Module GetModuleFor(int aModuleId) => repository.Modules.FirstOrDefault(t => t.Id == aModuleId);

        #endregion Shorthand Properties
    }
}