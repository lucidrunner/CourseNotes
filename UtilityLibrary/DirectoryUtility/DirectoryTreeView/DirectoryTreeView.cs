//Björn Rundquist 12/10-2020

using System.IO;
using System.Windows.Controls;

namespace UtilityLibrary.DirectoryUtility.DirectoryTreeView
{
    //Currently using the DirectoryTemplateTreeView which allows for
    //For transparency: This is largely based on the wpf-tutorials expandable directory tree view example
    //The actual version I'm using expands on this to allow templating
    public static class DirectoryTreeView
    {
        #region Public Methods

        /// <summary>
        /// Populates a TreeView with the drives on the computer as expandable base nodes
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
        /// Populates a TreeView with the provided base directory as the expandable base node
        /// </summary>
        public static void CreateTreeView(TreeView aTreeView, DirectoryInfo aBaseDirectory)
        {
            aTreeView.Items.Clear();
            aTreeView.Items.Add(CreateExpandableTreeItem(aBaseDirectory));
        }

        /// <summary>
        /// For a first-time expanded node, loads its subnodes
        /// </summary>
        public static void ExpandTreeViewItem(TreeViewItem aTreeViewItem, string aFilter)
        {
            TreeViewItem _item = aTreeViewItem;

            //We're only interested in adding new nodes if it hasn't been expanded before
            if(_item.Items.Count != 1 && !(_item.Items[0] is string))
            {
                return;
            }

            //Clear the dummy string child
            _item.Items.Clear();

            //Check the type of expandable node
            DirectoryInfo _expandedDirectory = null;
            switch(_item.Tag)
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
                foreach(DirectoryInfo _subDirectory in _expandedDirectory.GetDirectories())
                {
                    _item.Items.Add(CreateExpandableTreeItem(_subDirectory));
                }

                //TODO Realized as I'm turning this in that I only implemented filters in the template version so I'm gonna have to do that later
                foreach(FileInfo _file in _expandedDirectory.GetFiles("*"))
                {
                    _item.Items.Add(CreateTreeItem(_file));
                }
            }
            catch
            {
                // ignored
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Creates a TreeViewItem with a dummy subnode so we can expand it later
        /// </summary>
        private static TreeViewItem CreateExpandableTreeItem(object aItemBase)
        {
            TreeViewItem _item = CreateTreeItem(aItemBase);
            _item.Items.Add("Loading...");
            return _item;
        }

        /// <summary>
        /// Creates a TreeViewItem without a subnode so it can't be accidentally expanded
        /// </summary>
        private static TreeViewItem CreateTreeItem(object aItemBase)
        {
            TreeViewItem _item = new TreeViewItem { Header = aItemBase.ToString(), Tag = aItemBase };
            return _item;
        }

        #endregion Private Methods
    }
}