using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IndeeMusee.ViewModels
{

    class NowPlayingViewModel : BaseViewModel
    {
        List<SongModel> song_list = new List<SongModel>();

        SongModel selectedSong;

        int selectedSongIndex = 0;

        bool isShuffle = false;

        public ICommand SelectAudio { get; set; }
        public ICommand RemoveSongCommand { get; set; }
        public ICommand ChoseMusicToPlay { get; set; }
        public ICommand ShuffleCommand { get; set; }

        public int MainHeight { get; set; }

        public NowPlayingViewModel()
        {
            SongList = UpNextListModel.UpNextList;

            UpNextListModel.UpNextListChange += UpNextListModel_UpNextListChange;
            SelectAudio = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                //MessageBox.Show(p.ToString());
            });

            ShuffleCommand = new RelayCommand<object>((p) => { return true; }, (p) => { IsShuffle = !IsShuffle; NowPlayingSong.IsShuffle = IsShuffle; });
            RemoveSongCommand = new RelayCommand<object>((p) => { return true; }, (p) => {
                SongModel songRemove = p as SongModel;
                UpNextListModel.RemoveSongOutOfUpNextList(songRemove);
            });            
            ChoseMusicToPlay = new RelayCommand<object>((p) => { return true; }, (p) => {
                if (UpNextListModel.IsEditingLyric == true) return;
                SongModel songToPlaying = p as SongModel;
                UpNextListModel.GoToSongInPlayingQueue(songToPlaying);
            });

        }


        public List<SongModel>
        SongList
        {
            get => UpNextListModel.UpNextList;
            set => song_list = value;
        }
        private void UpNextListModel_UpNextListChange()
        {
            OnPropertyChanged("SongList");
        }
        public SongModel SelectedSong { get => selectedSong; set { selectedSong = value; } }

        public int SelectedSongIndex { get => selectedSongIndex; set => selectedSongIndex = value; }
        public bool IsShuffle { get => isShuffle; set => isShuffle = value; }

        public void DoubleClickAudio(SongModel selectedSong)
        {
            UpNextListModel.GoToSongInPlayingQueue(selectedSong);
        }        
        
        public void ChoseSongToPlayInQueue(int index)
        {
            if (index >= 0 && index < SongList.Count) 
                UpNextListModel.GoToSongInPlayingQueue(SongList[index]);
        }

        public void RemoveSongOutOfQueue(int index)
        {
            UpNextListModel.RemoveSongOutOfUpNextList(SongList[index]);
        }

        public void PlayNextSong()
        {

        }

        //#region Bổ sung vào NowPlayingView
        ////code behind
        //private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    (DataContext as ViewModels.NowPlayingViewModel).DoubleClickAudio((sender as ListViewItem).Content as Models.SongModel);
        //}

        ////view 
        //    <ListView.ItemContainerStyle>
        //       <EventSetter Event = "MouseDoubleClick" Handler="ListViewItem_MouseDoubleClick"/>
        //    </ListView.ItemContainerStyle>
        //#endregion



    }
}
