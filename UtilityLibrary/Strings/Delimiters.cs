using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace UtilityLibrary.Strings
{
    public static class Delimiters
    {
        public enum DelimitedType
        {
            SingleInt, IntFirst, Strings, Invalid
        }

        /// <summary>
        /// Returns the split type of the string based on the delimiter
        /// </summary>
        /// <returns>Whether the split list consists of a single int, strings, starts with an int etc</returns>
        public static DelimitedType DelimitType(char[] delimitChar, string delimitedString)
        {
            //split the passed in string with the delimiter chars to get our parts
            var _parts = SplitString(delimitChar, delimitedString);

            return GetDelimitedType(_parts);
        }

        public static DelimitedType DelimitType(char[] delimitChar, string delimitedString, out IEnumerable<string> aSplits)
        {
            //split the passed in string with the delimiter chars to get our parts
            var _parts = SplitString(delimitChar, delimitedString);

            //set our out value so we don't have to perform the split twice
            aSplits = _parts;

            return GetDelimitedType(_parts);
        }

        public static List<string> SplitString(char[] delimitChar, string delimitedString)
        {
            if (string.IsNullOrWhiteSpace(delimitedString))
                return null;


            //Split the string into parts, remove the empty / only whitespace parts, clean up whitespace and then cast the final result to a list
            var _parts = delimitedString.Split(delimitChar, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(s => s.Trim())
                .ToList();

            return _parts;
        }

        private static DelimitedType GetDelimitedType(List<string> _parts)
        {
            //If we have no valid parts after the split & cleanup, return as invalid
            if (_parts == null || _parts.Count == 0)
                return DelimitedType.Invalid;


            //See if we are starting with an int
            if (int.TryParse(_parts[0], out int _))
            {
                //Return the single int type if the split list is only one part
                if (_parts.Count == 1)
                {
                    return DelimitedType.SingleInt;
                }
                //Otherwise, return that we're starting with an int
                else
                {
                    return DelimitedType.IntFirst;
                }
            }


            //Finally, return by default that it's a simple split list of strings
            return DelimitedType.Strings;
        }
    }
}