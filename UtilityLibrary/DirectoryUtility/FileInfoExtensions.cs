//Björn Rundquist 12/10-2020
using System.IO;
using System.Text;

namespace UtilityLibrary.DirectoryUtility
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Removes the .fileEnding by splitting the Name on '.' and reconstructing it without the last part
        /// </summary>
        public static string NameWithoutFilePath(this FileInfo value)
        {
            string[] _splitName = value.Name.Split('.');

            StringBuilder _sb = new StringBuilder();

            for (int _index = 0; _index < _splitName.Length - 1; _index++)
            {
                _sb.Append(_splitName[_index]);
            }

            return _sb.ToString();
        }

    }
}

