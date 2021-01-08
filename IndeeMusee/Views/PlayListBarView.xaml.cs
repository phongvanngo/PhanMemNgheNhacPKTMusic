using IndeeMusee.ViewModels;
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

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PlayListBarView : UserControl
    {
        PlaylistBarViewModel PlaylistBarVM = new PlaylistBarViewModel(); 
        public PlayListBarView()
        {
            InitializeComponent();
            this.DataContext = PlaylistBarVM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            MainGrid.Opacity = 1;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            MainGrid.Opacity = 0.5;
        }
    }
}
