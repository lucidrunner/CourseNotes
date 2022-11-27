//Björn Rundquist 16/11-2020
using CourseNotesLib.Domain_Interfaces;

namespace CourseNotesLib.Domain_Classes
{
    internal class Tag : ITag
    {
        public int Id { get; }
        public string TagText { get; }

        public Tag(int id, string tagText)
        {
            Id = id;
            TagText = tagText;
        }
        
    }
}