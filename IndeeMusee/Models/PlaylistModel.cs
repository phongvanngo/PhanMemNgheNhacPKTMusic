using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IndeeMusee.Models
{
    public class PlaylistModel
    {
        public string PlaylistKey { get; set; }
        public string PlaylistName { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int AmountSong { get; set; }
        public ImageSource PlaylistImage { get; set; }

        //filed to check song song exist in playlist, use for Add Playlist Form, feature add to playlist, added
        public bool CheckExistedInPlaylist { get; set; }
    }
}
