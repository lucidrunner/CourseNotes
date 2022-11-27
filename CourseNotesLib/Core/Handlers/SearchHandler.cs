//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using System.Linq;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Domain_Classes;
using UtilityLibrary.Strings;

namespace CourseNotesLib.Core
{
    /// <summary>
    /// Handles filtering of the currently displayed notes by providing predicate methods based on the IDisplayFilterer
    /// </summary>
    internal class SearchHandler
    {
        #region Fields

        private readonly IDisplayFilterer searchFilterer;

        #endregion Fields

        #region Constructors

        internal SearchHandler(IDisplayFilterer searchFilterer)
        {
            this.searchFilterer = searchFilterer;

            LinkFilterer();
        }

        #endregion Constructors

        #region Properties

        internal Action SearchUpdated { get; set; }
        internal Converter<IEnumerable<int>, IEnumerable<string>> TagIdConverter { get; set; }

        #endregion Properties

        #region Methods


        /// <summary>
        /// Returns true if the passed in note matches the current tag search input, or true if no tag search is performed
        /// </summary>
        public bool PerformTagSearch(Note aNote)
        {
            if(searchFilterer == null) return true;

            //Split the tag input and check if any of the parts match the text on the note's tags
            if(SplitInput(searchFilterer.TagSearch, out List<string> _parts))
            {
                var _noteTags = TagIdConverter?.Invoke(aNote.TagIds);
                if(_noteTags == null) return false;

                //For any part, we pass the check if any tag contains the part
                return _parts.Any(part => _noteTags.Any(tag => tag.Contains(part)));
            }

            return true;
        }

        /// <summary>
        /// Returns true if the passed in note (or its link) matches the current word search input, or true if no word search is performed
        /// </summary>
        public bool PerformWordSearch(Note aNote)
        {
            if(searchFilterer == null) return true;

            //If the list is split incorrectly, show all possible notes
            if(SplitInput(searchFilterer.WordSearch, out List<string> _parts))
            {
                //Return true if any of the texts or (if we're including links) the links contains any of the provided words
                return _parts.Any(part => aNote.Text.Contains(part)) || (searchFilterer.IncludeLinks && _parts.Any(part => aNote.Link.Contains(part)));
            }

            return true;
        }

        internal bool DisplayForAll()
        {
            return searchFilterer?.DisplayAll ?? false;
        }

        private void LinkFilterer()
        {
            if(searchFilterer == null) return;

            searchFilterer.FilterChanged += (sender, args) => SearchUpdated?.Invoke();
        }

        //Small helper method to split the tag input and make sure it's valid
        private bool SplitInput(string aInput, out List<string> aSplitInput)
        {
            aSplitInput = Delimiters.SplitString(InternalConstants.DelimitChar, aInput);
            return aSplitInput != null && aSplitInput.Count != 0;
        }

        #endregion Methods
    }
}