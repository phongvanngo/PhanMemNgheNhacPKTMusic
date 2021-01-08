using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IndeeMusee
{
    /// <summary>
    /// Interaction logic for CreatePlaylistForm.xaml
    /// </summary>
    public partial class CreatePlaylistForm : Window
    {
        string playlistName = "";
        string imagePath = "";

        public static event ToggleFormDialogNotifyHandler ToggleForm;

        public CreatePlaylistForm()
        {
            InitializeComponent();
            ToggleForm();
            this.Closing += CreatePlaylistForm_Closing;
        }

        private void CreatePlaylistForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleForm();
        }

        public string PlaylistName { get => playlistName; set => playlistName = value; }
        public string ImagePath { get => imagePath; set => imagePath = value; }

        private void PlayListName_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TbPlayListName.Text != null)
                LblNon.Visibility = Visibility.Visible;
            else
                LblNon.Visibility = Visibility.Hidden;
        }

        private void BtnCreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (TbPlayListName.Text.Trim() == "")
            {
                borderListName.BorderThickness = new Thickness(2);
                return;
            }
            
            playlistName = TbPlayListName.Text;

            

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void PlaylistImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            
            // image filters  
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // display image in picture box  
                PlaylistImage.Source = new BitmapImage(new Uri(open.FileName));
                ImagePath = open.FileName;
                PlaylistImage.Stretch = Stretch.UniformToFill;
                PlaylistImage.Width = Double.NaN;
                // image file path  
            }
        }
    }
}
