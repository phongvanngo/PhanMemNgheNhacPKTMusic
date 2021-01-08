using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using IndeeMusee.ViewModels;
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
using System.Windows.Threading;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for PlayListInfoViews.xaml
    /// </summary>
    public partial class PlayListInfoViews : UserControl
    {
        DispatcherTimer timer = new DispatcherTimer();
        public PlayListInfoViews()
        {
            InitializeComponent();

            //timer.Tick += Timer_Tick;
            //timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((this.DataContext as PlaylistInfoViewModel) == null) return;
            timer.Stop();
            string ImagePath = (this.DataContext as PlaylistInfoViewModel).PlaylistDetail.ImagePath;
            if (File.Exists($@"{GeneralDataManagement.ImageFolderPath}\{ImagePath}") == true)
            {
                PlaylistImage.Stretch = Stretch.Fill;
                PlaylistImage.Source = new BitmapImage(new Uri($@"{GeneralDataManagement.ImageFolderPath}\{ImagePath}"));
            }
        }

        private void PlaylistImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();

            // image filters  
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // display image in picture box  


                string sourceFile = open.FileName;
                string destFile = $@"{GeneralDataManagement.ImageFolderPath}\{System.IO.Path.GetFileName(sourceFile)}";
                try
                {
                    PlaylistImage.Source = new BitmapImage(new Uri(open.FileName));
                    PlaylistModel playlistToEdit = (this.DataContext as PlaylistInfoViewModel).PlaylistDetail;

                    if (File.Exists(destFile) == false)
                    {
                        System.IO.File.Copy(sourceFile, destFile, true);
                    }
                    playlistToEdit.ImagePath = destFile.Replace($@"{GeneralDataManagement.ImageFolderPath}\", "");
                    SongDataAccess.UpdatePlaylist(playlistToEdit);
                    playlistToEdit.ImagePath = System.IO.Path.GetFileName(open.FileName);

                }
                catch (Exception)
                {
                    MessageBox.Show("Chỉnh sửa thất bại");
                }



            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as PlaylistInfoViewModel).ChangePlaylistName();
        }
    }
}
