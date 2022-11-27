//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Display_Interfaces
{
    /// <summary>
    /// Provides delegates and events for displaying a selectable list of modules
    /// </summary>
    public interface IModuleDisplay: IDisplay
    {
        /// <summary>
        /// Invoked by the library to pass a list of IModules that can be used to fill the display
        /// </summary>
        Action<IEnumerable<IModule>> FillModuleListAction { get; set; }

        /// <summary>
        /// Should return the Module Ids of the currently selected modules
        /// </summary>
        IEnumerable<int> SelectedModuleIds { get; }

        /// <summary>
        /// Should be raised by the control when the module selection changes
        /// </summary>
        event EventHandler ModuleSelectionChanged;



        //Original implementation below, kept it to show how you can use tuples to pass functionally anonymous types through actions / funcs, since you can't use actual anonymous types
        //The downside is you don't get labeling so you're stuck with Item1, Item2 etc for the different fields, which is why I ended up making public interfaces instead
        //Action<IEnumerable<Tuple<string, int>>> FillModuleListAction { get; set; }
    }
}