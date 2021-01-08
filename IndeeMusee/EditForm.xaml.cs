using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
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
using System.Windows.Shapes;

namespace IndeeMusee
{
    /// <summary>
    /// Interaction logic for EditForm.xaml
    /// </summary>
    public partial class EditForm : Window
    {

        SongModel songToEdit;

        public static event ToggleFormDialogNotifyHandler ToggleForm;
        public EditForm(SongModel songToEdit)
        {
            InitializeComponent();
            ToggleForm();
            this.songToEdit = songToEdit;
            TbSongTitle.Text = songToEdit.Title;
            TbSongGenre.Text = songToEdit.Genre;
            TbSongArtist.Text = songToEdit.ContributingArtists;

            this.Closing += EditForm_Closing;
        }

        private void EditForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleForm();
        }

        public SongModel SongToEdit { get => songToEdit; set => songToEdit = value; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (TbSongArtist.Text == "" || TbSongGenre.Text == "" || TbSongTitle.Text == "")
            {
                if (TbSongArtist.Text.Trim() == "")
                    borderArtist.BorderThickness = new Thickness(2);
                if (TbSongGenre.Text.Trim() == "")
                    borderGenre.BorderThickness = new Thickness(2);
                if (TbSongTitle.Text.Trim() == "")
                    borderSongTitle.BorderThickness = new Thickness(2);
                return;
            }    
            string[] dataChange = { "" };
            songToEdit.Title = TbSongTitle.Text;
            songToEdit.Genre = TbSongGenre.Text;
            songToEdit.ContributingArtists = TbSongArtist.Text;

            var tfile = TagLib.File.Create($@"{GeneralDataManagement.UserMusicFolderLocation}\{songToEdit.SongLocation}");
            tfile.Tag.Title = songToEdit.Title;

            dataChange[0] = songToEdit.Title;
            tfile.Tag.Genres = dataChange;

            dataChange[0] = songToEdit.ContributingArtists;
            tfile.Tag.Artists = dataChange;

            //tfile.Tag.Performers[0] = songToEdit.ContributingArtists;

            try
            {
                tfile.Save();
            }
            catch (Exception)
            {

            }

            SongDataAccess.UpdateSong(SongToEdit);
            MyMusicControl.UserSongListHasUpdated();
            MusicInPlaylistControl.UserSongListHasUpdated();

            this.Close();

        }


        private void btnSongTitle_Click(object sender, RoutedEventArgs e)
        {
            TbSongTitle.Text = "";
        }

        private void borderSongTitle_MouseEnter(object sender, MouseEventArgs e)
        {
            btnSongTitle.Visibility = Visibility.Visible;
        }

        private void borderSongTitle_MouseLeave(object sender, MouseEventArgs e)
        {
            btnSongTitle.Visibility = Visibility.Hidden;
        }

        private void borderArtist_MouseEnter(object sender, MouseEventArgs e)
        {
            btnArtist.Visibility = Visibility.Visible;
        }

        private void borderArtist_MouseLeave(object sender, MouseEventArgs e)
        {
            btnArtist.Visibility = Visibility.Hidden;
        }

        private void btnArtist_Click(object sender, RoutedEventArgs e)
        {
            TbSongArtist.Text = "";
        }

        private void borderGenre_MouseEnter(object sender, MouseEventArgs e)
        {
            btnGenre.Visibility = Visibility.Visible;
        }

        private void borderGenre_MouseLeave(object sender, MouseEventArgs e)
        {
            btnGenre.Visibility = Visibility.Hidden;
        }

        private void btnGenre_Click(object sender, RoutedEventArgs e)
        {
            TbSongGenre.Text = "";
        }
    }
}
