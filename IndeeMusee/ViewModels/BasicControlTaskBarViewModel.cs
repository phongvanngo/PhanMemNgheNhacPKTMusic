using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IndeeMusee.ViewModels
{
    class BasicControlTaskBarViewModel : BaseViewModel
    {
        ImageSource playButtonImage = new BitmapImage(new Uri("/IndeeMusee;component/Assets/Group 82.png", UriKind.Relative));
        public string TimeStatus { get => NowPlayingSong.SongStatusTime; set { } }



        //Giá trị âm lượng, value của thanh âm lượng
        public double AppVolume { get => NowPlayingSong.AppVolume; set => NowPlayingSong.AppVolume = value; }

        //value của thanh tiến trình bài hát
        public double CurrentSongProgress { get => NowPlayingSong.CurrentProgress; set => NowPlayingSong.CurrentProgress = value; }

        //Độ dài của bài hát, maxvalue của thanh tiến trình bài hát
        public double SongDuration { get => NowPlayingSong.SongDuration; set { } }

        public string SongLength { get => NowPlayingSong.SongLength; set { } }
        //define command
        public ICommand ReplayCommand { get; set; }
        public ICommand PlaySongCommand { get; set; }
        public ICommand NextSongCommand { get; set; }
        public ICommand PreviousSongCommand { get; set; }
        public ICommand AddToPlaylistCommand { get; set; }
        public ICommand FavoriteCommand { get; set; }
        public ICommand MuteCommand { get; set; }
        //

        // khi user thay đổi thay value (kéo, click) slider 
        public int CurrentProgressChange { get; set; }
        public ImageSource PlayButtonImage
        {
            get
            {
                if (NowPlayingSong.IsPlaying == true) return new BitmapImage(new Uri("/IndeeMusee;component/Assets/Group 82.png", UriKind.Relative));
                else return new BitmapImage(new Uri("/IndeeMusee;component/Assets/btnPlay.png", UriKind.Relative));
            }
            set => playButtonImage = value;
        }

        public BasicControlTaskBarViewModel()
        {
            AppVolume = 50;

            NowPlayingSong.ReplayStatus = 0;//lặp lại danh sách phát
            NowPlayingSong.ForcePlayNextSong = false;

            ReplayCommand = new RelayCommand<object>((p) =>
                                                {
                                                    if (NowPlayingSong.IsValidSongPath) return true; else return false;
                                                },
                                                (p) =>
                                                {
                                                    //if (NowPlayingSong.ReplaySong == true)
                                                    //    NowPlayingSong.ReplaySong = false;
                                                    //else NowPlayingSong.ReplaySong = true;
                                                    NowPlayingSong.ReplayStatus = (NowPlayingSong.ReplayStatus + 1) % 3;
                                                });

            PlaySongCommand = new RelayCommand<object>((p) => { if (NowPlayingSong.IsValidSongPath) return true; else return false; }, (p) => { PlayButton_Click(); });
            NextSongCommand = new RelayCommand<object>(
                (p) => { if (NowPlayingSong.IsValidSongPath) return true; else return false; },
                (p) =>
                {
                    NowPlayingSong.ForcePlayNextSong = true;
                    NowPlayingSong.PlayNextSong();
                    NowPlayingSong.ForcePlayNextSong = false;
                });
            PreviousSongCommand = new RelayCommand<object>(
                (p) =>
                {
                    if (NowPlayingSong.IsValidSongPath) return true; else return false;
                },
                (p) =>
                {
                    NowPlayingSong.PlayPreviousSong();
                });

            MuteCommand = new RelayCommand<object>((p) => { return true; }, (p) => { MessageBox.Show("Mute"); });
            AddToPlaylistCommand = new RelayCommand<object>((p) => { return true; }, (p) => { MessageBox.Show(" AddToPlaylist"); });
            FavoriteCommand = new RelayCommand<object>((p) => { return true; }, (p) => { MessageBox.Show("Favorite"); });

            NowPlayingSong.CurrentSongProgressChange += NowPlayingSong_CurrentSongProgressChange;
            NowPlayingSong.NewSongIsLoad += NowPlayingSong_NewSongIsLoad;
            NowPlayingSong.IsPlayingChange += NowPlayingSong_IsPlayingChange;

        }

        private void NowPlayingSong_IsPlayingChange()
        {
            OnPropertyChanged("PlayButtonImage");
        }

        private void NowPlayingSong_NewSongIsLoad()
        {
            OnPropertyChanged("SongDuration");
            OnPropertyChanged("SongLength");

        }

        // khi thời gian thay đổi, cập nhật slider
        private void NowPlayingSong_CurrentSongProgressChange()
        {
            OnPropertyChanged("CurrentSongProgress");
        }

        void PlayButton_Click()
        {
            NowPlayingSong.IsPlaying = !NowPlayingSong.IsPlaying;
        }

        //khi slider thay đổi - kiểm tra do user hoặc do chương trình và thay đổi time status, hàm được gọi khi có sự kiện valuechange của thanh tiến trình bài hát
        public void SongProgressChange(double newTime)
        {
            NowPlayingSong.SongProgressChange(newTime);
            OnPropertyChanged("TimeStatus");
        }

        public void VolumeChange(double newVolume)
        {
            AppVolume = newVolume;
        }

        //#region Bổ Sung vào codebehind của BasicControlTaskbar
        //bổ sung phần này vào code behind của BasicControlTaskbar
        //private void SongProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    ((BasicControlTaskBarViewModel)DataContext).SongProgressChange(e.NewValue);
        //}

        //private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    ((BasicControlTaskBarViewModel)DataContext).VolumeChange(e.NewValue / 100);
        //}
        //#endregion
    }
}
