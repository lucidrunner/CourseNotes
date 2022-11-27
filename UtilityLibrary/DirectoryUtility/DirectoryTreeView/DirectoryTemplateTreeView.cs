//Björn Rundquist 12/10-2020
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace UtilityLibrary.DirectoryUtility.DirectoryTreeView
{
    public static class DirectoryTemplateTreeView
    {
        #region Private Fields

        private const string LoadingString = "Loading...";

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Reloads a tree using the passed in filter array
        /// </summary>
        public static void ApplyFilter(TreeView aTemplateTreeView, string[] aFilters)
        {
            foreach(var _baseItem in aTemplateTreeView.Items)
            {
                //Our base items are always either a Drive or Directory so we don't have to do any extra checks to remove FileItems here
                if(_baseItem is DirectoryTreeItem _treeItem)
                {
                    //We're only interested in filtering the base items if they've already been expanded (ie their dummy string is removed)
                    if(_treeItem.Items.Count == 1 && _treeItem.Items[0] is string)
                    {
                        continue;
                    }

                    //For each expanded base item, recursively filter it
                    if(_treeItem.Items.Count > 0)
                    {
                        RecursiveFilter(_treeItem, aFilters);
                    }
                }
            }
        }

        /// <summary>
        /// Populates a TreeView based on the available disk drives
        /// </summary>
        public static void CreateTreeView(TreeView aTreeView)
        {
            aTreeView.Items.Clear();
            DriveInfo[] _drives = DriveInfo.GetDrives();
            foreach(DriveInfo _driveInfo in _drives)
            {
                aTreeView.Items.Add(CreateExpandableTreeItem(_driveInfo));
            }
        }

        /// <summary>
        /// Creates a TreeView with the provided directory as the base node, or a standard directory tree view if the directory is invalid or null
        /// </summary>
        public static void CreateTreeView(TreeView aTreeView, DirectoryInfo aBaseDirectory)
        {
            if(aBaseDirectory == null || !aBaseDirectory.Exists)
            {
                CreateTreeView(aTreeView);
                return;
            }

            aTreeView.Items.Clear();
            aTreeView.Items.Add(CreateExpandableTreeItem(aBaseDirectory));
        }

        /// <summary>
        /// Expands a TreeViewItem that has been created from DirectoryTreeItem. Note that this is e.OriginalSource rather than e.Source on a TreeView OnExpand event.
        /// </summary>
        public static void ExpandTreeViewItem(TreeViewItem aTreeViewItem, string[] aFilters = null)
        {
            //Attempt to cast the underlying item as either a drive or directory (we can't do a general cast since we can trigger the expand event for files by double clicking on them)
            DirectoryTreeItem _item = aTreeViewItem.Header as DriveItem ?? (DirectoryTreeItem)(aTreeViewItem.Header as DirectoryItem);

            //We're only interested in adding new nodes if it hasn't been expanded before
            if(_item == null || _item.Items.Count != 1 && !(_item.Items[0] is string))
            {
                return;
            }

            //Clear the dummy string child
            _item.Items.Clear();

            //Expand the item
            ExpandDirectoryTreeItem(_item, aFilters);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the files of a directory and only adds them if they match the filter
        /// </summary>
        private static void AddFilesWithFilters(DirectoryTreeItem aTreeItem, string[] aFilters, DirectoryInfo aExpandedDirectory)
        {
            foreach(string _fileEnding in aFilters)
            {
                foreach(FileInfo _file in aExpandedDirectory.GetFiles(FilterChecker.InsertFilterForAll(_fileEnding)))
                {
                    aTreeItem.Items.Add(CreateTreeItem(_file));
                }
            }
        }

        /// <summary>
        /// Creates & returns a templatable DirectoryTreeItem that can be expanded
        /// </summary>
        private static DirectoryTreeItem CreateExpandableTreeItem(object aItemBase)
        {
            DirectoryTreeItem _item = CreateTreeItem(aItemBase);
            _item.Items.Add(LoadingString);
            return _item;
        }


        /// <summary>
        /// Creates a templatable DirectoryTreeItem that can't be expanded
        /// </summary>
        private static DirectoryTreeItem CreateTreeItem(object aItemBase)
        {
            DirectoryTreeItem _item;
            switch(aItemBase)
            {
                case DriveInfo _:
                    _item = new DriveItem();
                    break;

                case DirectoryInfo _:
                    _item = new DirectoryItem();
                    break;

                case FileInfo _:
                    _item = new FileItem();
                    break;

                default:
                    _item = new DirectoryTreeItem();
                    break;
            }

            _item.Header = aItemBase.ToString();
            //By addings the Directory/Drive/FileInfo we base the item on as a tag we can access it easily during expansion / other checks
            _item.Tag = aItemBase;
            return _item;
        }

        /// <summary>
        /// Performs the actual expansion on the DirectoryTreeItem based on the Drive/DirectoryInfo tag
        /// </summary>
        private static void ExpandDirectoryTreeItem(DirectoryTreeItem aItem, string[] aFilters = null)
        {
            DirectoryInfo _expandedDirectory = null;
            switch(aItem.Tag)
            {
                case DriveInfo _driveInfo:
                    _expandedDirectory = _driveInfo.RootDirectory;
                    break;

                case DirectoryInfo _directoryInfo:
                    _expandedDirectory = _directoryInfo;
                    break;
            }

            //If we've managed to expand a node that's not a directory for some reason, return
            if(_expandedDirectory == null)
            {
                return;
            }

            //Try accessing and adding the different subdirectories & files to the tree view
            try
            {
                //Add the directories
                foreach(DirectoryInfo _subDirectory in _expandedDirectory.GetDirectories())
                {
                    aItem.Items.Add(CreateExpandableTreeItem(_subDirectory));
                }

                //Depending on if we've passed filters or not, run the different filters individually or simply add all files
                if(aFilters != null)
                {
                    AddFilesWithFilters(aItem, aFilters, _expandedDirectory);
                }
                else
                {
                    foreach(FileInfo _file in _expandedDirectory.GetFiles("*"))
                    {
                        aItem.Items.Add(CreateTreeItem(_file));
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Recursively filters a DirectoryTreeItem and all its sub-nodes that are directories
        /// </summary>
        private static void RecursiveFilter(DirectoryTreeItem aTreeItem, string[] aFilters)
        {
            //Perform the same Directory check we do as we normally expand a DirectoryTreeItem to get the DirectoryInfo
            DirectoryInfo _expandedDirectory = null;
            switch(aTreeItem.Tag)
            {
                case DriveInfo _driveInfo:
                    _expandedDirectory = _driveInfo.RootDirectory;
                    break;

                case DirectoryInfo _directoryInfo:
                    _expandedDirectory = _directoryInfo;
                    break;
            }

            if(_expandedDirectory == null)
            {
                return;
            }

            //Begin by seeing if there's any expanded directories we should continue down into
            foreach(var _item in aTreeItem.Items)
            {
                //Since files are added after directories we continue until we start hitting them
                if(!(_item is DirectoryItem _directory))
                {
                    break;
                }

                //If the directory isn't expanded, ignore it
                if(_directory.Items.Count == 1 && _directory.Items[0] is string)
                {
                    continue;
                }

                //We also might as well ignore dwelling into expanded but empty directories
                if(_directory.Items.Count > 0)
                {
                    RecursiveFilter(_directory, aFilters);
                }
            }

            //Then remove all the files from the item
            //Like with directories above we can start at the end and work on the files until we reach the directories or the start of the added items
            for(int _index = aTreeItem.Items.Count - 1; _index >= 0; _index--)
            {
                if(!(aTreeItem.Items[_index] is FileItem))
                {
                    break;
                }

                aTreeItem.Items.RemoveAt(_index);
            }

            try
            {
                //Finally re-add the files for each filter
                AddFilesWithFilters(aTreeItem, aFilters, _expandedDirectory);
            }
            catch
            {
                //ignored
            }
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Dummy class for a Directory in the tree view
    /// </summary>
    public class DirectoryItem: DirectoryTreeItem
    {
    }

    /// <summary>
    /// DataTemplateSelector based on the Drive / Directory / File dummy classes
    /// </summary>
    public class DirectoryTemplateSelector: DataTemplateSelector
    {
        #region Public Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(container is FrameworkElement _element && item != null && item is DirectoryTreeItem _treeViewItem)
            {
                switch(_treeViewItem)
                {
                    case DirectoryItem _:
                        return _element.FindResource("DirectoryTemplate") as HierarchicalDataTemplate;

                    case DriveItem _:
                        return _element.FindResource("DriveTemplate") as HierarchicalDataTemplate;

                    case FileItem _:
                        return _element.FindResource("FileTemplate") as DataTemplate;
                }
            }

            return null;
        }

        #endregion Public Methods
    }

    public class DirectoryTreeItem
    {
        #region Public Properties

        public string Header { get; set; }

        public ObservableCollection<object> Items { get; set; } = new ObservableCollection<object>();
        public object Tag { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Dummy class for a Drive in the tree view
    /// </summary>
    public class DriveItem: DirectoryTreeItem
    {
    }
    /// <summary>
    /// Dummy class for a File in the tree view
    /// </summary>
    public class FileItem: DirectoryTreeItem
    {
    }
}