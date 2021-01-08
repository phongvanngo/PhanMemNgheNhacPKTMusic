using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView : UserControl
    {
        ButtonAddPlaylist BtnAddPlaylist = new ButtonAddPlaylist();

        List<PlaylistModel> ListPlaylists = new List<PlaylistModel>();
        public PlaylistView()
        {
            InitializeComponent();
            BtnAddPlaylist.ButtonClick += AddPlaylistButton_ButtonClick;
            WrapPanelListOfPlayList.Children.Add(BtnAddPlaylist);

            ListPlaylists = SongDataAccess.GetAllPlaylist();

            UpdatePlaylistCard();
        }

        public void UpdatePlaylistCard()
        {

            WrapPanelListOfPlayList.Children.Clear();
            for (int i = 0; i < ListPlaylists.Count; i++)
            {
                PlaylistCard PLCard = new PlaylistCard(ListPlaylists[i]);
                WrapPanelListOfPlayList.Children.Add(PLCard);
            }

            WrapPanelListOfPlayList.Children.Add(BtnAddPlaylist);
        }

        private void AddPlaylistButton_ButtonClick()
        {
            CreatePlaylistForm DialogAddNewPlaylist = new CreatePlaylistForm();
            DialogAddNewPlaylist.ShowDialog();
            switch (DialogAddNewPlaylist.DialogResult)
            {
                case true:
                    PlaylistModel newPlaylist = new PlaylistModel();
                    newPlaylist.PlaylistKey = "PL" + MyUtilites.MyFunction.GenerateCode();
                    newPlaylist.PlaylistName = DialogAddNewPlaylist.PlaylistName;

                    //di chuyển hình ảnh playlist của người dùng
                    string ImageFilePath = DialogAddNewPlaylist.ImagePath;
                    if (File.Exists(ImageFilePath))
                    {
                        string sourceFile = ImageFilePath;
                        string destFile = $@"{GeneralDataManagement.ImageFolderPath}\{System.IO.Path.GetFileName(sourceFile)}";
                        try
                        {
                            if (File.Exists(destFile) == false)
                            {
                                System.IO.File.Copy(sourceFile, destFile, true);
                            }
                            newPlaylist.ImagePath = destFile.Replace($@"{GeneralDataManagement.ImageFolderPath}\", "");

                        }
                        catch (Exception)
                        {
                        }
                    }

                    SongDataAccess.InsertNewPlaylistToDatabase(newPlaylist);
                    ListPlaylists.Add(newPlaylist);
                    UpdatePlaylistCard();
                    break;

                case false:
                    break;

                default:
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ListOfPlayList.Children.Add(new UserControl1());
        }
    }
}
