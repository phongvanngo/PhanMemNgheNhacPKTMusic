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
    /// Interaction logic for CreatePlayList.xaml
    /// </summary>
    public partial class CreatePlayList : UserControl
    {
        string playlistName = "";
        public CreatePlayList()
        {
            InitializeComponent();
        }

        public string PlaylistName { get => playlistName; set => playlistName = value; }

        private void PlayListName_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TbPlayListName.Text != null)
                LblNon.Visibility = Visibility.Visible;
            else
                LblNon.Visibility = Visibility.Hidden;
        }

        private void BtnCreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            playlistName = TbPlayListName.Text;
            //this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}