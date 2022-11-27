//Björn Rundquist 16/11-2020
namespace CourseNotesLib.Utility.Interfaces
{
    public interface IErrorMessage
    {
        string Title { get; }
        string Header { get; }
        string Message { get; }
    }
}