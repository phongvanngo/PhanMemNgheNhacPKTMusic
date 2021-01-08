using IndeeMusee.Models;
using IndeeMusee.MyUtilites;
using IndeeMusee.ViewModels;
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

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for NowPlayingView.xaml
    /// </summary>
    /// 


    public partial class NowPlayingView : UserControl
    {
        NowPlayingViewModel NowPlayingVM = new NowPlayingViewModel();
        int CurrentIndexSelected = -1;

        public static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        public static System.Windows.Forms.Timer myTimerAnimation = new System.Windows.Forms.Timer();
        
        public NowPlayingView()
        {
            InitializeComponent();
            this.DataContext = NowPlayingVM;
            myTimer.Interval = 100;
            myTimer.Tick += MyTimer_Tick;
            UpNextListModel.ChangeSongToPlayEvent += UpNextListModel_ChangeSongToPlayEvent;
            isSuffer = true;


        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                TextBlock PreviousTexbBlock = TblockSongSelected;

                int index = ListOfAudio.SelectedIndex;
                ListViewItem lvi = ListOfAudio.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;

                if (lvi == null) return;
                TextBlock newTb = GetSpecificControl.GetChildOfType<TextBlock>(lvi);

                if (newTb != TblockSongSelected)
                {
                    TblockSongSelected = newTb;
                    SetAnimation();
                }
            }
            catch (Exception)
            {

            }
        }

        private void UpNextListModel_ChangeSongToPlayEvent(int index)
        {
            ListOfAudio.SelectedIndex = index;
        }


        bool IsFullSize = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsFullSize == false)
            {
                IsFullSize = true;
                GridNowPlaying.VerticalAlignment = VerticalAlignment.Stretch;

                //animation
                DoubleAnimation animation = new DoubleAnimation();
                Storyboard.SetTargetName(animation, "GridNowPlaying");
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath(Grid.HeightProperty));
                animation.From = 50;
                animation.To = 550;
                animation.Duration = TimeSpan.FromSeconds(0.2);
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(animation);
                storyboard.Begin(this);

            }
            else
            {
                GridNowPlaying.VerticalAlignment = VerticalAlignment.Bottom;
                IsFullSize = false;

                DoubleAnimationUsingKeyFrames myDoubleAnimation = new DoubleAnimationUsingKeyFrames();
                Storyboard.SetTargetName(myDoubleAnimation, "GridNowPlaying");
                Storyboard.SetTargetProperty(myDoubleAnimation,
                    new PropertyPath(Grid.HeightProperty));
                myDoubleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(700, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.05))));
                myDoubleAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(50, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2))));
                Storyboard myStoryboard = new Storyboard();
                myStoryboard.Children.Add(myDoubleAnimation);
                myStoryboard.Begin(this);
            }
        }

        ThicknessAnimationUsingKeyFrames myAnimation;
        Storyboard storyboard;
        TextBlock TblockSongSelected;
        Thickness SourceTbTitleMargin = new Thickness(20, 0, 0, 0);
        string SourceTbTitleText;
        private void ListOfAudio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //myTimer.Stop();

            //nếu nhấp chọn bài hát đang phát, thì không làm gì hết
            if (CurrentIndexSelected == ListOfAudio.SelectedIndex) return;

            //nếu chọn bài khác trong lúc chỉnh sửa lời một bài hát, ko cho chọn
            if (UpNextListModel.IsEditingLyric == true)
            {
                ListOfAudio.SelectedIndex = CurrentIndexSelected;
                return;
            }

            //hợp lệ 
            myTimer.Stop();
            ListOfAudio.ScrollIntoView(ListOfAudio.SelectedItem);
            myTimerAnimation.Tick += MyTimerAnimation_Tick;
            myTimerAnimation.Start();

            NowPlayingVM.ChoseSongToPlayInQueue(ListOfAudio.SelectedIndex);

            CurrentIndexSelected = ListOfAudio.SelectedIndex;

        }
        private void MyTimerAnimation_Tick(object sender, EventArgs e)
        {
            myTimerAnimation.Stop();
            if (TblockSongSelected != null)
            {
                storyboard.Pause(this);
                storyboard.Stop(this);
                TblockSongSelected.Text = SourceTbTitleText;
                TblockSongSelected.Margin = SourceTbTitleMargin;
            }
            TblockSongSelected = null;
            while (TblockSongSelected == null)
            {
                try
                {
                    int index = ListOfAudio.SelectedIndex;
                    ListViewItem lvi = ListOfAudio.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
                    if (lvi == null) return;
                    TblockSongSelected = GetSpecificControl.GetChildOfType<TextBlock>(lvi);
                }
                catch (Exception ex)
                {
                    return;
                }
            }

            SourceTbTitleText = TblockSongSelected.Text;

            SetAnimation();
            myTimer.Start();
        }

        void SetAnimation()
        {
            string Artist = (ListOfAudio.SelectedItem as SongModel).ContributingArtists;
            string Genre = (ListOfAudio.SelectedItem as SongModel).Genre;
            string Length = (ListOfAudio.SelectedItem as SongModel).Length;
            string TextToRun = $@"{TblockSongSelected.Text} - {Artist.Trim()} - {Genre.Trim()} - {Length.Trim()}";

            TblockSongSelected.Text = TextToRun;

            Thickness originalMargin = TblockSongSelected.Margin;
            Thickness secondMargin = originalMargin;

            TblockSongSelected.Text += "         ";
            secondMargin.Left = -MyFunction.MeasureString(TblockSongSelected.Text, TblockSongSelected) + SourceTbTitleMargin.Left;
            TblockSongSelected.Text += TblockSongSelected.Text;

            myAnimation = new ThicknessAnimationUsingKeyFrames();
            Storyboard.SetTargetName(myAnimation, "test");
            Storyboard.SetTargetProperty(myAnimation, new PropertyPath(Rectangle.MarginProperty));

            myAnimation.KeyFrames.Add(new SplineThicknessKeyFrame(originalMargin, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            myAnimation.KeyFrames.Add(new SplineThicknessKeyFrame(secondMargin, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(8))));
            myAnimation.KeyFrames.Add(new SplineThicknessKeyFrame(secondMargin, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(9))));

            myAnimation.RepeatBehavior = RepeatBehavior.Forever;
            TblockSongSelected.Name = "TB" + MyFunction.GenerateCode();
            this.RegisterName(TblockSongSelected.Name, TblockSongSelected);

            Storyboard.SetTargetName(myAnimation, TblockSongSelected.Name);
            Storyboard.SetTargetProperty(myAnimation, new PropertyPath(TextBlock.MarginProperty));

            storyboard = new Storyboard();
            storyboard.Children.Add(myAnimation);

            storyboard.Begin(this, true);
        }
        private bool isSuffer;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(isSuffer)
            {
                img_suffer.Source = new BitmapImage(new Uri("/IndeeMusee;component/Assets/NgauNhien.png", UriKind.Relative));
                isSuffer = false;
            }
            else
            {
                img_suffer.Source = new BitmapImage(new Uri("/IndeeMusee;component/Assets/Group 117.png", UriKind.Relative));
                isSuffer = true;
            }
        }
    }
}
