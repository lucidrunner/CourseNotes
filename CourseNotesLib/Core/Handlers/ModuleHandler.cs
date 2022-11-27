//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using System.Linq;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Domain_Classes;
using CourseNotesLib.Repository_Interfaces;
using UtilityLibrary.Strings;

namespace CourseNotesLib.Core
{
    /// <summary>
    /// Handles interaction and manipulation of modules
    /// </summary>
    internal class ModuleHandler
    {
        #region Fields

        internal Action<IEnumerable<int>> DisplayNotes;
        internal Func<bool> DisplayNotesForAllModules;

        private readonly IModuleDisplay moduleDisplay;
        private readonly IModuleRepository moduleRepository;

        #endregion Fields

        #region Constructors

        public ModuleHandler(IModuleDisplay moduleDisplay, IModuleRepository moduleRepository)
        {
            this.moduleDisplay = moduleDisplay;
            this.moduleRepository = moduleRepository;

            SetupDisplayEvents();
        }

        #endregion Constructors

        #region Methods

        internal void AddModules(string moduleString, int aCourseId)
        {
            //Begin by splitting our input module string
            var _splitType = Delimiters.DelimitType(InternalConstants.DelimitChar, moduleString, out var _parts);

            //Based on the type of delimited string, we use different creation methods and pass in the gotten parts
            switch(_splitType)
            {
                case Delimiters.DelimitedType.SingleInt: //For a single int, create a set of generic modules matching the int
                    GenerateGenericModules(_parts, aCourseId);
                    break;

                case Delimiters.DelimitedType.IntFirst: //If we have a single int and then one or several strings, create a set of named modules based on the first string
                    GenerateNamedSetOfModules(_parts, aCourseId);
                    break;

                case Delimiters.DelimitedType.Strings: //If we only have strings, create a module for each passed in string
                    GenerateModulesFromStrings(_parts, aCourseId);
                    break;

                //Otherwise, return
                case Delimiters.DelimitedType.Invalid:
                    return;
            }
        }

        internal void DisplayModules(int aCourseId)
        {
            //Filter modules based on the provided course ID
            var _modules = moduleRepository.Modules.Where(t => t.CourseId == aCourseId);
            //And pass it to the fill action
            moduleDisplay.FillModuleListAction?.Invoke(_modules);
        }

        internal int? GetCurrentSingleId()
        {
            //Try to get the full current selection
            var _selectedModules = (moduleDisplay?.SelectedModuleIds ?? new List<int>()).ToList();

            //On no / many selected, return an invalid Id
            if(_selectedModules.Count == 0 || _selectedModules.Count > 1)
                return null;

            //Otherwise, return
            return _selectedModules[0];
        }

        internal void RefreshModuleNotesDisplay()
        {
            bool _displayForAll = DisplayNotesForAllModules?.Invoke() ?? false;

            //Display notes either for all saved modules or just for the currently selected ones (or a default empty list if we fail with getting either)
            var _selectedModules = _displayForAll ? moduleRepository.Modules.Select(t => t.Id) : moduleDisplay?.SelectedModuleIds ?? new List<int>();
            DisplayNotes?.Invoke(_selectedModules);
        }

        private void GenerateGenericModules(IEnumerable<string> aSplits, int aCourseId)
        {
            var _collection = aSplits.ToList();
            if(_collection.Count == 0)
                return;

            //if we have a single int, we generate a set of generic modules based on the index until we reach the set module count
            if(!int.TryParse(_collection[0], out int _numberOfModules)) return;

            for(int _index = 0; _index < _numberOfModules; _index++)
            {
                moduleRepository.Modules.Add(new Module(moduleRepository.Modules.Count, aCourseId, InternalConstants.DefaultModuleName + " " + (_index + 1)));
            }
        }

        private void GenerateModulesFromStrings(IEnumerable<string> aSplits, int aCourseId)
        {
            //If we have a set of strings, we generate our modules using those as names
            foreach(string _aSplit in aSplits)
            {
                moduleRepository.Modules.Add(new Module(moduleRepository.Modules.Count, aCourseId, _aSplit));
            }
        }

        private void GenerateNamedSetOfModules(IEnumerable<string> aSplits, int aCourseId)
        {
            var _collection = aSplits.ToList();
            if(_collection.Count < 2)
                return;
            //if we start with an int we use the next value in the split to decide the module names
            if(!int.TryParse(_collection[0], out int _numberOfModules)) return;
            string _moduleNames = _collection[1];

            for(int _index = 0; _index < _numberOfModules; _index++)
            {
                moduleRepository.Modules.Add(new Module(moduleRepository.Modules.Count, aCourseId, _moduleNames + " " + (_index + 1)));
            }
        }

        private void SetupDisplayEvents()
        {
            moduleDisplay.ModuleSelectionChanged += (sender, i) =>
            {
                RefreshModuleNotesDisplay();
            };
        }

        #endregion Methods
    }
}