//Björn Rundquist 16/11-2020
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Domain_Classes
{
    internal class Module : IModule
    {
        public int Id { get; set; }

        //Rather than have direct references these domain classes are more like DB entities using Ids to map relationships. These Ids are then used by the different handlers 
        //to get the corresponding objects
        public int CourseId { get; set; }

        public string ModuleName { get; set; }

        public Module(int id, int courseId, string moduleName)
        {
            Id = id;
            CourseId = courseId;
            ModuleName = moduleName;
        }
    }
}