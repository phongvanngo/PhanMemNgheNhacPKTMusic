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

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for EditPlayListNameForm.xaml
    /// </summary>
    public partial class EditPlayListNameForm : Window
    {
        public static event ToggleFormDialogNotifyHandler ToggleForm;

        public string NewPlaylistName { get; set; }
        public EditPlayListNameForm(string playlistName)
        {
            InitializeComponent();
            ToggleForm();
            this.Closing += EditPlayListNameForm_Closing;
            this.TbSongTitle.Text = playlistName;
        }

        private void EditPlayListNameForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleForm();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TbSongTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TbSongTitle.Text = "";
            btnRemove.Visibility = Visibility.Visible;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            TbSongTitle.Text = "";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(TbSongTitle.Text.Trim()=="")
            {
                BorderSongTitle.BorderThickness = new Thickness(2.5);
                return;
            }
            NewPlaylistName = TbSongTitle.Text;
            this.DialogResult = true;
        }
    }
}
