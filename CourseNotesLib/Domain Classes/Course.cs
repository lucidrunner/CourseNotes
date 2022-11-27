//Björn Rundquist 16/11-2020
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Domain_Classes
{
    
    internal class Course : ICourse
    {
        public int Id { get; }
        public string Name { get; }
        public string Link { get; }



        internal Course(int id, string name, string link)
        {
            this.Id = id;
            this.Name = name;
            this.Link = link;
        }

    }
}