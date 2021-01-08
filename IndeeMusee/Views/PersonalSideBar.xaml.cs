using System;
using System.Collections.Generic;
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
using IndeeMusee.ViewModels;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for PersonalSideBar.xaml
    /// </summary>
    public partial class PersonalSideBar : UserControl
    {
        SidebarViewModel sidebarVM = new SidebarViewModel();
        public PersonalSideBar()
        {
            InitializeComponent();
            this.DataContext = sidebarVM;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            MainGrid.Opacity = 1;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            MainGrid.Opacity = 0.5;
        }
    }
}
