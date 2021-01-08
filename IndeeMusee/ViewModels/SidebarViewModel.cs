using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace IndeeMusee.ViewModels
{
    class SidebarViewModel : BaseViewModel
    {
        public ICommand HomeCommand { get; set; }
        public ICommand MyMusicCommand { get; set; }
        public ICommand FavoriteSongCommand { get; set; }

        public static event ChangePageHandler ChangePage;
        public SidebarViewModel()
        {
            HomeCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ChangePage("Home"); });
            MyMusicCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ChangePage("MyMusic"); });
            FavoriteSongCommand =  new RelayCommand<object>((p) => { return true; }, (p) => { ChangePage("FavoriteSong"); });
        }
    }
}
