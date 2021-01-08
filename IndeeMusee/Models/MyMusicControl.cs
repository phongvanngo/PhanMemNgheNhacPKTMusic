using IndeeMusee.DataManager.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IndeeMusee.Models
{
    public static class MyMusicControl
    {
        static List<SongModel> AllUserSongList = new List<SongModel>();
        static List<SongModel> currentSongList = new List<SongModel>();
        static string searchKeyWord="";

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

        public static void FetchUserSongListData()
        {
            AllUserSongList = SongDataAccess.GetAllUserSongList();
            //CurrentSongList = AllUserSongList;
        } 
        
        public static void UserSongListHasUpdated()
        {
            AllUserSongList = SongDataAccess.GetAllUserSongList();
            CurrentSongListChange();
        }

        public static List<SongModel> FilterSongList()
        {
            List<SongModel> filteredList = new List<SongModel>();
            foreach (var song in AllUserSongList)
            {
                // duyệt qua từng thuộc tính của bài hát để xem có trùng với ký tự search nào không

                //biến kiểm tra nếu đã tìm được bài hát thõa keyword thì true, ngược lại false
                foreach (PropertyInfo prop in song.GetType().GetProperties())
                {
                    var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    if (type == typeof(string))
                    {
                        var song_field = prop.GetValue(song, null);
                        if (song_field != null)
                        {
                            string song_data = IndeeMusee.MyUtilites.MyFunction.RemoveUnicode(song_field.ToString().Trim());
                            string keyword = IndeeMusee.MyUtilites.MyFunction.RemoveUnicode(searchKeyWord.Trim());
                            if (song_data.Contains(keyword))
                            {
                                filteredList.Add(song);
                                break;
                            }
                        }
                    }
                    //nếu bài hát đã thõa keyword
                }
            }
            return filteredList;
            
        }
    }
}
