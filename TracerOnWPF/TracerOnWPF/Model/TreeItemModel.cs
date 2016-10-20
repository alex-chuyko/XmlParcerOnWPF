using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TracerOnWPF.Model
{
    class TreeItemModel : INotifyPropertyChanged
    {
        public TreeItemModel() { }

        public TreeItemModel(string title)
        {
            Title = title;
        }

        private ObservableCollection<TreeItemModel> items = new ObservableCollection<TreeItemModel>();
        public ObservableCollection<TreeItemModel> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        private string title;
        public string Title
        {
            get { return (ItemName == "thread") 
                    ? ItemName + "(id=" + Id + "; time=" + Time + ")" 
                    : ItemName + "(name=" + Name + "; time=" + Time + "; package=" + Package + "; paramsCount=" + ParamsCount + ")"; }
            set { title = value; }
        }

        private string itemName;
        public string ItemName
        {
            get { return itemName; }
            set
            {
                itemName = value;
                OnPropertyChanged("ItemName");
                OnPropertyChanged("Title");
            }
        }

        private int id;
        public int Id
        {
            get { return (ItemName != "method") ? id : -1; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
                OnPropertyChanged("Title");
            }
        }

        private int time;
        public int Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged("Time");
                OnPropertyChanged("Title");
            }
        }

        private int paramsCount;
        public int ParamsCount
        {
            get { return (ItemName != "thread") ? paramsCount : -1; }
            set
            {
                paramsCount = value;
                OnPropertyChanged("ParamsCount");
                OnPropertyChanged("Title");
            }
        }

        private string package;
        public string Package
        {
            get { return (ItemName != "thread") ? package : ""; }
            set
            {
                package = value;
                OnPropertyChanged("Package");
                OnPropertyChanged("Title");
            }
        }

        private string name;
        public string Name
        {
            get { return (ItemName != "thread") ? name : ""; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("Title");
            }
        }

        private int threadId;
        public int ThreadId
        {
            get { return threadId; }
            set
            {
                threadId = value;
                OnPropertyChanged("ThreadId");
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
