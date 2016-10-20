using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TracerOnWPF.Model;

namespace TracerOnWPF.ViewModel
{
    class TreeViewViewModel
    {
        private static ObservableCollection<TreeItemModel> items = new ObservableCollection<TreeItemModel>();
        
        public static ObservableCollection<TreeItemModel> Items
        {
            get { return items; }
        }

        public TreeViewViewModel()
        { }

        public TreeViewViewModel(ObservableCollection<TreeItemModel> newItems)
        {
            if (items == null)
                items = new ObservableCollection<TreeItemModel>();
            items = newItems;
        }
    }
}
