using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IndeeMusee.ViewModels
{
    class LyricEditorViewModel : BaseViewModel
    {
        static SongModel songToEdit;
        string lyricContent;
        string currentSongTimePosition;
        bool insertMode = false;
        DispatcherTimer timer = new DispatcherTimer();

        public static SongModel SongToEdit { get => songToEdit; set => songToEdit = value; }
        public string LyricContent { get => lyricContent; set => lyricContent = value; }
        public string CurrentSongTimePosition { get => currentSongTimePosition; set => currentSongTimePosition = value; }
        public bool InsertMode { get => insertMode; set => insertMode = value; }

        public LyricEditorViewModel()
        {
            LyricContent = songToEdit.SongName;
            timer.Tick += Timer_Tick;
            timer.Start();
            UpNextListModel.AddSongToUpNextListAndPlay(SongToEdit);
            UpNextListModel.IsEditingLyric = true;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(NowPlayingSong.AppMediaPlayer !=null)
            {
                CurrentSongTimePosition = NowPlayingSong.AppMediaPlayer.Position.ToString(@"mm\:ss\.ff");
                OnPropertyChanged("CurrentSongTimePosition");
            }
        }
    }
}
