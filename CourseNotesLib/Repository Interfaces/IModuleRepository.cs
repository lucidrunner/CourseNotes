//Björn Rundquist 16/11-2020
using System.Collections.Generic;
using CourseNotesLib.Domain_Classes;

namespace CourseNotesLib.Repository_Interfaces
{
    internal interface IModuleRepository
    {
        List<Module> Modules { get; }
        
    }
}