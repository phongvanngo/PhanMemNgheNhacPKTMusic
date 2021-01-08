using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using IndeeMusee.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IndeeMusee.ViewModels
{
    class ListMusicInPlaylistViewModel : BaseViewModel
    {
        List<SongModel> currentSongList = new List<SongModel>();
        public ICommand PlayNowCommand { get; set; }
        public ICommand AddToPlayingQueueCommand { get; set; }
        public ICommand MoreControlCommand { get; set; }
        public ICommand AddToPlaylistCommand { get; set; }
        public ICommand PlayAllCommand { get; set; }
        public ICommand AddSongCommand { get; set; }
        public ICommand FavoriteCommand { get; set; }

        public List<SongModel> CurrentSongList
        {
            get
            {
                currentSongList = MusicInPlaylistControl.CurrentSongList;
                return currentSongList;
            }
            set { currentSongList = value; }
        }

        public event AddNewSongHandler AddNewSongEvent;
        public event ChangeSelectedIndex ButtonFavoriteSongClick;


        public ListMusicInPlaylistViewModel()
        {
            PlayAllCommand = new RelayCommand<object>((p) => { return true; }, (p) => { PlayAllSongRequest(); });
            AddToPlaylistCommand = new RelayCommand<object>((p) => { return true; }, (p) => { });

            MusicInPlaylistControl.CurrentSongListChange += MyMusicControl_CurrentSongListChange;

            FavoriteCommand = new RelayCommand<object>((p) => { return true; },
    (p) =>
    {
        SongModel songSelected = p as SongModel;
        int index = CurrentSongList.FindIndex(e => (e.SongKey == songSelected.SongKey));
        if (songSelected.IsFavorite == 1)
        {
            //bỏ thích
            SongDataAccess.RemoveSongFromFavorite(songSelected.SongKey);
            if (PlaylistInfoViewModel.currentPlaylistKey == "FAVORITE_PLAYLIST")
            {
                MusicInPlaylistControl.UserSongListHasUpdated();
            } else
                ButtonFavoriteSongClick(index, 0);
        }
        else
        {
            //thích
            SongDataAccess.InsertSongToFavorite(songSelected.SongKey);
            if (PlaylistInfoViewModel.currentPlaylistKey == "FAVORITE_PLAYLIST")
            {
                MusicInPlaylistControl.UserSongListHasUpdated();
            }
            else
                ButtonFavoriteSongClick(index, 1);
        }
    });

            PlayNowCommand = new RelayCommand<object>((p) => { return true; },
               (p) =>
               {
                   SongModel songSelected = p as SongModel;
                   int index = CurrentSongList.FindIndex(e => (e.SongKey == songSelected.SongKey));
                   PlayNowRequest(index);
               });

            MoreControlCommand = new RelayCommand<object>((p) => { return true; },
                (p) =>
                {
                    SongModel songSelected = p as SongModel;
                    MiniTool.SongDetail = songSelected;
                });
            AddToPlayingQueueCommand = new RelayCommand<object>((p) => { return true; },
                (p) =>
                {
                    SongModel songSelected = p as SongModel;
                    int index = CurrentSongList.FindIndex(e => (e.SongKey == songSelected.SongKey));
                    AddToPlayingQueueRequest(index);
                });


        }

        private void MyMusicControl_CurrentSongListChange()
        {
            OnPropertyChanged("CurrentSongList");
        }

        public void PlayAllSongRequest()
        {
            UpNextListModel.PlayAllSongList(MusicInPlaylistControl.CurrentSongList);
        }

        public void PlayNowRequest(int index)
        {
            SongModel requestedSong = currentSongList[index];
            UpNextListModel.AddSongToUpNextListAndPlay(requestedSong);

        }

        public void AddToPlayingQueueRequest(int index)
        {
            UpNextListModel.AddSongToUpNextList(currentSongList[index]);
        }
    }
}
