using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TracerOnWPF.Commands;
using TracerOnWPF.Model;
using TracerOnWPF.View;

namespace TracerOnWPF.ViewModel
{
    class TabsViewModel : TabPageModel, INotifyPropertyChanged
    {
        public TabsViewModel()
        {
            TabsList = new ObservableCollection<TabPageModel>();
        }

        private static ObservableCollection<TabPageModel> TabsList = new ObservableCollection<TabPageModel>();
        public static ObservableCollection<TabPageModel> Tabs
        {
            get { return TabsList; }
            set { TabsList = value; }
        }

        public static void AddNewTabItem(string header, ObservableCollection<TreeItemModel> items)
        {
            TabsList.Add(new TabPageModel(header, items));
        }

        public static void RemoveTabItem(int index)
        {
            TabsList.RemoveAt(index);
        }
    }
}
