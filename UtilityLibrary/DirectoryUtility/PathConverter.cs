//Björn Rundquist 12/10-2020
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilityLibrary.DirectoryUtility
{
    //This isn't safe when it comes to long paths but I don't really wanna dig too hard into fixing that
    //for this specific project :x
    //Contains different methods (currently only shortening) to convert paths into more easily displayable formats
    public static class PathConverter
    {
        #region Public Methods

        /// <summary>
        /// Shortens the provided directory path by replacing the middle of it with ... to keep it under the max allowed length
        /// </summary>
        public static string ShortenPath(DirectoryInfo aDirectoryInfo, int aMaxLength = 50)
        {
            return ShortenPath(aDirectoryInfo.FullName, aMaxLength);
        }

        /// <summary>
        /// Shortens the provided file path by replacing the middle of it with ... to keep it under the max allowed length
        /// </summary>
        public static string ShortenPath(FileInfo aFileInfo, int aMaxLength = 50)
        {
            return ShortenPath(aFileInfo.FullName, aMaxLength);
        }

        //Shortens a path by splitting it into segments and rebuilding them until adding another segment would overflow the max length
        public static string ShortenPath(string aFullPath, int aMaxLength = 50)
        {
            //If the path is valid for printing, simply return it
            if(aFullPath.Length < aMaxLength)
            {
                return aFullPath;
            }

            //Otherwise begin by splitting the path
            List<string> _pathParts = new List<string>(aFullPath.Split(@"\".ToCharArray())); aFullPath.Split(@"\".ToCharArray());

            //If the file itself is too long to display, just return it
            if(_pathParts.Count < 2)
            {
                return aFullPath;
            }

            //Add the first and last part to the builders
            StringBuilder _pathStartBuilder = new StringBuilder(_pathParts.First() + @":\");
            StringBuilder _pathEndBuilder = new StringBuilder(@"\" + _pathParts.Last());

            //and remove them from the considered list
            _pathParts.RemoveAt(0);
            _pathParts.RemoveAt(_pathParts.Count - 1);

            bool _addStart = false;

            //If we somehow run out of parts without going over the limit, abort and return instantly
            while(_pathParts.Count > 0)
            {
                //Add to the start of the path on every other check
                if(_addStart)
                {
                    _pathStartBuilder.Append(_pathParts.First() + @"\");
                    _pathParts.RemoveAt(0);
                }

                //And to the end on every other
                else
                {
                    _pathEndBuilder.Insert(0, @"\" + _pathParts.Last());
                    _pathParts.RemoveAt(_pathParts.Count - 1);
                }

                //Check if we should stop combing (+3 for the ...)
                if(_pathStartBuilder.Length + _pathEndBuilder.Length + 3 >= aMaxLength)
                {
                    break;
                }

                //Flip if we're adding the start of the end of the path to the builders
                _addStart = !_addStart;
            }

            return _pathStartBuilder + "..." + _pathEndBuilder;
        }

        #endregion Public Methods
    }
}