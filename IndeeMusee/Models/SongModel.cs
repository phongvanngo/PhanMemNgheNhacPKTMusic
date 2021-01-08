using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndeeMusee.Models
{
    public class SongModel
    {
        public string SongKey { get; set; }
        public string SongName { get; set; }
        public string SongLocation { get; set; }
        public string Title { get; set; }
        public string ContributingArtists { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public string Length { get; set; }
        public string LyricPath { get; set; }
        public string PlayingKey { get; set; }
        public string PlaylistKey { get; set; }
        public int IsFavorite { get; set; }

        public bool IsInPlaylist { get; set; }// trường dữ liệu sử dụng trong ListMusicDialog playlist

        public SongModel Copy()
        {
            SongModel newSong = new SongModel();
            newSong.SongKey = this.SongKey;
            newSong.SongName = this.SongName;
            newSong.SongLocation = this.SongLocation;
            newSong.Title = this.Title;
            newSong.ContributingArtists = this.ContributingArtists;
            newSong.Album = this.Album;
            newSong.Genre = this.Genre;
            newSong.Length = this.Length;
            newSong.LyricPath = this.LyricPath;
            newSong.PlayingKey = this.PlayingKey;
            return newSong;
        }


    }
}
