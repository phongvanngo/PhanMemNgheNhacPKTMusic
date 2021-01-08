using IndeeMusee.Models;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for SidebarView.xaml
    /// </summary>
    public partial class BasicContrlTaskbar : UserControl
    {
        public string SongTime { get => ((BasicControlTaskBarViewModel)DataContext).SongLength; set { } }

        private const int V = 0;
        DoubleAnimation blinkAnimation = new DoubleAnimation();
        Storyboard myStoryboard = new Storyboard();

        public BasicContrlTaskbar()
        {
            InitializeComponent();
            DataContext = new BasicControlTaskBarViewModel();

            //SongModel testLyric = new SongModel();
            //testLyric.SongLocation = @"C:\Users\19520\Downloads\Lyrics Trên Tình Bạn Dưới Tình Yêu -MIN Official Lyrics Video.mp3";
            //testLyric.LyricPath = @"C:\Users\19520\Downloads\Tren tinh ban duoi tinh yeu lyric.lrc";
            //NowPlayingSong.SongIsPlaying = testLyric;

            //animation
            NowPlayingSong.IsPlayingChange += NowPlayingSong_IsPlayingChange;

            blinkAnimation.From = 0.5;
            blinkAnimation.To = 1;
            blinkAnimation.AutoReverse = true;
            blinkAnimation.Duration = TimeSpan.FromSeconds(2);
            blinkAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTargetName(blinkAnimation, "song_slider");
            Storyboard.SetTargetProperty(blinkAnimation, new PropertyPath(Slider.OpacityProperty));

            myStoryboard.Children.Add(blinkAnimation);
            myStoryboard.Begin(this);

            MainWindow.MoveForwardRequest += MainWindow_MoveForwardRequest;
            MainWindow.MoveBackwardRequest += MainWindow_MoveBackwardRequest;            
            
            LyricEditorVersion2.MoveForwardRequest += MainWindow_MoveForwardRequest;
            LyricEditorVersion2.MoveBackwardRequest += MainWindow_MoveBackwardRequest;


        }

        private void MainWindow_MoveBackwardRequest()
        {
            song_slider.Value = song_slider.Value - 3;
        }

        private void MainWindow_MoveForwardRequest()
        {
            song_slider.Value = song_slider.Value + 3;

        }

        private void NowPlayingSong_IsPlayingChange()
        {
            if (NowPlayingSong.IsPlaying == false)
            {
                myStoryboard.Begin(this);
            }
            else myStoryboard.Pause(this);
        }

        private void SongProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)

        {
            ((BasicControlTaskBarViewModel)DataContext).SongProgressChange(e.NewValue);
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((BasicControlTaskBarViewModel)DataContext).VolumeChange(e.NewValue / 100);
        }

        int LapLaiTatCa = 1;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            switch (LapLaiTatCa)
            {
                case 1:
                    LapLai_img.Source = new BitmapImage(new Uri("/IndeeMusee;component/Assets/Lap lại 1 bài.png", UriKind.Relative));
                    LapLaiTatCa = 2;
                    break;
                case 0:
                    LapLai_img.Source = new BitmapImage(new Uri("/IndeeMusee;component/Assets/Lặp lại.png", UriKind.Relative));
                    LapLaiTatCa = 1;
                    break;
                case 2:
                    LapLai_img.Source= new BitmapImage(new Uri("/IndeeMusee;component/Assets/Group 114.png", UriKind.Relative));
                    LapLaiTatCa = 0;
                    break;
            }                 
        }

        bool IsMute = false;
        double OldValue;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (IsMute == false)
            {
                IsMute = true;
                Volume_img.Source = new BitmapImage(new Uri("/IndeeMusee;component/Assets/sound-off.png", UriKind.Relative));
                Volume.IsEnabled = false;
                OldValue = Volume.Value;
                Volume.Value = 0;
            }
            else
            {
                IsMute = false;
                Volume_img.Source = new BitmapImage(new Uri("/IndeeMusee;component/Assets/LoaLon.png", UriKind.Relative));
                Volume.IsEnabled = true;
                Volume.Value = OldValue;
            }
        }

        private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                song_slider.Value = song_slider.Value - 20;
            }           
            
            if (e.Key == Key.Right)
            {
                song_slider.Value = song_slider.Value + 20;
            }
        }
    }
}
