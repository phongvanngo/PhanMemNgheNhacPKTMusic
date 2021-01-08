using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
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
    /// Interaction logic for MiniTool.xaml
    /// </summary>
    public partial class MiniTool : UserControl
    {
        static SongModel songDetail;

        //public string ItemKey { get; set; }
        public MiniTool()
        {
            InitializeComponent();
        }
        bool IsFavorSong = false;

        public static SongModel SongDetail { get => songDetail; set => songDetail = value; }


        private void btnEditSong_Click(object sender, RoutedEventArgs e)
        {

            EditForm editForm = new EditForm(SongDetail);
            editForm.ShowDialog();

        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult messageBoxResult = MessageBox.Show($@"Loại bỏ bài hát {SongDetail.Title} ra khỏi ứng dụng?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SongDataAccess.DeleteSongFromDatabase(songDetail.SongKey);

                try
                {
                    MyMusicControl.UserSongListHasUpdated();
                    MusicInPlaylistControl.UserSongListHasUpdated();
                }
                catch (Exception)
                {

                }
            }
        }

        private void btnLyric_Click(object sender, RoutedEventArgs e)
        {
            EditLyric editLyricForm = new EditLyric(SongDetail);
            editLyricForm.ShowDialog();
        }

        private void BtnAddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            AddPlayListForm addlaylistForm = new AddPlayListForm(SongDetail);
            addlaylistForm.Show();
        }

        private void AddToPLButton_Click(object sender, RoutedEventArgs e)
        {
            AddPlayListForm AddToPLForm = new AddPlayListForm(songDetail);
            AddToPLForm.ShowDialog();
        }

    }
}
