using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using IndeeMusee.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IndeeMusee.ViewModels
{
    class PlaylistInfoViewModel : BaseViewModel
    {

        PlaylistModel playlistDetail;

        string playlistName;

        string totalTime;

        ImageSource playlistImage = new BitmapImage(new Uri("/IndeeMusee;component/Assets/Group 6.png", UriKind.Relative));

        string amountOfSong = "0";

        public event ChangePageHandler GoBack;

        public ICommand PlayAllCommand { get; set; }
        public ICommand RemovePlaylistCommand { get; set; }
        public ICommand AddSongCommand { get; set; }
        public string TotalTime { get => totalTime; set => totalTime = value; }
        public string AmountOfSong { get => amountOfSong; set => amountOfSong = value; }
        public PlaylistModel PlaylistDetail { get => playlistDetail; set => playlistDetail = value; }
        public string PlaylistName { get => playlistName; set => playlistName = value; }
        public ImageSource PlaylistImage { get => playlistImage; set => playlistImage = value; }

        public static string currentPlaylistKey;

        public PlaylistInfoViewModel(string PLKey)
        {
            playlistDetail = SongDataAccess.GetPlaylist(PLKey);
            PlaylistName = playlistDetail.PlaylistName;

            currentPlaylistKey = PLKey;

            //cập nhật hình của playlist

            if (File.Exists($@"{GeneralDataManagement.ImageFolderPath}\{playlistDetail.ImagePath}") == true)
            {
                PlaylistImage = new BitmapImage(new Uri($@"{GeneralDataManagement.ImageFolderPath}\{playlistDetail.ImagePath}"));
                OnPropertyChanged("PlaylistImage");
            }

            //load các bài hát của playlist cho list music
            MusicInPlaylistControl.PlaylistKey = playlistDetail.PlaylistKey;
            MusicInPlaylistControl.FetchUserSongListData();
            MusicInPlaylistControl.CurrentSongListChange += MusicInPlaylistControl_CurrentSongListChange;

            totalTime = MusicInPlaylistControl.CalculateTotalMinute().ToString() + " minutes";
            amountOfSong = MusicInPlaylistControl.TotalSong.ToString() + " songs";

            PlayAllCommand = new RelayCommand<object>((p) => { return true; }, (p) => { UpNextListModel.PlayAllSongList(MusicInPlaylistControl.CurrentSongList); });
            RemovePlaylistCommand = new RelayCommand<object>(
                (p) =>
                {
                    return true;
                }, (p) =>
            {
                if (playlistDetail.PlaylistKey == "FAVORITE_PLAYLIST")
                {
                    MessageBox.Show("Không thể xóa danh sách phát này");
                    return;
                }
                MessageBoxResult messageBoxResult = MessageBox.Show($@"Bạn có chắc chắn xóa Playlist này ?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    SongDataAccess.DropPlaylistFromDatabase(playlistDetail.PlaylistKey);
                    GoBack("Playlist");
                }

            });
            AddSongCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ListMusicDialog listMusicForm = new ListMusicDialog(playlistDetail.PlaylistKey);
                if (listMusicForm.ShowDialog() == true)
                {
                }
            });

        }

        private void MusicInPlaylistControl_CurrentSongListChange()
        {
            totalTime = MusicInPlaylistControl.CalculateTotalMinute().ToString() + " minutes";
            amountOfSong = MusicInPlaylistControl.TotalSong.ToString() + " song";
            OnPropertyChanged("TotalTime");
            OnPropertyChanged("AmountOfSong");
        }

        public void ChangePlaylistName()
        {
            EditPlayListNameForm editPlayListNameForm = new EditPlayListNameForm(playlistDetail.PlaylistName);

            editPlayListNameForm.ShowDialog();
            if (editPlayListNameForm.DialogResult == true)
            {
                playlistDetail.PlaylistName = editPlayListNameForm.NewPlaylistName;
                SongDataAccess.UpdatePlaylist(PlaylistDetail);
                PlaylistName = playlistDetail.PlaylistName;
                OnPropertyChanged("PlaylistName");
            }

        }
    }
}

