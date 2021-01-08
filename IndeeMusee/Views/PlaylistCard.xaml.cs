using IndeeMusee.DataManager;
using IndeeMusee.Models;
using System;
using System.Collections.Generic;
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

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    public partial class PlaylistCard : UserControl
    {
        public string PlaylistKey { get; set; }

        public static event ChangePlaylistInfoViewHandler ClickPlaylistCardEvent;
        public PlaylistCard(PlaylistModel playlist)
        {
            InitializeComponent();
            PlaylistKey = playlist.PlaylistKey;
            TbPlaylistName.Text = playlist.PlaylistName;

            if(File.Exists($@"{GeneralDataManagement.ImageFolderPath}\{playlist.ImagePath}")==true)
            {
                PlaylistImage.Source = new BitmapImage(new Uri($@"{GeneralDataManagement.ImageFolderPath}\{playlist.ImagePath}"));
                PlaylistImage.Width = Double.NaN;
                PlaylistImage.Height = Double.NaN;

                PlaylistImage.Stretch = Stretch.Fill;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClickPlaylistCardEvent(PlaylistKey);
        }
    }
}
