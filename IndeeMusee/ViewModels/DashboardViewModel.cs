using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IndeeMusee.ViewModels
{

    public class DashboardViewModel
    {
        private MainWindow mainWindow;

        string songName = "hi";


        public DashboardViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public string SongName { get => songName; set => songName = value; }
    }
}
