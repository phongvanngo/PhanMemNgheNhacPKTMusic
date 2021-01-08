using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IndeeMusee.ViewModels
{
    class ListMusicDialogViewModel : BaseViewModel
    {
        List<SongModel> listMusic = new List<SongModel>();
        List<SongModel> allUserSongList = new List<SongModel>();
        List<string> ListSongToAddPlaylist = new List<string>();
        List<string> ListSongExistedInPlaylist = new List<string>();
        string searchKeyword = "";
        string playlistKey = "";

        public ICommand CheckToChooseSongCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ListMusicDialogViewModel(string PLKey)
        {
            playlistKey = PLKey;

            allUserSongList = SongDataAccess.GetAllSongListWithPlaylistInfo(playlistKey);

            foreach (SongModel song in allUserSongList)
            {
                if (song.PlaylistKey != null)
                {
                    song.IsInPlaylist = true;
                    ListSongExistedInPlaylist.Add(song.SongKey);
                }

            }

            ListMusic = allUserSongList;
            SearchKeyword = "";

            CheckToChooseSongCommand = new RelayCommand<object>(
                (p) =>
                {
                    return true;
                },
                (p) =>
                {
                    SongModel songToAddToPlaylist = p as SongModel;
                    string songToAddToPlaylistKey = (p as SongModel).SongKey;
                    //tìm xem trong check mới hay bỏ check
                    int index = ListSongToAddPlaylist.FindIndex(e => (e == songToAddToPlaylistKey));

                    //trường hợp mới check
                    if (index == -1)
                    {
                        ListSongToAddPlaylist.Add(songToAddToPlaylistKey);
                    }
                    //trường hợp bỏ check
                    else
                    {
                        ListSongToAddPlaylist.RemoveAt(index);
                    }


                });

            SaveCommand = new RelayCommand<object>(
                (p) =>
                {
                    return true;
                },
                (p) =>
                {
                    foreach (string songKey in ListSongToAddPlaylist)
                    {
                        //nếu đã có trong data base thì bỏ ra khỏi database
                        if (ListSongExistedInPlaylist.Exists(e => e == songKey))
                        {
                            if (playlistKey == "FAVORITE_PLAYLIST")
                            {
                                SongDataAccess.RemoveSongFromFavorite(songKey);
                            }
                            else
                            {
                                SongDataAccess.DeleteSongFromPlaylist(songKey, playlistKey);

                            }
                        }
                        else
                        {
                            if (playlistKey == "FAVORITE_PLAYLIST")
                            {
                                SongDataAccess.InsertSongToFavorite(songKey);
                            }
                            else
                            {
                                SongDataAccess.InsertSongToPlaylist(songKey, playlistKey);

                            }
                        }
                    }
                    MusicInPlaylistControl.UserSongListHasUpdated();
                });
        }

        public List<SongModel> ListMusic { get => FilterSongList(); set => listMusic = value; }
        public string SearchKeyword
        {
            get => searchKeyword;
            set
            {
                searchKeyword = value;
                OnPropertyChanged("ListMusic");
                OnPropertyChanged("SearchKeyword");
            }
        }

        public List<SongModel> FilterSongList()
        {
            List<SongModel> filteredList = new List<SongModel>();
            foreach (var song in allUserSongList)
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
                            string keyword = IndeeMusee.MyUtilites.MyFunction.RemoveUnicode(searchKeyword.Trim());
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
