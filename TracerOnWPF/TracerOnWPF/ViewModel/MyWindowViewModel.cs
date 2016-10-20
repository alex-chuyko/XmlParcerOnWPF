using Microsoft.Win32;
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
using System.Xml;
using System.Xml.Linq;
using TracerOnWPF.Commands;
using TracerOnWPF.Model;
using TracerOnWPF.View;

namespace TracerOnWPF.ViewModel
{
    class MyWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TabItem> TabsList { get; set; }
        private Model.Frame frameList;
        private List<string> listFileName = new List<string>();
        private string fileName;
        private int index = 0;

        public MyWindowViewModel() { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string p_propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p_propertyName));
            }
        }

        private ICommand openFileCommand;
        public ICommand OpenFileCommand
        {
            get
            {
                if (openFileCommand == null)
                    openFileCommand = new RelayCommand(p => OpenFile());
                return openFileCommand;
            }
        }

        private ICommand saveFileCommand;
        public ICommand SaveFileCommand
        {
            get
            {
                if (saveFileCommand == null)
                    saveFileCommand = new RelayCommand(p => BuildXml());
                return saveFileCommand;
            }
        }

        private ICommand saveAsFileCommand;
        public ICommand SaveAsFileCommand
        {
            get
            {
                if (saveAsFileCommand == null)
                    saveAsFileCommand = new RelayCommand(p => SaveAsFile());
                return saveAsFileCommand;
            }
        }

        private ICommand removeTab;
        public ICommand RemoveTab
        {
            get
            {
                if (removeTab == null)
                    removeTab = new RelayCommand(p => RemoveTabItem());
                return removeTab;
            }
        }

        private ICommand exit;
        public ICommand Exit
        {
            get
            {
                if (exit == null)
                    exit = new RelayCommand(p => ExitProgramm());
                return exit;
            }
        }

        private void ExitProgramm()
        {
            if(index > 0)
            {
                TabPageModel tabPage = MainWindow.tabControl.SelectedContent as TabPageModel;
                ObservableCollection<TabPageModel> pageList = MainWindow.tabControl.ItemsSource as ObservableCollection<TabPageModel>;
                int i = 0;
                while(pageList.Count != 0)
                {
                    MainWindow.tabControl.SelectedIndex = i;
                    if(pageList[i].Header[pageList[i].Header.Length - 1] == '*')
                    {
                        var result = MessageBox.Show("Save file " + pageList[i].Header + "?", "Save", MessageBoxButton.YesNoCancel);
                        if (result == MessageBoxResult.Yes)
                        {
                            BuildXml();
                            closeTab();
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            closeTab();
                        }
                    }
                    else
                    {
                        closeTab();
                    }
                }
            }
            Application.Current.Shutdown();
        }

        private void RemoveTabItem()
        {
            if(index > 0)
            {
                TabPageModel tabPage = MainWindow.tabControl.SelectedContent as TabPageModel;
                if (tabPage.Header[tabPage.Header.Length - 1] == '*')
                {
                    var result = MessageBox.Show("Save file " + tabPage.Header + "?", "Save", MessageBoxButton.YesNoCancel);
                    if (result == MessageBoxResult.Yes)
                    {
                        BuildXml();
                        closeTab();
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        closeTab();
                    }
                }
                else
                {
                    closeTab();
                }
            }
        }

        private void closeTab()
        {
            int indexItem = MainWindow.tabControl.SelectedIndex;
            TabsViewModel.RemoveTabItem(indexItem);
            listFileName.RemoveAt(indexItem);
            index--;
        }

        private void SaveAsFile()
        {
            TabPageModel tabPage = MainWindow.tabControl.SelectedContent as TabPageModel;
            if (index != 0)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "XML files (*.xml)|*.xml";
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == true)
                {
                    fileName = saveFileDialog1.FileName;
                    if (Path.GetExtension(fileName) != ".xml")
                    {
                        MessageBox.Show("This is not xml file!", "Save error!");
                        return;
                    }
                    tabPage.FullPath = saveFileDialog1.FileName;
                    tabPage.Header = Path.GetFileName(saveFileDialog1.FileName);
                    listFileName[MainWindow.tabControl.SelectedIndex] = saveFileDialog1.FileName;
                    BuildXml();
                }
            }
            else
            {
                MessageBox.Show("Open file!");
            }
        }

        private void BuildXml()
        {
            if (index != 0)
            {
                XDocument xmlDoc = new XDocument();
                XElement xmlRoot = new XElement("root");
                XElement xmlThread;
                XAttribute attr;
                TabPageModel tabPage = MainWindow.tabControl.SelectedContent as TabPageModel;
                if (tabPage.Header[tabPage.Header.Length - 1] == '*')
                    tabPage.Header = Path.GetFileName(tabPage.FullPath);
                foreach (TreeItemModel tim in tabPage.Items)
                {
                    if (tim.ItemName == "thread")
                    {
                        xmlThread = new XElement(tim.ItemName);
                        attr = new XAttribute("id", tim.Id);
                        xmlThread.Add(attr);
                        attr = new XAttribute("time", tim.Time);
                        xmlThread.Add(attr);
                        addNodeXml(tim, ref xmlThread);
                        xmlRoot.Add(xmlThread);
                    }
                }
                xmlDoc.Add(xmlRoot);
                xmlDoc.Save(tabPage.FullPath);
            }
        }

        private void addNodeXml(TreeItemModel item, ref XElement thread)
        {
            foreach(TreeItemModel tempItem in item.Items)
            {
                XElement xmlMethod = new XElement(tempItem.ItemName);
                XAttribute attr;
                attr = new XAttribute("name", tempItem.Name);
                xmlMethod.Add(attr);
                attr = new XAttribute("time", tempItem.Time);
                xmlMethod.Add(attr);
                attr = new XAttribute("package", tempItem.Package);
                xmlMethod.Add(attr);
                attr = new XAttribute("paramsCount", tempItem.ParamsCount);
                xmlMethod.Add(attr);
                addNodeXml(tempItem, ref xmlMethod);

                thread.Add(xmlMethod);
            }
        }

        private void parse(XmlNode node, ref Model.Frame frame, int threadId, ref int nodeId)
        {
            int i = 0;
            foreach (XmlNode node1 in node)
            {
                nodeId++;
                frame.childFrame[i] = new Model.Frame();
                if (node1.Name == "thread")
                {
                    threadId++;
                    frame.childFrame[i].parentId = -1;
                }
                else
                {
                    frame.childFrame[i].parentId = frame.nodeId;
                }
                frame.childFrame[i].nodeId = nodeId;
                frame.childFrame[i].threadId = threadId;
                frame.childFrame[i].name = node1.Name;
                Array.Resize<Model.Frame>(ref frame.childFrame, frame.childFrame.Length + 1);
                int j = 0;
                if(node1.Name == "thread")
                {
                    frame.childFrame[i].attrId = Int32.Parse(node1.Attributes[0].Value);
                    frame.childFrame[i].attrTime = Int32.Parse(node1.Attributes[1].Value);
                }
                else
                {
                    frame.childFrame[i].attrName = node1.Attributes[0].Value;
                    frame.childFrame[i].attrTime = Int32.Parse(node1.Attributes[1].Value);
                    frame.childFrame[i].attrPackage = node1.Attributes[2].Value;
                    frame.childFrame[i].attrParamsCount = Int32.Parse(node1.Attributes[3].Value);
                }
                foreach (XmlAttribute attr in node1.Attributes)
                {
                    frame.childFrame[i].attr[j] = new MyAttribute(attr.Name, attr.Value);
                    j++;
                    Array.Resize<MyAttribute>(ref frame.childFrame[i].attr, j + 1);
                }
                parse(node1, ref frame.childFrame[i], threadId, ref nodeId);
                i++;
            }
        }

        private void buildTreeView(Model.Frame frame, ref TreeItemModel item)
        {
            TreeViewItem newNode;
            TreeItemModel temp;
            TreeItemModel newItem = new TreeItemModel();
            for (int i = 0; i < frame.childFrame.Length - 1; i++)
            {
                newNode = new TreeViewItem();
                temp = new TreeItemModel(frame.childFrame[i].text);
                item.Items.Add(temp);
                item.Items.Last().ItemName = frame.childFrame[i].name;
                item.Items.Last().Time = frame.childFrame[i].attrTime;
                item.Items.Last().Name = frame.childFrame[i].attrName;
                item.Items.Last().Id = frame.childFrame[i].attrId;
                item.Items.Last().Package = frame.childFrame[i].attrPackage;
                item.Items.Last().ParamsCount = frame.childFrame[i].attrParamsCount;
                item.Items.Last().Title = frame.childFrame[i].name;
                item.Items.Last().ThreadId = frame.childFrame[i].threadId;
                item.Items.Last().Title += addAttribute(frame.childFrame[i]);
                newItem = item.Items.Last();
                buildTreeView(frame.childFrame[i], ref newItem);
            }
        }

        private string addAttribute(Model.Frame frame)
        {
            string text = "";
            text += " ( ";
            for (int j = 0; j < frame.attr.Length - 1; j++)
            {
                text += frame.attr[j].name + "=" + frame.attr[j].value + "; ";
            }
            text += ")";

            return text;
        }

        private void OpenFile()
        {
            XmlDocument doc = new XmlDocument();
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            ObservableCollection<TreeItemModel> listFrame = new ObservableCollection<TreeItemModel>();

            openFileDialog1.InitialDirectory = @"C:\Users\Александр\Documents\Visual Studio 2015\Projects\SPP\Lab2\TracerOnWinForms\TracerOnWinForms\bin\Debug";
            openFileDialog1.Filter = "XML files (*.xml)|*.xml";
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            doc.Load(openFileDialog1.FileName);
                            fileName = openFileDialog1.FileName;
                        }

                        if (!listFileName.Contains(fileName))
                        {
                            int nodeId = -1;
                            listFileName.Add(fileName);
                            foreach (XmlNode node in doc.SelectNodes("root"))
                            { 
                                frameList = new Model.Frame();
                                frameList.name = "root";
                                parse(node, ref frameList, -1, ref nodeId);
                            }

                            index++;

                            TreeItemModel tiModel = new TreeItemModel();
                            TreeItemModel tempTIModel;
                            for (int i = 0; i < frameList.childFrame.Length - 1; i++)
                            {
                                tempTIModel = new TreeItemModel();
                                tempTIModel.Title = frameList.childFrame[i].text;
                                listFrame.Add(tempTIModel);
                                listFrame.Last().ItemName = frameList.childFrame[i].name;
                                listFrame.Last().Time = frameList.childFrame[i].attrTime;
                                listFrame.Last().Name = frameList.childFrame[i].attrName;
                                listFrame.Last().Id = frameList.childFrame[i].attrId;
                                listFrame.Last().Package = frameList.childFrame[i].attrPackage;
                                listFrame.Last().ParamsCount = frameList.childFrame[i].attrParamsCount;
                                listFrame.Last().Title = frameList.childFrame[i].name;
                                listFrame.Last().ThreadId = frameList.childFrame[i].threadId;
                                listFrame.Last().Title += addAttribute(frameList.childFrame[i]);

                                tiModel = listFrame.Last();
                                buildTreeView(frameList.childFrame[i], ref tiModel);
                            }

                            TabsViewModel.AddNewTabItem(openFileDialog1.FileName, listFrame);
                            TabsViewModel.Tabs.Last().IsSelected = true;
                        }
                        else
                        {
                            MessageBox.Show("File " + Path.GetFileName(fileName) + " is alredy open!");
                            int ind = listFileName.IndexOf(fileName);
                            TabsViewModel.Tabs[ind].IsSelected = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
    }
}
