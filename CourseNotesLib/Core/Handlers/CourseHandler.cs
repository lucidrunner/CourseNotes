//Björn Rundquist 16/11-2020
using System;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Domain_Classes;
using CourseNotesLib.Repository_Interfaces;
using CourseNotesLib.Utility;
using UtilityLibrary.Strings;

namespace CourseNotesLib.Core
{
    internal class CourseHandler
    {
        #region Fields

        //Internal delegates
        internal Action<string, int> AddModules;
        internal Action<int> DisplayModules;

        //Presentation layer
        private readonly ICourseCreator courseCreator;
        private readonly ICourseDisplay courseDisplay;

        
        private readonly ICourseRepository repository;

        #endregion Fields

        #region Constructors

        internal CourseHandler(ICourseCreator courseCreator, ICourseDisplay courseDisplay, ICourseRepository repository)
        {
            this.courseCreator = courseCreator;
            this.courseDisplay = courseDisplay;
            this.repository = repository;

            SetupCreatorEvents(courseCreator);
            SetupDisplayEvents(courseDisplay);
        }

        #endregion Constructors

        #region Methods

        internal void UpdateCourseDisplayList()
        {
            if(repository == null || courseDisplay?.FillCourseListAction == null)
                return;

            //Pass all courses to the display
            courseDisplay.FillCourseListAction(repository.Courses);

            /* Original implementation, abandoned this for domain interfaces instead
            //Project the courses to a simple representation using tuples
            var _courseRepresentations = repository.Courses.Select(t => new Tuple<string, string, int>(t.Name,  t.Link, t.Id));

            //Then use the Fill action in the display interface to pass the tuples to the display class
            courseDisplay.FillCourseListAction(_courseRepresentations);
            */
        }

        private void CreateCourse(ICourseCreator aCourseCreator)
        {
            //Create the course using the provided information
            string _name = aCourseCreator.CourseName;
            string _link = aCourseCreator.CourseLink ?? "";

            Course _newCourse = new Course(repository.Courses.Count, _name, _link);

            //Add it to our simple repository
            repository.Courses.Add(_newCourse);

            //And use our internally settable action to create the modules
            AddModules?.Invoke(aCourseCreator.CourseModules, _newCourse.Id);

            //Finally, update the course display list
            UpdateCourseDisplayList();
        }

        private void SetupCreatorEvents(ICourseCreator aCourseCreator)
        {
            //Add a listener to the course creation event
            aCourseCreator.CourseCreating += (sender, args) =>
            {
                //If the sender is valid, create the course and set it as handled
                if(sender is ICourseCreator _courseCreator && ValidateCourseCreation(_courseCreator))
                {
                    CreateCourse(_courseCreator);
                    args.SetHandled();
                }
            };
        }

        private void SetupDisplayEvents(ICourseDisplay aCourseDisplay)
        {
            aCourseDisplay.CourseSelected += (sender, i) => DisplayModules?.Invoke(i);
        }

        private bool ValidateCourseCreation(ICourseCreator aCourseCreator)
        {
            //Go through the different course creator properties, checking each for validity
            if(string.IsNullOrWhiteSpace(aCourseCreator.CourseName))
            {
                ErrorMessage.DisplayFor(courseCreator, "Validation Error", "No Text Entered", "A text must be entered in the Course Name field.");
                return false;
            }

            //Check if the module input string matches one of the valid formats
            if(Delimiters.DelimitType(InternalConstants.DelimitChar, aCourseCreator.CourseModules) == Delimiters.DelimitedType.Invalid)
            {
                ErrorMessage.DisplayFor(courseCreator, "Validation Error", "Invalid format for Modules field",
                    "Valid formats\nSingle number (ie '5')" +
                    "\nNumber followed by preferred Module Name (ie '5;Quizzes')" +
                    "\nManual Module Names (ie 'Module 1;Quiz 2;Test 3)");

                return false;
            }

            //Since the link is optional we don't do any checking on that

            return true;
        }

        #endregion Methods
    }
}