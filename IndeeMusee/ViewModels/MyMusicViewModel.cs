using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using IndeeMusee.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IndeeMusee.ViewModels
{
    class MyMusicViewModel : BaseViewModel
    {
        List<SongModel> currentSongList = new List<SongModel>();
        public ICommand PlayNowCommand { get; set; }
        public ICommand AddToPlayingQueueCommand { get; set; }
        public ICommand MoreControlCommand { get; set; }
        public ICommand AddToPlaylistCommand { get; set; }
        public ICommand FavoriteCommand { get; set; }
        public ICommand PlayAllCommand { get; set; }
        public ICommand AddSongCommand { get; set; }

        public event ChangeSelectedIndex ButtonFavoriteSongClick;
        public List<SongModel> CurrentSongList
        {
            get
            {
                currentSongList = MyMusicControl.CurrentSongList;
                return currentSongList;
            }
            set { currentSongList = value; }
        }

        public event AddNewSongHandler AddNewSongEvent;

        public MyMusicViewModel()
        {

            PlayNowCommand = new RelayCommand<object>(
                (p) => 
                {
                    return NowPlayingSong.LoadSongStatus; 
                },
                (p) =>
                {
                    SongModel songSelected = p as SongModel;
                    int index = CurrentSongList.FindIndex(e => (e.SongKey == songSelected.SongKey));
                    PlayNowRequest(index);
                });

            FavoriteCommand = new RelayCommand<object>((p) => { return true; },
                (p) =>
                {
                    SongModel songSelected = p as SongModel;
                    int index = CurrentSongList.FindIndex(e => (e.SongKey == songSelected.SongKey));
                    if (songSelected.IsFavorite == 1)
                    {
                        //bỏ thích
                        SongDataAccess.RemoveSongFromFavorite(songSelected.SongKey);
                        ButtonFavoriteSongClick(index,0);
                    }
                    else
                    {
                        //thích
                        SongDataAccess.InsertSongToFavorite(songSelected.SongKey);
                        ButtonFavoriteSongClick(index,1);
                    }
                });

            MoreControlCommand = new RelayCommand<object>((p) => { return true; },
                (p) =>
                {
                    SongModel songSelected = p as SongModel;
                    MiniTool.SongDetail = songSelected;
                });
            AddToPlayingQueueCommand = new RelayCommand<object>((p) => { return true; },
                (p) =>
                {
                    SongModel songSelected = p as SongModel;
                    int index = CurrentSongList.FindIndex(e => (e.SongKey == songSelected.SongKey));
                    AddToPlayingQueueRequest(index);
                });

            AddSongCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                openFileDialog.Filter = "All Supported Audio | *.mp3; *.wma | MP3s | *.mp3 | WMAs | *.wma| WAVs | *.wav";
                openFileDialog.InitialDirectory = GeneralDataManagement.UserMusicFolderLocation;
                openFileDialog.Multiselect = true;
                System.Windows.Forms.DialogResult dialogResult = openFileDialog.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    // Read the files
                    foreach (String file in openFileDialog.FileNames)
                    {
                        try
                        {
                            SongModel newSong = new SongModel();

                            if (SongDataAccess.CheckSongExistInDatabase(Path.GetFileName(file)))
                            {
                                continue;
                            }

                            var tfile = TagLib.File.Create(file);

                            newSong.SongName = Path.GetFileName(file);

                            if (tfile.Tag.Title == null) tfile.Tag.Title = newSong.SongName;
                            if (tfile.Tag.Title.Trim() == "") tfile.Tag.Title = newSong.SongName;

                            newSong.Title = tfile.Tag.Title;

                            newSong.Album = tfile.Tag.Album;
                            if (tfile.Tag.Artists.Length != 0)
                            {
                                newSong.ContributingArtists = tfile.Tag.Artists[0];
                            }
                            else
                            {
                                newSong.ContributingArtists = "unknown";
                            }

                            if (tfile.Tag.Genres.Length != 0)
                            {
                                newSong.Genre = tfile.Tag.Genres[0];
                            }
                            else
                            {
                                newSong.Genre = "unknown";
                            }

                            TimeSpan songLength = TimeSpan.FromSeconds(tfile.Properties.Duration.TotalSeconds);
                            newSong.Length = songLength.ToString(@"mm\:ss");

                            tfile.Tag.InitialKey = MyUtilites.MyFunction.GenerateCode();
                            newSong.SongKey = tfile.Tag.InitialKey;

                            try
                            {
                                tfile.Save();
                                //di chuyển bài hát vào thư mục nhạc của người dùng
                                string sourceFile = file;
                                string destFile = GeneralDataManagement.UserMusicFolderLocation + "/" + newSong.SongName;
                                System.IO.File.Copy(sourceFile,destFile,true);
                                newSong.SongLocation = destFile.Replace(GeneralDataManagement.UserMusicFolderLocation,"");
                                SongDataAccess.InsertSongToDatabase(newSong);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Thêm bài hát thất bại");
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Thêm bài hát thất bại");
                        }

                    }
                };
                MyMusicControl.UserSongListHasUpdated();
                AddNewSongEvent();
            });
            PlayAllCommand = new RelayCommand<object>((p) => { return true; }, (p) => { PlayAllSongRequest(); });
            AddToPlaylistCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                SongModel songSelected = p as SongModel;
                int index = CurrentSongList.FindIndex(e => (e.SongKey == songSelected.SongKey));
                AddToPlayingQueueRequest(index);
            });

            MyMusicControl.CurrentSongListChange += MyMusicControl_CurrentSongListChange;

            MyMusicControl.FetchUserSongListData();
            currentSongList = MyMusicControl.CurrentSongList;

        }

        private void MyMusicControl_CurrentSongListChange()
        {
            OnPropertyChanged("CurrentSongList");
        }

        public void PlayAllSongRequest()
        {
            UpNextListModel.PlayAllSongList(MyMusicControl.CurrentSongList);
        }

        public void PlayNowRequest(int index)
        {
            SongModel requestedSong = currentSongList[index];
            UpNextListModel.AddSongToUpNextListAndPlay(requestedSong);

        }

        public void AddToPlayingQueueRequest(int index)
        {
            UpNextListModel.AddSongToUpNextList(currentSongList[index]);
        }
    }
}
