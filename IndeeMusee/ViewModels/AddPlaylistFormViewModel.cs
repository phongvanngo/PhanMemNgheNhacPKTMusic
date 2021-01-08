using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace IndeeMusee.ViewModels
{
    class AddPlaylistFormViewModel : BaseViewModel
    {
        SongModel songDetail;

        List<PlaylistModel> listPlaylist;

        string searchKeyword ="";

        public List<PlaylistModel> FilteredPlaylist { get; set; }

        public string SearchKeyword 
        {
            get => searchKeyword;
            set 
            {
                searchKeyword = value;
                OnPropertyChanged("SearchKeyword");

                List<PlaylistModel> newListPlaylist = new List<PlaylistModel>();
                foreach (PlaylistModel playlist in listPlaylist)
                {
                    string playlist_name = IndeeMusee.MyUtilites.MyFunction.RemoveUnicode(playlist.PlaylistName.ToString().Trim());
                    string keyword = IndeeMusee.MyUtilites.MyFunction.RemoveUnicode(searchKeyword.Trim());
                    if (playlist_name.Contains(keyword))
                    {
                        newListPlaylist.Add(playlist);
                    }
                }

                FilteredPlaylist = newListPlaylist;
                OnPropertyChanged("FilteredPlaylist");
                ListPlaylistChange();
            } 
        }

        public ICommand AddToPLCommand { get; set; }
        public ICommand RemoveFromPLCommand { get; set; }
        public List<PlaylistModel> ListPlaylist { get => listPlaylist; set => listPlaylist = value; }

        public event NotifyHandler ListPlaylistChange;

        public event AddToPlaylistStatusHandler ButtonStatusChange;


        public AddPlaylistFormViewModel(SongModel song)
        {
            songDetail = song;

            SetupListPlaylist();

            FilteredPlaylist = listPlaylist;

            AddToPLCommand = new RelayCommand<object>(
                (p) =>
                {
                    return true;
                },
                (p) =>
                {
                    PlaylistModel playlist = p as PlaylistModel;

                    SongDataAccess.InsertSongToPlaylist(songDetail.SongKey,playlist.PlaylistKey);

                    int index = ListPlaylist.FindIndex(e => e.PlaylistKey == playlist.PlaylistKey);
                    ListPlaylist[index].CheckExistedInPlaylist = true;
                    listPlaylist[index].AmountSong++;

                    index = FilteredPlaylist.FindIndex(e => e.PlaylistKey == playlist.PlaylistKey);
                    FilteredPlaylist[index].CheckExistedInPlaylist = true;
                    FilteredPlaylist[index].AmountSong++;
                    
                    

                    ButtonStatusChange(index);

                    OnPropertyChanged("AmountSong");

                });
            RemoveFromPLCommand = new RelayCommand<object>(
                (p) =>
                {
                    return true;
                },
                (p) =>
                {
                    PlaylistModel playlist = p as PlaylistModel;
                    SongDataAccess.DeleteSongFromPlaylist(songDetail.SongKey,playlist.PlaylistKey);

                    int index = ListPlaylist.FindIndex(e => e.PlaylistKey == playlist.PlaylistKey);
                    ListPlaylist[index].CheckExistedInPlaylist = false;
                    listPlaylist[index].AmountSong--;

                    index = FilteredPlaylist.FindIndex(e => e.PlaylistKey == playlist.PlaylistKey);
                    FilteredPlaylist[index].CheckExistedInPlaylist = false;
                    FilteredPlaylist[index].AmountSong--;

                    ButtonStatusChange(index);
                    OnPropertyChanged("AmountSong");

                });
        }

        void SetupListPlaylist()
        {
            //update amout of song
            //create playlist image

            /// IndeeMusee; component / Assets / my music.png
            /// 
            listPlaylist = SongDataAccess.GetAllPlaylist();

            foreach (PlaylistModel playlist in listPlaylist)
            {
                //load hình ảnh
                string imagePath = $@"{GeneralDataManagement.ImageFolderPath}\{playlist.ImagePath}";
                if (File.Exists(imagePath) == true)
                {
                    playlist.PlaylistImage = new BitmapImage(new Uri(imagePath));
                }
                else
                {
                    playlist.PlaylistImage = new BitmapImage(new Uri("IndeeMusee; component/Assets/Group 6.png", UriKind.Relative));
                }

                //load số bài hát
                playlist.AmountSong = SongDataAccess.GetAmountOfSongInPlaylist(playlist.PlaylistKey);

                //kiem tra bai hat co ton tai trong playlist hay ko
                playlist.CheckExistedInPlaylist = SongDataAccess.CheckSongExistInPlaylist(songDetail.SongKey,playlist.PlaylistKey);
            }

            OnPropertyChanged("ListPlaylist");

        }
    }
}