using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using IndeeMusee.ViewModels;
using IndeeMusee.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IndeeMusee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class ThreadIntroClass
    {
        public static bool done;
        public static void DoIntro()
        {
            done = false;
            FormIntro formIntro = new FormIntro();
            formIntro.ShowDialog();
            if (formIntro.DialogResult == true)
            {
                done = true;
            };
        }
    }
    public partial class MainWindow : Window
    {
        HomeViewModel HomeVM;
        MyMusicViewModel MyMusicVM;
        PlaylistViewModel PlaylistVM;
        PlaylistInfoViewModel PlaylistInfoVM;
        PlaylistInfoViewModel pLFavoriteVM;
        ExploreMusicViewModel ExploreMusicVM;
        KaraokeViewModel KaraokeVM;

        DoubleAnimation transparentAnimation = new DoubleAnimation();
        Storyboard myStoryboard = new Storyboard();

        public static event NotifyHandler MoveForwardRequest;
        public static event NotifyHandler MoveBackwardRequest;

        internal PlaylistInfoViewModel PLFavoriteVM { get => pLFavoriteVM; set => pLFavoriteVM = value; }

        public static event NotifyHandler SearchRequest;
        public static event NotifyHandler DeleteSearchContent;

        DispatcherTimer timerWaitRecentSong = new DispatcherTimer();
        DispatcherTimer timerWaitIntroPage = new DispatcherTimer();

        
        public MainWindow()
        {

            //Thread threadIntro = new Thread(ThreadIntroClass.DoIntro);
            //threadIntro.SetApartmentState(ApartmentState.STA);
            //threadIntro.Start();
            //timerWaitIntroPage.Tick += TimerWaitIntroPage_Tick;
            //timerWaitIntroPage.Start();

            FormIntro formIntro = new FormIntro();
            formIntro.ShowDialog();

            #region when startup app

            IndeeMusee.DataManager.GeneralDataManagement.OnStartup();
            IndeeMusee.DataManager.GeneralDataManagement.CheckUserMusicFolderLocation();

            if (IndeeMusee.DataManager.GeneralDataManagement.IsHavingDatabaseInUserMusicFolder() == false)
            {
                IndeeMusee.DataManager.GeneralDataManagement.InitializeDatabase();
                IndeeMusee.DataManager.GeneralDataManagement.FindAndImportSongToDatabase();
            }

            IndeeMusee.DataManager.GeneralDataManagement.RemoveInvalidSongFromDatabase();


            #endregion

            //SongDataAccess.AmountOfSongInPlaylist("FAVORITE_PLAYLIST");

            InitializeComponent();

            this.Opacity = 1;

            HomeVM = new HomeViewModel();
            MyMusicVM = new MyMusicViewModel();
            PlaylistVM = new PlaylistViewModel();
            ExploreMusicVM = new ExploreMusicViewModel();
            KaraokeVM = new KaraokeViewModel();

            SidebarViewModel.ChangePage += SidebarViewModel_ChangePage;
            PlaylistBarViewModel.ChangePage += SidebarViewModel_ChangePage;
            HeaderBar.ChangePage += SidebarViewModel_ChangePage;
            PlaylistCard.ClickPlaylistCardEvent += PlaylistCard_ClickPlaylistCardEvent;
            EditLyric.ChangePage += SidebarViewModel_ChangePage;
            LyricEditorVersion2.ChangePage += SidebarViewModel_ChangePage;
            UtilitiesControl.ChangePage += SidebarViewModel_ChangePage;

            EditLyric.ToggleForm += NewFormIsLoad;
            EditForm.ToggleForm += NewFormIsLoad;
            EditPlayListNameForm.ToggleForm += NewFormIsLoad;
            ListMusicDialog.ToggleForm += NewFormIsLoad;
            AddPlayListForm.ToggleForm += NewFormIsLoad;
            CreatePlaylistForm.ToggleForm += NewFormIsLoad;
            ChooseMusicFolderForm.ToggleForm += NewFormIsLoad;
            LyricEditor_FormAddLyric.ToggleForm += NewFormIsLoad;
            SleepTimerForm.ToggleForm += NewFormIsLoad;
            GoodNightForm.ToggleForm += NewFormIsLoad;

            PLFavoriteVM = new PlaylistInfoViewModel("FAVORITE_PLAYLIST");

            //chạy bài hát gần đây nhất
            string recentSongKey = GeneralDataManagement.GetRecentSong();
            SongModel recentSong = SongDataAccess.GetSongFromDatabase(recentSongKey);
            if (recentSong != null)
            {
                UpNextListModel.AddSongToUpNextListAndPlay(recentSong);

                timerWaitRecentSong.Tick += TimerWaitRecentSong_Tick;
                timerWaitRecentSong.Start();
            }

        }

        private void TimerWaitIntroPage_Tick(object sender, EventArgs e)
        {
            //if (ThreadIntroClass.done == true)
            //{
            //    timerWaitIntroPage.Stop();
            //    this.WindowState = WindowState.Normal;
            //}
        }

        private void TimerWaitRecentSong_Tick(object sender, EventArgs e)
        {
            if (NowPlayingSong.LoadSongStatus == true && NowPlayingSong.SongIsPlaying!=null)
            {
                timerWaitRecentSong.Stop();
                NowPlayingSong.IsPlaying = false;
            }
        }

        private void NewFormIsLoad()
        {
            if (this.Opacity == 1)
            {
                transparentAnimation.From = 1;
                transparentAnimation.To = 0.3;
                transparentAnimation.Duration = TimeSpan.FromSeconds(0.4);
                Storyboard.SetTargetName(transparentAnimation, "MussicIndee");
                Storyboard.SetTargetProperty(transparentAnimation, new PropertyPath(Slider.OpacityProperty));
                myStoryboard.Children.Add(transparentAnimation);
                myStoryboard.Begin(this);
            }
            else
            {
                transparentAnimation.From = 0.3;
                transparentAnimation.To = 1;
                transparentAnimation.Duration = TimeSpan.FromSeconds(0.4);
                Storyboard.SetTargetName(transparentAnimation, "MussicIndee");
                Storyboard.SetTargetProperty(transparentAnimation, new PropertyPath(Slider.OpacityProperty));
                myStoryboard.Children.Add(transparentAnimation);
                myStoryboard.Begin(this);
            }

        }

        private void PlaylistCard_ClickPlaylistCardEvent(string playlistKey)
        {
            PlaylistInfoViewModel PlaylistInfoVM = new PlaylistInfoViewModel(playlistKey);
            PlaylistInfoVM.GoBack += SidebarViewModel_ChangePage;
            this.DataContext = PlaylistInfoVM;
        }

        private void SidebarViewModel_ChangePage(string page)
        {
            //nếu đang sửa lời bài hát không cho chuyển trang
            if (UpNextListModel.IsEditingLyric == true) return;

            lyric_view.Visibility = Visibility.Hidden;
            favorite_view.Visibility = Visibility.Hidden;

            if (HeaderBar.ExploreOnlineMode == true) DeleteSearchContent();
            HeaderBar.IsInMyMusicView = false;
            HeaderBar.ExploreOnlineMode = false;

            switch (page)
            {
                case "Home":
                    lyric_view.Visibility = Visibility.Visible;
                    ContentControl_CurrentView.Visibility = Visibility.Hidden;
                    DeleteSearchContent();

                    break;
                case "MyMusic":
                    SearchRequest();
                    this.DataContext = MyMusicVM;
                    ContentControl_CurrentView.Visibility = Visibility.Visible;
                    break;
                case "FavoriteSong":
                    //PlaylistInfoVM = new PlaylistInfoViewModel("FAVORITE_PLAYLIST");
                    //PlaylistInfoVM.GoBack += SidebarViewModel_ChangePage;
                    //this.DataContext = PlaylistInfoVM;
                    //this.DataContext = MyMusicVM;
                    DeleteSearchContent();
                    //PlaylistCard_ClickPlaylistCardEvent("FAVORITE_PLAYLIST");
                    PlaylistInfoViewModel PLFavoriteVM = new PlaylistInfoViewModel("FAVORITE_PLAYLIST");
                    favorite_view.DataContext = PLFavoriteVM;
                    favorite_view.Visibility = Visibility.Visible;
                    ContentControl_CurrentView.Visibility = Visibility.Hidden;
                    MusicInPlaylistControl.UserSongListHasUpdated();
                    break;
                case "Playlist":
                    DeleteSearchContent();
                    PlaylistVM = new PlaylistViewModel();
                    this.DataContext = PlaylistVM;
                    ContentControl_CurrentView.Visibility = Visibility.Visible;
                    break;
                case "LyricEditor":
                    DeleteSearchContent();
                    this.DataContext = new LyricEditorVersion2ViewModel();
                    ContentControl_CurrentView.Visibility = Visibility.Visible;
                    break;

                case "Karaoke":
                    DeleteSearchContent();
                    this.DataContext = new KaraokeViewModel();
                    ContentControl_CurrentView.Visibility = Visibility.Visible;
                    break;

                case "Explore":
                    DeleteSearchContent();
                    HeaderBar.ExploreOnlineMode = true;
                    SearchRequest();
                    this.DataContext = ExploreMusicVM;
                    ContentControl_CurrentView.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void NowPlayingView_Loaded()
        {
            //IndeeMusee.EditForm editForm = new IndeeMusee.EditForm();
        }

        private void MussicIndee_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                //e.Handled để kêt thúc sự kiện
                SidebarViewModel_ChangePage("MyMusic");
                SearchRequest();
                e.Handled = true;
            }

            if (e.Key == Key.D && Keyboard.Modifiers == ModifierKeys.Control)
            {
                UpNextListModel.RemoveAllQueue();
                e.Handled = true;
            }

            if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SidebarViewModel_ChangePage("Playlist");
                e.Handled = true;
            }

            if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SidebarViewModel_ChangePage("FavoriteSong");
                e.Handled = true;
            }

            if (e.Key == Key.H && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SidebarViewModel_ChangePage("Home");
                e.Handled = true;
            }

            if (e.Key == Key.Q && Keyboard.Modifiers == ModifierKeys.Control)
            {
                NowPlayingSong.IsPlaying = !NowPlayingSong.IsPlaying;
            }

            if (e.Key == Key.Left)
            {
                MoveBackwardRequest();
                e.Handled = true;
            }

            if (e.Key == Key.Right)
            {
                MoveForwardRequest();
                e.Handled = true;
            }

        }
        bool isUtilitiesToolShow = true;
        private void Ellipse_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isUtilitiesToolShow)
            {
                utilitiescontrol.Visibility = Visibility.Visible;
                triangle.Visibility = Visibility.Visible;
                isUtilitiesToolShow = false;
            }
            else
            {
                utilitiescontrol.Visibility = Visibility.Hidden;
                triangle.Visibility = Visibility.Hidden;
                isUtilitiesToolShow = true;
            }
        }
    }
}
