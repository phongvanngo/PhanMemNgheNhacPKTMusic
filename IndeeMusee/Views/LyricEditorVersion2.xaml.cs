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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for LyricEditorVersion2.xaml
    /// </summary>
    public partial class LyricEditorVersion2 : UserControl
    {
        public static event ChangePageHandler ChangePage;


        public static event NotifyHandler MoveForwardRequest;
        public static event NotifyHandler MoveBackwardRequest;

        public LyricEditorVersion2()
        {
            InitializeComponent();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as LyricEditorVersion2ViewModel).SaveStatus=="SAVE*")
            {
                MessageBoxResult messageBoxResult = MessageBox.Show($@"Nội dung chưa được lưu, bạn có chắc chắn muốn thoát ?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return;
                }
            }
            UpNextListModel.IsEditingLyric = false;
            MyMusicControl.UserSongListHasUpdated();
            MusicInPlaylistControl.UserSongListHasUpdated();
            ChangePage("MyMusic");

        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && ModifierKeys.Control == Keyboard.Modifiers)
            {
                NowPlayingSong.IsPlaying = !NowPlayingSong.IsPlaying;
                e.Handled = true;
            }
        }
        int indexSelected = 0;
        private void ListView_ListPlaylist_PreviewKeyDown(object sender, KeyEventArgs e)
        {
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
            if (e.Key == Key.Down)
            {
                //ListView_ListPlaylist.SelectedIndex = ListView_ListPlaylist.SelectedIndex + 1;

            }
        }

        private void ListView_ListPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
