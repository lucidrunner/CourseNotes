//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Display_Interfaces
{
    /// <summary>
    /// Provides delegates and events for the library to be able to fill a list of courses and respond to any selection
    /// </summary>
    public interface ICourseDisplay : IDisplay
    {
        /// <summary>
        /// Passes a simple representation of all courses in the form of tuples(Name, optional Link, Id) that can be used to fill the course list
        /// </summary>
        Action<IEnumerable<ICourse>> FillCourseListAction { get; set; }

        /// <summary>
        /// Should be raised when a new course is selected. Takes the Id of the course and fills the module display
        /// </summary>
        event EventHandler<int> CourseSelected;
    }
}