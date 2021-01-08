using IndeeMusee.DataManager;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IndeeMusee
{
    /// <summary>
    /// Interaction logic for ChooseMusicFolderForm.xaml
    /// </summary>
    public partial class ChooseMusicFolderForm : Window
    {
        public string UserPath { get; set; }

        public static event ToggleFormDialogNotifyHandler ToggleForm;

        public ChooseMusicFolderForm()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(IndeeMusee.DataManager.GeneralDataManagement.UserMusicFolderLocation) == false)
            {
                tb.Text = IndeeMusee.DataManager.GeneralDataManagement.UserMusicFolderLocation;
            };

            if (ToggleForm != null) ToggleForm();
            this.Closing += ChooseMusicFolderForm_Closing;
        }

        private void ChooseMusicFolderForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ToggleForm != null) ToggleForm();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            if (Directory.Exists(GeneralDataManagement.UserMusicFolderLocation) == true) fbd.SelectedPath = GeneralDataManagement.UserMusicFolderLocation;
            
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                tb.Text = fbd.SelectedPath;
                UserPath = fbd.SelectedPath;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //kiểm tra null, thông báo
            if (Directory.Exists(UserPath) == true)
            {
                IndeeMusee.DataManager.GeneralDataManagement.UserMusicFolderLocation = tb.Text;
                this.DialogResult = true;
                this.Close();

            }
            else
            {
                System.Windows.MessageBox.Show("Đường dẫn không hợp lệ");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
