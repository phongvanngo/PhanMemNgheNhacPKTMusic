using IndeeMusee.DataManager;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for UtilitiesControl.xaml
    /// </summary>
    public partial class UtilitiesControl : UserControl
    {

        public static event ChangePageHandler ChangePage;
        public UtilitiesControl()
        {
            InitializeComponent();
            labelUserName.Content = System.Environment.MachineName;
        }
        private void BtnChangeFolder_Click(object sender, RoutedEventArgs e)
        {
            ChooseMusicFolderForm form_choose_music_folder = new ChooseMusicFolderForm();

            form_choose_music_folder.UserPath = GeneralDataManagement.UserMusicFolderLocation;
            if (form_choose_music_folder.ShowDialog() == true)
            {
                //ghi đường dẫn thư mục vào file MusicLocation.txt;
                System.IO.File.WriteAllText(GeneralDataManagement.FileLinkMusicLocationPath, GeneralDataManagement.UserMusicFolderLocation);

                if (IndeeMusee.DataManager.GeneralDataManagement.IsHavingDatabaseInUserMusicFolder() == false)
                {
                    IndeeMusee.DataManager.GeneralDataManagement.InitializeDatabase();
                    IndeeMusee.DataManager.GeneralDataManagement.FindAndImportSongToDatabase();
                }

                IndeeMusee.DataManager.GeneralDataManagement.RemoveInvalidSongFromDatabase();

                MyMusicControl.UserSongListHasUpdated();

                ChangePage("MyMusic");

            }
            else
            //nếu không chọn thư mục mà tắt form đi
            {

            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnSleepTimer_Click(object sender, RoutedEventArgs e)
        {
            SleepTimerForm sleepTimerForm = new SleepTimerForm();
            sleepTimerForm.ShowDialog();
            if (sleepTimerForm.DialogResult == true)
            {

            }
            else
            {

            }
        }
    }
}
