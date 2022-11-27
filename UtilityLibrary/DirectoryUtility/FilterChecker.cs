//Björn Rundquist 12/10-2020

namespace UtilityLibrary.DirectoryUtility
{
    //Contains methods to make sure filters for file checks are correctly formatted
    public static class FilterChecker
    {
        /// <summary>
        /// Inserts "*." (or "*" if "." is already present) at the start of each provided filter
        /// </summary>
        public static void InsertFilterForAll(string[] aFilters)
        {
            for (int _index = 0; _index < aFilters.Length; _index++)
            {
                aFilters[_index] = InsertFilterForAll(aFilters[_index]);
            }
        }


        /// <summary>
        /// Inserts "*." (or "*" if "." is already present) at the start of the provided filter
        /// </summary>
        public static string InsertFilterForAll(string aFilter)
        {
            if (aFilter.StartsWith("*."))
                return aFilter;

            if(!aFilter.StartsWith("."))
                aFilter = "." + aFilter;

            return "*" + aFilter;

        }

    }
}