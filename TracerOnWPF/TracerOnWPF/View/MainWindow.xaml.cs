using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml;
using TracerOnWPF.Model;
using TracerOnWPF.ViewModel;

namespace TracerOnWPF.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    ///// </summary>
    public partial class MainWindow : Window
    {
        public static TabControl tabControl = new TabControl();
        ObservableCollection<TreeItemModel> list = new ObservableCollection<TreeItemModel>();
        TreeItemModel item = new TreeItemModel();

        public MainWindow()
        {
            InitializeComponent();
            tabControl = winTabControl;
        }

        public void SelectItem(object sender, RoutedEventArgs e)
        {
            TreeView tw = sender as TreeView;
            list = tw.ItemsSource as ObservableCollection<TreeItemModel>;
            item = tw.SelectedItem as TreeItemModel;
            TabPageModel.SelectedItem = item; 
            if (item.ItemName != "thread")
            {
                Name.IsEnabled = true;
                Name.Text = item.Name;
                Time.IsEnabled = true;
                Time.Text = Convert.ToString(item.Time);
                Package.IsEnabled = true;
                Package.Text = item.Package;
                ParamsCount.IsEnabled = true;
                ParamsCount.Text = Convert.ToString(item.ParamsCount);
                SaveBtn.IsEnabled = true;
            }
            else
            {
                Name.IsEnabled = false;
                Name.Text = "";
                Time.IsEnabled = false;
                Time.Text = "";
                Package.IsEnabled = false;
                Package.Text = "";
                ParamsCount.IsEnabled = false;
                ParamsCount.Text = "";
                SaveBtn.IsEnabled = false;
            }
        }

        private void changeDataInItem(ObservableCollection<TreeItemModel> items, TreeItemModel currentItem)
        {
            foreach(TreeItemModel tim in items)
            {
                if(tim.Equals(currentItem))
                {
                    tim.Name = Name.Text;
                    tim.Package = Package.Text;
                    tim.ParamsCount = Int32.Parse(ParamsCount.Text);
                    tim.Time = Int32.Parse(Time.Text);
                }
                changeDataInItem(tim.Items, currentItem);
            }
        }

        private void changeTime(TreeItemModel items, int diff)
        {
            int tempTime = 0;
            foreach(TreeItemModel tim in items.Items)
            {
                if(!tim.Equals(item))
                {
                    changeTime(tim, diff);
                    tempTime = tim.Time;
                    tim.Time = tempTime + diff;
                }
                else
                {
                    break;
                }
            }
        }

        private void test()
        {
            MessageBox.Show("test");
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((Int32.Parse(Time.Text) >= 0) && (Int32.Parse(ParamsCount.Text) >= 0))
                {
                    int oldTime = item.Time;
                    int newTime = Int32.Parse(Time.Text);
                    TabPageModel currentTab = winTabControl.SelectedItem as TabPageModel;
                    if (currentTab.Header[currentTab.Header.Length - 1] != '*')
                    {
                        foreach (TabPageModel tp in TabsViewModel.Tabs)
                        {
                            if (tp.Equals(currentTab))
                            {
                                tp.Header += "*";
                                break;
                            }
                        }
                    }
                    changeDataInItem(list, item);
                    changeTime(list[item.ThreadId], newTime - oldTime);
                    int tempTime = list[item.ThreadId].Time;
                    list[item.ThreadId].Time = tempTime + (newTime - oldTime);
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
                Name.Text = item.Name;
                Time.Text = Convert.ToString(item.Time);
                Package.Text = item.Package;
                ParamsCount.Text = Convert.ToString(item.ParamsCount);
            }
        }
    }
}
