using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAppPraktika.Helper;
using WpfAppPraktika.Model;
using WpfAppPraktika.ViewModel;
using static System.Net.Mime.MediaTypeNames;

namespace WpfAppPraktika.View
{
    public partial class WindowEmployee : Window
    {

        public WindowEmployee()
        {
            InitializeComponent();
            DataContext = new PersonViewModel();
        }
    }

}
