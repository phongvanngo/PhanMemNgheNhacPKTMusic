using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IndeeMusee.ViewModels
{
    class PlaylistBarViewModel
    {
        public static event ChangePageHandler ChangePage;
        public ICommand PlaylistCommand { get; set; }
        public ICommand ExploreCommand { get; set; }
        public ICommand KaraokeCommand { get; set; }

        public PlaylistBarViewModel()
        {
            PlaylistCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ChangePage("Playlist"); });
            ExploreCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ChangePage("Explore"); });
            KaraokeCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ChangePage("Karaoke"); });
        }
    }
}
