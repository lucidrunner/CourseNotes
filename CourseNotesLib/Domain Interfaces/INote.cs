//Björn Rundquist 16/11-2020
namespace CourseNotesLib.Domain_Interfaces
{
    public interface INote
    {
        int Id { get; }
        string Text { get; }
        string Link { get; }

    }
}