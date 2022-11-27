//Björn Rundquist 16-11-2020
using System.Collections.Generic;
using CourseNotesLib.Domain_Classes;

namespace CourseNotesLib.Repository_Interfaces
{

    internal interface ITagsRepository
    {
        /// <summary>
        /// The repository list of tags
        /// </summary>
        List<Tag> Tags { get; }

        /// <summary>
        /// Returns the latest unique id for tag creation, updated when a new tag is inserted
        /// </summary>
        int NewTagId { get; }
    }
}