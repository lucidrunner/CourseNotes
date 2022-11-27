//Björn Rundquist 16/11-2020
namespace CourseNotesLib.Domain_Interfaces
{
    public interface ICourse
    {
        int Id { get; }
        string Name { get; }
        string Link { get; }
    }
}