using IndeeMusee.DataManager.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndeeMusee.Models
{
    public static class MusicInPlaylistControl
    {
        static List<SongModel> AllUserSongList = new List<SongModel>();
        static List<SongModel> currentSongList = new List<SongModel>();
        static string searchKeyWord = "";
        public static int TotalSong { get; set; }
        public static int TotalMinute { get; set; }
        static string playlistKey;

        public static event PropertiesChangeHandler CurrentSongListChange;
        public static List<SongModel> CurrentSongList { get => FilterSongList(); set => currentSongList = value; }

        public static string SearchKeyWord
        {
            get => searchKeyWord;
            set
            {
                searchKeyWord = value;
                CurrentSongListChange();
            }
        }

        public static int CalculateTotalMinute()
        {
            int TotalSecond = 0;
            foreach (SongModel song in AllUserSongList)
            {
                TotalSecond += (int)TimeSpan.ParseExact(song.Length, @"mm\:ss", null).TotalSeconds;
            }
            return (int)TimeSpan.FromSeconds(TotalSecond).TotalMinutes;
        }
        public static string PlaylistKey { get => playlistKey; set => playlistKey = value; }

        public static void FetchUserSongListData()
        {
            AllUserSongList = SongDataAccess.GetAllSongInPlaylist(PlaylistKey);
            TotalSong = AllUserSongList.Count;

            CurrentSongList = AllUserSongList;
        }

        public static void UserSongListHasUpdated()
        {
            AllUserSongList = SongDataAccess.GetAllSongInPlaylist(PlaylistKey);
            TotalSong = AllUserSongList.Count;
            if (CurrentSongListChange != null)
                CurrentSongListChange();
        }

        public static List<SongModel> FilterSongList()
        {
            List<SongModel> filteredList = new List<SongModel>();
            return AllUserSongList;
            //foreach (var song in AllUserSongList)
            //{
            //    // duyệt qua từng thuộc tính của bài hát để xem có trùng với ký tự search nào không

            //    //biến kiểm tra nếu đã tìm được bài hát thõa keyword thì true, ngược lại false
            //    foreach (PropertyInfo prop in song.GetType().GetProperties())
            //    {
            //        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            //        if (type == typeof(string))
            //        {
            //            var song_field = prop.GetValue(song, null);
            //            if (song_field != null)
            //            {
            //                string song_data = IndeeMusee.MyUtilites.MyFunction.RemoveUnicode(song_field.ToString().Trim());
            //                string keyword = IndeeMusee.MyUtilites.MyFunction.RemoveUnicode(searchKeyWord.Trim());
            //                if (song_data.Contains(keyword))
            //                {
            //                    filteredList.Add(song);
            //                    break;
            //                }
            //            }
            //        }
            //        //nếu bài hát đã thõa keyword
            //    }
            //}
            //return filteredList;

        }
    }
}
