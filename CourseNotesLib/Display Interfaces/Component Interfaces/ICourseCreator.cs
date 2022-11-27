//Björn Rundquist 16/11-2020
using System;
using UtilityLibrary.Events;

namespace CourseNotesLib.Display_Interfaces
{
    /// <summary>
    /// Provides properties so the library can access the different course creation inputs, as well as an event that can be raised to validate & (possibly) create a new course
    /// </summary>
    public interface ICourseCreator: IDisplay
    {
        /// <summary>
        /// The event that is raised when the implementing class wants to create a course.
        /// Validates the properties and sets the event as handled on successful creation.
        /// </summary>
        event EventHandler<HandledEventArgs> CourseCreating;

        /// <summary>
        /// The name of the course
        /// </summary>
        string CourseName { get; }

        /// <summary>
        /// The optional url of the course
        /// </summary>
        string CourseLink { get; }

        /// <summary>
        /// The single string representation of the list of modules.
        /// </summary>
        string CourseModules { get; }
    }
}