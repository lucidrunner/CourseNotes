//Björn Rundquist 16/11-2020
using System;
using System.Collections.Generic;
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
using CourseNotesLib.Display_Interfaces;
using CourseNotesLib.Domain_Interfaces;
using CourseNotesLib.Utility;
using CourseNotesLib.Utility.Interfaces;
using GenericControls;
using UtilityLibrary.Events;
using UtilityLibrary.WPF_Utilities;

namespace CourseNotes.UserControls
{
    /// <summary>
    /// Interaction logic for CoursesControl.xaml
    /// </summary>
    public partial class CoursesControl: UserControl, ICourseCreator, ICourseDisplay, IModuleDisplay
    {
        #region Fields

        //The backing lists for the displayed courses and modules
        private readonly List<ICourse> displayedCourses = new List<ICourse>();

        private readonly List<IModule> displayedModules = new List<IModule>();

        #endregion Fields

        #region Constructors

        public CoursesControl()
        {
            InitializeComponent();

            //Bind the BLL actions to the fill methods
            FillCourseListAction += OnCourseListFill;
            FillModuleListAction += OnModuleFill;

            MultiSelectCheckbox.CheckBoxState.Checked += (sender, args) => ModuleList.SelectionMode = SelectionMode.Multiple;
            MultiSelectCheckbox.CheckBoxState.Unchecked += (sender, args) => ModuleList.SelectionMode = SelectionMode.Single;
        }

        #endregion Constructors

        #region BLL Events

        public event EventHandler<HandledEventArgs> CourseCreating;

        public event EventHandler<int> CourseSelected;

        public event EventHandler ModuleSelectionChanged;

        #endregion BLL Events

        #region BLL Properties

        //We provide a default empty string rather than returning null if these are accessed pre-loading as a safety precaution
        public string CourseLink => LinkInput?.InputText ?? "";

        public string CourseModules => ModulesInput?.InputText ?? "";

        public string CourseName => NameInput?.InputText ?? "";

        public Action<IErrorMessage> DisplayErrorMessageAction { get; set; }
        public Action<IEnumerable<ICourse>> FillCourseListAction { get; set; }

        public Action<IEnumerable<IModule>> FillModuleListAction { get; set; }

        #endregion BLL Properties

        #region Methods

        public IEnumerable<int> SelectedModuleIds
        {
            get
            {
                //Get all selected indexes for the listbox
                var _selectedIndexes = SelectionsUtility.GetIndexesOfSelectedItems(ModuleList);

                //Retrieve Id's from the backing list
                List<int> _selectedIds = new List<int>();
                foreach(int _selectedIndex in _selectedIndexes)
                {
                    //Skip if the index is invalid
                    if(_selectedIndex < 0 || _selectedIndex >= displayedModules.Count)
                        continue;

                    _selectedIds.Add(displayedModules[_selectedIndex].Id);
                }

                return _selectedIds;
            }
        }

        /// <summary>
        /// Resets the state of the control by clearing the input fields & viewed collections
        /// </summary>
        public void ResetState()
        {
            displayedModules.Clear();
            CourseList.SelectedItem = null;
            CourseList.ItemsSource = null;

            //Since we can have different selection modes on the ModuleList we have to be a bit careful here because WPF throws exception if we attempt to access the "wrong" selection
            if(ModuleList.SelectionMode == SelectionMode.Single)
                ModuleList.SelectedItem = null;
            else
                ModuleList.SelectedItems.Clear();
            ModuleList.ItemsSource = null;

            ClearInputFields();
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearInputFields();
        }

        private void ClearInputFields()
        {
            //Clears the Add-course fields
            NameInput.ClearInput();
            LinkInput.ClearInput();
            ModulesInput.ClearInput();
        }

        private void CopyLink_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(GetCurrentLink()))
                e.CanExecute = true;
        }

        private void CopyLink_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Fairly straightforward to copy to clip but only works in certain environments afaik
            Clipboard.SetData(DataFormats.Text, GetCurrentLink());
        }

        private void CourseCreation_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Raise the pre-creation event with a trackable HandledEventArgs
            HandledEventArgs _e = new HandledEventArgs(this);
            CourseCreating?.Invoke(this, _e);

            //If the event was handled the course was successfully created and we can clear our input fields
            if(_e.Handled)
                ClearInputFields();
        }

        private void Courses_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CourseList.SelectedIndex < 0 || CourseList.SelectedIndex >= displayedCourses.Count)
                return;
            //Get the currently selected Id from the backing list
            int _id = displayedCourses[CourseList.SelectedIndex].Id;
            //And invoke the event
            CourseSelected?.Invoke(this, _id);
        }

        private string GetCurrentLink()
        {
            //Make sure the current selection is valid
            if(CourseList.SelectedIndex < 0 || CourseList.SelectedIndex >= displayedCourses.Count)
                return null;

            //Then get the link from the backing list
            return displayedCourses[CourseList.SelectedIndex].Link;
        }

        private void Modules_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Begin by making sure the current selection is valid
            if(ModuleList.SelectionMode == SelectionMode.Single && ModuleList.SelectedIndex < 0)
                return;

            if(ModuleList.SelectionMode == SelectionMode.Multiple && SelectedModuleIds.ToList().Count == 0)
                return;

            //Then raise the selection event
            ModuleSelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCourseListFill(IEnumerable<ICourse> aCourses)
        {
            //Clear the current fill
            displayedCourses.Clear();
            //Add the sent in courses to our backing list
            var _collection = aCourses.ToList();
            displayedCourses.AddRange(_collection);
            //And display only the names in the ListBox
            var _courseNames = _collection.Select(t => t.Name).ToList();
            CourseList.ItemsSource = _courseNames;
        }

        private void OnModuleFill(IEnumerable<IModule> aModules)
        {
            //Same as for the course fill, clear the current backing list and create a new backing / display list
            displayedModules.Clear();
            var _collection = aModules.ToList();
            displayedModules.AddRange(_collection);
            var _moduleNames = _collection.Select(t => t.ModuleName).ToList();
            ModuleList.ItemsSource = _moduleNames;
        }

        #endregion Methods
    }
}