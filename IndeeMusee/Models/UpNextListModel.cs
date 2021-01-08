using IndeeMusee.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IndeeMusee.Models
{
    public static class UpNextListModel
    {
        static List<SongModel> upNextList = new List<SongModel>();

        static bool isEditingLyric = false;

        public static event ChangeSongToPlayHandler ChangeSongToPlayEvent;

        public static event PropertiesChangeHandler UpNextListChange;
        static UpNextListModel()
        {
            //upNextList = IndeeMusee.DataManager.UpNextListData.GetUpNextList();
        }

        public static void GoToSongInPlayingQueue(SongModel selectedSong)
        {
            if (NowPlayingSong.SongIsPlaying != null)
            {
                if (NowPlayingSong.SongIsPlaying.PlayingKey == selectedSong.PlayingKey) return;
                else
                {
                    NowPlayingSong.SongIsPlaying = selectedSong;
                }
            }
            else
            {
                NowPlayingSong.SongIsPlaying = selectedSong;
            }
        }

        public static void RemoveSongOutOfUpNextList(SongModel SongWillRemove)
        {
            List<SongModel> NewPlayingQueue = new List<SongModel>();

            //xóa bài hát trong queue

            for (int i = 0; i < upNextList.Count; i++)
            {
                if (upNextList[i].PlayingKey != SongWillRemove.PlayingKey)
                {
                    NewPlayingQueue.Add(upNextList[i]);
                }
                else
                if (NowPlayingSong.SongIsPlaying != null)
                {
                    if (SongWillRemove.PlayingKey == NowPlayingSong.SongIsPlaying.PlayingKey)
                    {
                        if (upNextList.Count >= 2)
                        {
                            LoadNextSong();
                        }
                        else
                        {
                            NowPlayingSong.StopSong();
                        }
                    }
                }
            }
            upNextList = NewPlayingQueue;
            UpNextListChange();


            //tìm index của bài hát đang chạy đẻ thông báo cho now playing view;
            for (int i = 0; i < upNextList.Count; i++)
            {
                if (NowPlayingSong.SongIsPlaying.PlayingKey == upNextList[i].PlayingKey)
                {
                    ChangeSongToPlayEvent(i);
                    return;
                }
            }
        }

        public static void RemoveAllQueue()
        {
            List<SongModel> NewPlayingQueue = new List<SongModel>();

            //xóa bài hát trong queue

            for (int i = 0; i < upNextList.Count; i++)
            {
                //if (upNextList[i].PlayingKey != SongWillRemove.PlayingKey)
                //{
                //    NewPlayingQueue.Add(upNextList[i]);
                //}
                //else
                if (NowPlayingSong.SongIsPlaying != null)
                {
                    if (upNextList[i].PlayingKey == NowPlayingSong.SongIsPlaying.PlayingKey)
                    {
                        //if (upNextList.Count >= 2)
                        //{
                        //    LoadNextSong();
                        //}
                        //else
                        //{
                        //    NowPlayingSong.StopSong();
                        //}
                        NewPlayingQueue.Add(upNextList[i]);
                    }
                }
            }
            upNextList = NewPlayingQueue;
            UpNextListChange();


            //tìm index của bài hát đang chạy đẻ thông báo cho now playing view;
            for (int i = 0; i < upNextList.Count; i++)
            {
                if (NowPlayingSong.SongIsPlaying.PlayingKey == upNextList[i].PlayingKey)
                {
                    ChangeSongToPlayEvent(i);
                    return;
                }
            }
        }
        public static void AddSongToUpNextList(SongModel song)
        {
            SongModel newSong = song.Copy();
            List<SongModel> NewPlayingQueue = new List<SongModel>(upNextList);

            newSong.PlayingKey = MyUtilites.MyFunction.GenerateCode();
            NewPlayingQueue.Add(newSong);

            upNextList = NewPlayingQueue;
            UpNextListChange();
        }
        public static void AddSongToUpNextListAndPlay(SongModel song)
        {
            if (song == null) return;
            SongModel newSong = song.Copy();
            //upNextList.Add(song);
            List<SongModel> NewPlayingQueue = new List<SongModel>(upNextList);

            newSong.PlayingKey = MyUtilites.MyFunction.GenerateCode();
            NewPlayingQueue.Add(newSong);

            upNextList = NewPlayingQueue;

            UpNextListChange();
            ChangeSongToPlayEvent(upNextList.Count - 1);
        }

        public static void PlayAllSongList(List<SongModel> songList)
        {
            List<SongModel> NewPlayingQueue = new List<SongModel>(upNextList);
            int indexSongToPlay = NewPlayingQueue.Count;
            foreach (SongModel song in songList)
            {
                SongModel newSong = song.Copy();
                newSong.PlayingKey = MyUtilites.MyFunction.GenerateCode();
                NewPlayingQueue.Add(newSong);
            }
            UpNextList = NewPlayingQueue;
            UpNextListChange();

            if (indexSongToPlay <= NewPlayingQueue.Count)
            {
                ChangeSongToPlayEvent(indexSongToPlay);
            }
        }

        public static void LoadNextSong()
        {
            for (int i = 0; i < upNextList.Count; i++)
            {
                if (NowPlayingSong.SongIsPlaying.PlayingKey == upNextList[i].PlayingKey)
                {
                    if (i + 1 < upNextList.Count)
                    {
                        //NowPlayingSong.SongIsPlaying = upNextList[i + 1];
                        ChangeSongToPlayEvent(i + 1);
                    }
                    else
                    {
                        //NowPlayingSong.SongIsPlaying = upNextList[0];
                        if (NowPlayingSong.ReplayStatus == 0)
                        {
                            ChangeSongToPlayEvent(0);
                        }

                        else if (NowPlayingSong.ForcePlayNextSong == true)
                        {
                            ChangeSongToPlayEvent(0);
                        }


                    }
                    return;
                }
            }
        }
        public static void LoadRandomSong()
        {
            Random Ran = new Random();
            while (true)
            {
                int NextSongIndex = Ran.Next(0, upNextList.Count - 1);

                ChangeSongToPlayEvent(NextSongIndex);
                return;
            }

        }

        public static void LoadPreviousSong()
        {
            for (int i = 0; i < upNextList.Count; i++)
            {
                if (NowPlayingSong.SongIsPlaying.PlayingKey == upNextList[i].PlayingKey)
                {
                    if (i > 0)
                    {
                        //NowPlayingSong.SongIsPlaying = upNextList[i - 1]; 
                        ChangeSongToPlayEvent(i - 1);
                    }
                    else
                    {
                        //NowPlayingSong.SongIsPlaying = upNextList[i];
                        ChangeSongToPlayEvent(i);
                    }
                    return;
                }
            }
        }

        public static List<SongModel> UpNextList { get => upNextList; set => upNextList = value; }
        public static bool IsEditingLyric { get => isEditingLyric; set => isEditingLyric = value; }
    }
}
