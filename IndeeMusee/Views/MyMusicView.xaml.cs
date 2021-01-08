using IndeeMusee.Models;
using IndeeMusee.MyUtilites;
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
    /// Interaction logic for MyMusicView.xaml
    /// </summary>
    public partial class MyMusicView : UserControl
    {
        MyMusicViewModel myMusicVM = new MyMusicViewModel();

        public MyMusicView()
        {
            InitializeComponent();
            this.DataContext = myMusicVM;
            myMusicVM.AddNewSongEvent += MyMusicVM_AddNewSongEvent;
            myMusicVM.ButtonFavoriteSongClick += MyMusicVM_ButtonFavoriteSongClick;
        }

        private void MyMusicVM_ButtonFavoriteSongClick(int index, int status)
        {
            ListViewItem lvi = ListViewSong.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
            List<Control> list_button = GetSpecificControl.AllChildren(lvi);
            Button ButtonFavorite = list_button.Find(e => e.Name == "BtnFavorite") as Button;

            switch (status)
            {
                case 1:
                    //thích
                    ButtonFavorite.Style = (Style)FindResource("ButtonFavoriteSongFill");
                    (lvi.Content as SongModel).IsFavorite = 1;
                    break;
                case 0:
                    ButtonFavorite.Style = (Style)FindResource("ButtonFavoriteSong");
                    (lvi.Content as SongModel).IsFavorite = 0;
                    break;
                default:
                    break;
            }


        }

        private void MyMusicVM_AddNewSongEvent()
        {
            ListViewSong.SelectedIndex = myMusicVM.CurrentSongList.Count - 1;
        }

        private void ListViewSong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewSong.ScrollIntoView(ListViewSong.SelectedItem);
            IsMinitoolShow = false;
            Minitool.Visibility = Visibility.Hidden;
            TriagleDown.Visibility = Visibility.Hidden;
            TriagleUp.Visibility = Visibility.Hidden;
        }
        private void PlayNowButton_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            index = ListViewSong.SelectedIndex;
            myMusicVM.PlayNowRequest(index);
        }
        private void AddToQueueButton_Click(object sender, RoutedEventArgs e)
        {
            //int index = 0;
            //index = ListViewSong.SelectedIndex;
            //myMusicVM.AddToPlayingQueueRequest(k);
        }

        private void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            item.IsSelected = true;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {

        }
        bool IsMinitoolShow = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show((ListViewSong.SelectedItem as SongModel).Title); 
            if (IsMinitoolShow == false)
            {
                Point point = Mouse.GetPosition(Application.Current.MainWindow);
                int temp = Convert.ToInt32((point.Y - 155) / 35)+1;
                if (point.Y - (temp * 35 + 155) < 0)
                {
                    temp -= 1;
                }
                int Y = (temp + 1) * 35-5;
                if (Y > ListViewSong.ActualHeight / 2)
                {
                    Y = (temp-1) * 35 - 140+5;
                    var moveTriagle = new TranslateTransform();
                    moveTriagle.X = 0;
                    moveTriagle.Y = Y + 140;
                    TriagleDown.RenderTransform = moveTriagle;
                    TriagleDown.Visibility = Visibility.Visible;
                }
                else
                {
                    var moveTriagle = new TranslateTransform();
                    moveTriagle.X = 0;
                    moveTriagle.Y = Y - 10;
                    TriagleUp.RenderTransform = moveTriagle;
                    TriagleUp.Visibility = Visibility.Visible;
                }
                var moveMinitool = new TranslateTransform();
                Minitool.RenderTransform = moveMinitool;
                moveMinitool.X = 0;
                moveMinitool.Y = Y;
                IsMinitoolShow = true;
                Minitool.Visibility = Visibility.Visible;

            }
            else
            {
                Minitool.Visibility = Visibility.Hidden;
                TriagleDown.Visibility = Visibility.Hidden;
                TriagleUp.Visibility = Visibility.Hidden;
                IsMinitoolShow = false;
            }
        }

        ThicknessAnimationUsingKeyFrames myAnimation;
        Storyboard storyboard;
        TextBlock TblockSongSelected;
        Thickness SourceTbTitleMargin = new Thickness(20, 6, 0, 6);
        string SourceTbTitleText;
        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (TblockSongSelected != null)
            {
                storyboard.Pause(this);
                storyboard.Stop(this);
                TblockSongSelected.Text = SourceTbTitleText;
                TblockSongSelected.Margin = SourceTbTitleMargin;
            }

            var item = e.OriginalSource as ListBoxItem;

            //set button trai tim
            SongModel songSelected = item.Content as SongModel;
            if (songSelected.IsFavorite == 1)
            {
                List<Control> list_button = GetSpecificControl.AllChildren(item);
                Button ButtonFavorite = list_button.Find(btn => btn.Name == "BtnFavorite") as Button;
                Style newStyle = (Style)FindResource("ButtonFavoriteSongFill");
                ButtonFavorite.Style = newStyle;
            }


            ListViewSong.SelectedItem = item;
            TblockSongSelected = GetSpecificControl.GetChildOfType<TextBlock>(item);
            SourceTbTitleText = TblockSongSelected.Text;


            Thickness originalMargin = TblockSongSelected.Margin;
            Thickness secondMargin = originalMargin;
            TblockSongSelected.Text += "         ";
            secondMargin.Left = -MyFunction.MeasureString(TblockSongSelected.Text, TblockSongSelected) + 20;
            TblockSongSelected.Text += TblockSongSelected.Text += TblockSongSelected.Text;



            myAnimation = new ThicknessAnimationUsingKeyFrames();
            Storyboard.SetTargetName(myAnimation, "test");
            Storyboard.SetTargetProperty(myAnimation, new PropertyPath(Rectangle.MarginProperty));

            myAnimation.KeyFrames.Add(new SplineThicknessKeyFrame(originalMargin, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            myAnimation.KeyFrames.Add(new SplineThicknessKeyFrame(secondMargin, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(5))));
            myAnimation.KeyFrames.Add(new SplineThicknessKeyFrame(secondMargin, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(6))));

            myAnimation.RepeatBehavior = RepeatBehavior.Forever;
            TblockSongSelected.Name = "TB" + MyFunction.GenerateCode();
            this.RegisterName(TblockSongSelected.Name, TblockSongSelected);

            Storyboard.SetTargetName(myAnimation, TblockSongSelected.Name);
            Storyboard.SetTargetProperty(myAnimation, new PropertyPath(TextBlock.MarginProperty));

            storyboard = new Storyboard();
            storyboard.Children.Add(myAnimation);
            storyboard.Begin(this, true);
            
        }

        private void ListBoxItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (TblockSongSelected != null)
            {
                storyboard.Pause(this);
                storyboard.Stop(this);
                TblockSongSelected.Text = SourceTbTitleText;
                TblockSongSelected.Margin = SourceTbTitleMargin;
            }
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("hi");
        }

        private void ListViewSong_Loaded(object sender, RoutedEventArgs e)
        {
            //UpdateHeartForFavoriteSong();
        }
    }
}


