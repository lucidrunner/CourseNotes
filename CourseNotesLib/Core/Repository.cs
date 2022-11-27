//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using System.Linq;
using CourseNotesLib.Domain_Classes;
using CourseNotesLib.Repository_Interfaces;

namespace CourseNotesLib.Core
{
    /// <summary>
    /// Simple repository for our domain classes. I use internal interfaces to limit access a bit.
    /// Didn't want to add a dedicated DAL layer to this project to keep it a bit more manageable so these lists are directly modified in the different handlers instead
    /// </summary>
    internal class Repository: ICourseRepository, IModuleRepository, INotesRepository, ITagsRepository
    {
        #region Constructors

        internal Repository()
        {
            Courses = new List<Course>();
            Modules = new List<Module>();
            Notes = new List<Note>();
            Tags = new List<Tag>();
        }

        #endregion Constructors

        #region Events

        internal event EventHandler RepositoryLoaded;

        #endregion Events

        #region Properties

        public List<Course> Courses { get; }
        public List<Module> Modules { get; }

        public List<Note> Notes { get; }

        public List<Tag> Tags { get; }

        //Since we can remove tags & notes we need to be a bit cleverer when it comes to generating new id's for them. This makes sure that they get unique int id's
        public int NewNoteId => Notes.LastOrDefault()?.Id + 1 ?? 0;

        public int NewTagId => Tags.LastOrDefault()?.Id + 1 ?? 0;

        #endregion Properties

        #region Methods

        public bool Load(List<Course> courses, List<Module> modules, List<Note> notes, List<Tag> tags)
        {
            //Validate that all the internal id's are valid
            bool _moduleCourseConnected = modules.All(module => courses.Any(course => course.Id == module.CourseId));

            bool _noteModuleConnected = notes.All(note => modules.Any(module => module.Id == note.ModuleId));

            bool _noteTagsConnected = notes.All(note => note.TagIds.All(tagId => tags.Any(tag => tag.Id == tagId)));

            //If we passed validation, set our repository to contain the loaded items
            if(_moduleCourseConnected && _noteModuleConnected && _noteTagsConnected)
            {
                Courses.Clear();
                Courses.AddRange(courses);

                Modules.Clear();
                Modules.AddRange(modules);

                Notes.Clear();
                Notes.AddRange(notes);

                Tags.Clear();
                Tags.AddRange(tags);

                //Invoke the loaded event so we can refresh the display
                RepositoryLoaded?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        //This breaks my other architecture a bit (should have removed the note in the note handler and then called an action to scrub any lingering tags rather)
        //I forgot I hadn't moved it out until I was turning this in and didn't want to make any more changes though
        public bool RemoveNote(int aId)
        {
            //Get the note from the collection
            Note _note = Notes.FirstOrDefault(t => t.Id == aId);

            //if we've passed an invalid Id, return
            if(_note == null)
                return false;

            //Remove the note
            Notes.Remove(_note);

            //And remove any tags that no longer are connected to any notes
            Tags.RemoveAll(tag => !Notes.Any(note => note.TagIds.Contains(tag.Id)));

            return true;
        }

        #endregion Methods
    }
}