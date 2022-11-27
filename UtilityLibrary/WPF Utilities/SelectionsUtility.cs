using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityLibrary.WPF_Utilities
{
    public static class SelectionsUtility
    {
        public static IEnumerable<int> GetIndexesOfSelectedItems(ListBox aListBox)
        {
            if (aListBox.SelectionMode == SelectionMode.Single)
                return new[]{aListBox.SelectedIndex};


            return (from object _selectedItem in aListBox.SelectedItems where _selectedItem != null select aListBox.Items.IndexOf(_selectedItem));
        }
        
    }
}