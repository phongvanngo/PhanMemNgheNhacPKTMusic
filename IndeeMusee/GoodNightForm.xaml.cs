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
    /// Interaction logic for GoodNightForm.xaml
    /// </summary>
    public partial class GoodNightForm : Window
    {

        public static event ToggleFormDialogNotifyHandler ToggleForm;
        public GoodNightForm()
        {
            InitializeComponent();
            ToggleForm();
            this.Closing += GoodNightForm_Closing;
        }

        private void GoodNightForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleForm();
        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            NowPlayingSong.IsPlaying = true;
            this.Close();
        }

        private void BtnCloseApp_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
