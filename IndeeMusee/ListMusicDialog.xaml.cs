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
using System.Windows.Shapes;

namespace IndeeMusee
{
    /// <summary>
    /// Interaction logic for ListMusicDialog.xaml
    /// </summary>
    public partial class ListMusicDialog : Window
    {
        string playlistKey = "";

        public static event ToggleFormDialogNotifyHandler ToggleForm;


        public ListMusicDialog(string PLKey)
        {
            InitializeComponent();
            ToggleForm();
            this.Closing += ListMusicDialog_Closing;
            playlistKey = PLKey;
            this.DataContext = new ListMusicDialogViewModel(playlistKey);
        }

        private void ListMusicDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleForm();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
