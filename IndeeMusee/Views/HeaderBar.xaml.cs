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
using System.Windows.Threading;

namespace IndeeMusee.Views
{
    /// <summary>
    /// Interaction logic for HeaderBar.xaml
    /// </summary>
    public partial class HeaderBar : UserControl
    {
        public static event ChangePageHandler ChangePage;

        static bool exploreOnlineMode = false;

        public static bool FirstTimeSearch = true;

        public static bool ExploreOnlineMode = false;

        public static event SearchMusicHandler SearchRequest;

        public static bool IsInMyMusicView = false;

        DispatcherTimer NewThread = new DispatcherTimer();
        public HeaderBar()
        {
            InitializeComponent();
            MainWindow.SearchRequest += MainWindow_SearchRequest;
            MainWindow.DeleteSearchContent += MainWindow_DeleteSearchContent;
            NewThread.Tick += NewThread_Tick;
        }

        private void MainWindow_DeleteSearchContent()
        {
            TbSearch.Text = "";
        }

        private void NewThread_Tick(object sender, EventArgs e)
        {
            NewThread.Stop();
            MyMusicControl.SearchKeyWord = TbSearch.Text;

            if (IsInMyMusicView == false && TbSearch.Text != "")
            {
                ChangePage("MyMusic");
                IsInMyMusicView = true;
            }
        }

        private void MainWindow_SearchRequest()
        {
            TbSearch.Focus();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ExploreOnlineMode == false)
            {
                NewThread.Start();
            }
            else
            {

            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TbSearch.Focus();
        }

        private void TbSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ExploreOnlineMode == true)
                {
                    if (SearchRequest != null)
                    {
                        SearchRequest(TbSearch.Text);
                    }
                }
            }
        }
    }
}
