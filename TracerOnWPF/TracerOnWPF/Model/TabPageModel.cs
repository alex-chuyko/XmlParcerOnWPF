using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TracerOnWPF.Commands;

namespace TracerOnWPF.Model
{
    class TabPageModel : INotifyPropertyChanged
    {
        public TabPageModel(string header, ObservableCollection<TreeItemModel> newItems)
        {
            Header = Path.GetFileName(header);
            FullPath = header;
            if (Items == null)
                Items = new ObservableCollection<TreeItemModel>();
            Items = newItems;
        }

        public TabPageModel() { }

        private static TreeItemModel selectedItem;
        public static TreeItemModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
            }
        }

        private ObservableCollection<TreeItemModel> items = new ObservableCollection<TreeItemModel>();
        public ObservableCollection<TreeItemModel> Items
        {
            get { return items; }
            set { items = value; OnPropertyChanged("Items"); }
        }

        private string header;
        public string Header
        {
            get { return header; }
            set { header = value; OnPropertyChanged("Header"); }
        }

        private string fullPath;
        public string FullPath
        {
            get { return fullPath; }
            set { fullPath = value; OnPropertyChanged("FullPath"); }
        }

        private bool selectFlag;
        public bool IsSelected
        {
            get { return selectFlag; }
            set
            {
                if(value != selectFlag)
                    selectFlag = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string p_propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p_propertyName));
            }
        }
    }
}
