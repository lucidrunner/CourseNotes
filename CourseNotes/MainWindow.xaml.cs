//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CourseNotesLib;
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Utility;
using CourseNotesLib.Utility.Interfaces;
using GenericControls;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace CourseNotes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window, INoteTakerInterface
    {
        #region Fields

        private CourseNoteTaker courseNoteTaker;
        private FileInfo currentFile;

        #endregion Fields

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            courseNoteTaker = new CourseNoteTaker(this);

            SetupControlLinks();
        }

        #endregion Constructors

        #region Properties

        //The different sub-interfaces from the INoteTakerInterface
        public ICourseCreator CourseCreator => CoursesControl;

        public ICourseDisplay CourseDisplay => CoursesControl;

        //The name / filterer confusion is due to it originally only being for word search before I added some filtering capabilities and realized the original naming didn't make a lot of sense
        public IDisplayFilterer DisplayFilterer => SearchControl;

        public IModuleDisplay ModuleDisplay => CoursesControl;

        public INotesPrinter NotePrinter => PrintControl;

        public INotesCreator NotesCreator => AddControl;

        public INotesDisplay NotesDisplay => NotesControl;

        public INoteViewer NoteViewer => ViewControl;

        public Action ResetDisplay { get; set; }
        public Func<FileInfo, bool> LoadFromFile { get; set; }

        public Func<DirectoryInfo, string, FileInfo> SaveAs { get; set; }

        public Func<FileInfo, bool> SaveToFile { get; set; }

        #endregion Properties

        #region Methods

        private void DisplayErrorMessage(IErrorMessage obj)
        {
            ErrorMessage _message = new ErrorMessage(obj.Title, obj.Header, obj.Message);
            _message.ShowDialog();
        }

        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            //Open a dialog looking for a xml file
            OpenFileDialog _fileDialog = new OpenFileDialog();
            _fileDialog.Filter = "Course XMl file (*.xml) | *.xml";
            if(_fileDialog.ShowDialog() == true)
            {
                FileInfo _selectedFile = new FileInfo(_fileDialog.FileName);

                //If the file is correctly loaded, set it as our worked file for easier access
                if(_selectedFile.Exists && (LoadFromFile?.Invoke(_selectedFile) ?? false))
                {
                    currentFile = _selectedFile;
                }
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            //If we're working with a file, use that
            if(currentFile != null)
            {
                SaveToFile?.Invoke(currentFile);
            }
            else
            {
                //Otherwise invoke the Save As method instead
                SaveAs_OnClick(sender, e);
            }
        }

        private void SaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            //use a folder browser to get our file save location
            VistaFolderBrowserDialog _folderDialog = new VistaFolderBrowserDialog {Description = "Select Folder for new CourseNotes file."};


            if(_folderDialog.ShowDialog() == true)
            {
                //if the location is valid, invoke the Save action
                DirectoryInfo _directory = new DirectoryInfo(_folderDialog.SelectedPath);
                if(_directory.Exists)
                    currentFile = SaveAs?.Invoke(_directory, "CourseNotes");
            }
        }

        /// <summary>
        /// Sets up internal linkage between the controls and the main window
        /// </summary>
        private void SetupControlLinks()
        {
            
            NotesControl.ModifyNote += AddControl.SetModifiedNote;

            //Tie the reset action to the different controls
            ResetDisplay += CoursesControl.ResetState;
            ResetDisplay += NotesControl.ResetState;
            ResetDisplay += AddControl.ResetState;
            ResetDisplay += ViewControl.ResetState;
            ResetDisplay += SearchControl.ResetState;
            ResetDisplay += PrintControl.ResetState;

            //Rather than implementing error windows for each control we tie them all to the main DisplayErrorMessage method
            CoursesControl.DisplayErrorMessageAction += DisplayErrorMessage;
            NotesControl.DisplayErrorMessageAction += DisplayErrorMessage;
            AddControl.DisplayErrorMessageAction += DisplayErrorMessage;
            ViewControl.DisplayErrorMessageAction += DisplayErrorMessage;
            SearchControl.DisplayErrorMessageAction += DisplayErrorMessage;
        }


        #endregion Methods
    }
}