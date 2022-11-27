//Björn Rundquist 16/11-2020
namespace CourseNotesLib.Domain_Interfaces
{
    public interface IModule
    {
        int Id { get; }
        int CourseId { get; }
        string ModuleName { get; }
    }
}