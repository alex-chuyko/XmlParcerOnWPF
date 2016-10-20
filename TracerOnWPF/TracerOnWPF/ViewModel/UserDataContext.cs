using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TracerOnWPF.ViewModel
{
    class UserDataContext
    {
        private MyWindowViewModel myWindowViewModel = new MyWindowViewModel();
        public MyWindowViewModel MyWindowViewModel
        {
            get { return myWindowViewModel; }
        }

        private TabsViewModel tabsViewModel = new TabsViewModel();
        public TabsViewModel TabsViewModel
        {
            get { return tabsViewModel; }
        }
    }
}
