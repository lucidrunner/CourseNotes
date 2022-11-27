using System.Windows.Input;

namespace CourseNotes.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand CreateCourse = new RoutedUICommand
        (
            "CreateCourse",
            "CreateCourse",
            typeof(CustomCommands)
        );


        public static readonly RoutedUICommand CopyLink = new RoutedUICommand
        (
            "CopyLink",
            "CopyLink",
            typeof(CustomCommands)
        );



        public static readonly RoutedUICommand ModifyNote = new RoutedUICommand
        (
            "ModifyNote",
            "ModifyNote",
            typeof(CustomCommands)
        );


        public static readonly RoutedUICommand Clear = new RoutedUICommand
        (
            "Clear",
            "Clear",
            typeof(CustomCommands)
        );
    }
}