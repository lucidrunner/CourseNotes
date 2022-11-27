//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourseNotesLib.Domain_Classes;
using CourseNotesLib.Repository_Interfaces;
using UtilityLibrary.Strings;

namespace CourseNotesLib.Core
{
    /// <summary>
    /// Handles creation and conversion of tags between string and tag formats.
    /// </summary>
    internal class TagsHandler
    {
        #region Fields

        private readonly ITagsRepository repository;

        #endregion Fields

        #region Constructors

        internal TagsHandler(ITagsRepository repository)
        {
            this.repository = repository;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Returns the text for the provided tag Ids
        /// </summary>
        public IEnumerable<string> GetTagStrings(IEnumerable<int> aTagIds)
        {
            return repository.Tags.Where(t => aTagIds.Contains(t.Id)).Select(t => t.TagText);
        }

        /// <summary>
        /// Converts a provided collection of Ids to a delimited string representation instead
        /// </summary>
        /// <param name="aTagIds"></param>
        /// <returns></returns>
        internal string ConvertTagIds(IEnumerable<int> aTagIds)
        {
            StringBuilder _sb = new StringBuilder();

            //If we have no delimiter set, we can't convert the tags to a string represenation
            if(InternalConstants.DelimitChar.Length == 0)
            {
                Console.WriteLine("Couldn't find internal delimiter.");
                return "";
            }

            //Go through all the provided ids
            foreach(int _tagId in aTagIds)
            {
                //Try to get it from the repository
                Tag _tag = repository.Tags.FirstOrDefault(t => t.Id == _tagId);

                if(_tag == null)
                    continue;

                //And append their text to the stringbuilder
                _sb.Append(_tag.TagText + InternalConstants.DelimitChar[0]);
            }

            //Finally, we trim the last delimit char off and return the string
            return _sb.ToString().TrimEnd(InternalConstants.DelimitChar[0]);
        }

        internal IEnumerable<int> GetTagIds(string aTagDelimitedString)
        {
            if(Delimiters.DelimitType(InternalConstants.DelimitChar, aTagDelimitedString, out IEnumerable<string> _splits) != Delimiters.DelimitedType.Strings)
                return new List<int>();

            List<int> _tagIds = new List<int>();
            foreach(string _aSplit in _splits)
            {
                //When adding a new tag, we first check to make sure we don't have an existing tag with the exact same string, if we do we simply use that instead
                var _tag = repository.Tags.FirstOrDefault(t => t.TagText.ToLowerInvariant().Equals(_aSplit.ToLowerInvariant()));
                //Otherwise create a new tag with the provided split and add it to the repository
                if(_tag == null)
                {
                    _tag = new Tag(repository.NewTagId, _aSplit);
                    repository.Tags.Add(_tag);
                }

                _tagIds.Add(_tag.Id);
            }

            return _tagIds;
        }

        #endregion Methods
    }
}