
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Utility.Interfaces;

namespace CourseNotesLib.Utility
{
    /// <summary>
    /// Basic error message, providing a header title & actual message
    /// </summary>
    internal class ErrorMessage : IErrorMessage
    {
        public string Title { get; }
        public string Header { get; }
        public string Message { get; }

        internal ErrorMessage(string errorTitle, string header, string errorMessage)
        {
            Title = errorTitle;
            Message = errorMessage;
            Header = header;
        }

        /// <summary>
        /// Static wrapper for attempting to invoke an error message on a provided display
        /// </summary>
        internal static void DisplayFor(IDisplay aDisplay, string aTitle, string aHeader, string aMessage)
        {
            aDisplay?.DisplayErrorMessageAction?.Invoke(new ErrorMessage(aTitle, aHeader, aMessage));
        }
    }
}