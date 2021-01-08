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
    /// Interaction logic for ExploreView.xaml
    /// </summary>
    public partial class ExploreView : UserControl
    {
        public ExploreView()
        {
            InitializeComponent();
            ExploreMusicViewModel.ToggleLoadingModeRequest += ExploreMusicViewModel_ToggleLoadingModeRequest;
        }

        private void ExploreMusicViewModel_ToggleLoadingModeRequest()
        {
            if ((this.DataContext as ExploreMusicViewModel).IsLoading == true)
            {
                loadingGif.Visibility = Visibility.Visible;
            }
            else
            {
                loadingGif.Visibility = Visibility.Hidden;
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {

        }
    }
}
