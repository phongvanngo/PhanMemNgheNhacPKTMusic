using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using IndeeMusee.ViewModels;
using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace IndeeMusee
{
    /// <summary>
    /// Interaction logic for EditForm.xaml
    /// </summary>
    public partial class EditLyric : Window
    {
        SongModel SongToEdit;

        public static event ToggleFormDialogNotifyHandler ToggleForm;

        public static event ChangePageHandler ChangePage;
        public EditLyric(SongModel songToEdit)
        {
            InitializeComponent();
            ToggleForm();
            this.SongToEdit = songToEdit;
            TbSongLyric.Text = songToEdit.LyricPath;
            this.Closing += EditLyric_Closing;
        }

        private void EditLyric_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleForm();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(TbSongLyric.Text) == true)
            {
                try
                {
                    //di chuyen file loi bai hat vao database
                    string sourceFile = TbSongLyric.Text;
                    string destinationFile =$@"{GeneralDataManagement.LyricFolderPath}\{System.IO.Path.GetFileName(sourceFile)}";
                    File.Copy(sourceFile, destinationFile, true);
                    SongToEdit.LyricPath = System.IO.Path.GetFileName(sourceFile);
                    SongDataAccess.UpdateSong(SongToEdit);
                    MyMusicControl.UserSongListHasUpdated();

                }
                catch (Exception)
                {
                    MessageBox.Show("Them bai hat that bai");
                }

            }
            else
            {
                SongToEdit.LyricPath = "";
                SongDataAccess.UpdateSong(SongToEdit);
            }

            this.Close();

        }

        private void AddFileLyric_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            //openFileDialog.Filter= "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                TbSongLyric.Text = openFileDialog.FileName;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TbSongLyric.Text = "";
        }

        private void BtnLyricEditor_Click(object sender, RoutedEventArgs e)
        {
            LyricEditorVersion2ViewModel.SongToEdit = SongToEdit;
            this.Close();
            ChangePage("LyricEditor");
        }
    }
}
