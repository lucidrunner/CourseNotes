//Björn Rundquist 16/11-2020
using System.Collections.Generic;
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Domain_Classes
{
    internal class Note : INote
    {
        public int Id { get;}

        public string Text { get; internal set; }
        public string Link { get; internal set; }
        public int ModuleId { get; }

        public List<int> TagIds { get; private set; }

        internal Note(int id, int moduleId)
        {
            Id = id;
            ModuleId = moduleId;
        }
        internal Note(int id, int moduleId, string text, string link)
        {
            Id = id;
            Text = text;
            Link = link;
            ModuleId = moduleId;
            TagIds = new List<int>();
        }

        public void SetTags(IEnumerable<int> aTags)
        {
            if(aTags != null)
                TagIds = new List<int>(aTags);
        }
    }
}