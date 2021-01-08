using IndeeMusee.DataManager;
using IndeeMusee.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace IndeeMusee.Models
{
    public static class NowPlayingSong
    {
        public static int ThreadSpecificStaticValue;

        static MediaPlayer mediaPlayer = new MediaPlayer();
        static DispatcherTimer timer = new DispatcherTimer();

        static bool loadSongStatus = true;

        static DispatcherTimer timerLoadSong = new DispatcherTimer();

        static bool isMouseDown = false;

        static List<LyricModel> songLyric = new List<LyricModel>();
        static int IndexPositionLyric = 0;

        private static SongModel songIsPlaying = new SongModel();

        //0: lặp lại danh sách phát, 1: lặp lại bài hát, 2:không lặp lại danh sách phát
        public static int ReplayStatus { get; set; }

        //giải quyết cho vấn đề không play next song khi ReplayStatus  = 2
        public static bool ForcePlayNextSong { get; set; }

        static double appVolume = 20;
        static double songDuration = 0;
        static string songLength = "00:00";
        static double currentProgress = 0;
        static bool replaySong = false;
        static bool isPlaying = false;
        static bool isShuffle = false;

        //biến kiểm tra tính hợp lện của đường link mp3
        static bool isValidSongPath = true;

        static string songStatusTime = "00:00";

        public static event PropertiesChangeHandler CurrentSongProgressChange;
        public static event PropertiesChangeHandler NewSongIsLoad;
        public static event PropertiesChangeHandler IsPlayingChange;
        public static event ChangeLyricHandler ChangeLyric;
        public static event NotifyHandler SetupNewLyric; //thông báo cho lyric khi chuyển bài hát khác
        public static string SongStatusTime { get => songStatusTime; set => songStatusTime = value; }
        public static string SongLength { get => songLength; set => songLength = value; }

        public static double AppVolume
        {
            get => appVolume;
            set { appVolume = value; mediaPlayer.Volume = appVolume; }
        }
        public static double SongDuration
        {
            get => songDuration;
            set
            {
                songDuration = value;
            }
        }
        public static double CurrentProgress
        {
            get => currentProgress;
            set
            {
                currentProgress = value;
                if (CurrentSongProgressChange != null) CurrentSongProgressChange();
            }
        }
        public static bool IsPlaying
        {
            get => isPlaying;
            set
            {
                isPlaying = value;
                if (isPlaying == true)
                {
                    PlaySong();
                }
                else
                {
                    PauseSong();
                }
                IsPlayingChange();
            }
        }

        public static bool ReplaySong { get => replaySong; set { replaySong = value; } }
        public static bool IsValidSongPath { get => isValidSongPath; set => isValidSongPath = value; }


        public static SongModel SongIsPlaying
        {
            get => songIsPlaying;
            set
            {
                songIsPlaying = value;
                if (value != null)
                {
                    LoadSongStatus = false;
                    GeneralDataManagement.InsertSongToRecentPlay(songIsPlaying.SongKey);
                    if (mediaPlayer.HasAudio) mediaPlayer.Stop();
                    timerLoadSong = new DispatcherTimer();
                    timerLoadSong.Tick += TimerLoadSong_Tick;
                    timerLoadSong.Start();
                }
            }
        }

        private static void TimerLoadSong_Tick(object sender, EventArgs e)
        {
            timerLoadSong.Stop();
            SetupSong();
            IsPlaying = true;
            //NowPlayingView.myTimer.Start();
            PlaySong();
            LoadSongStatus = true;
        }

        public static bool IsShuffle { get => isShuffle; set => isShuffle = value; }
        public static bool IsMouseDown { get => isMouseDown; set => isMouseDown = value; }
        public static MediaPlayer AppMediaPlayer { get => mediaPlayer; set => mediaPlayer = value; }
        public static bool LoadSongStatus { get => loadSongStatus; set => loadSongStatus = value; }

        public static void PlayNextSong()
        {
            if (IsShuffle == true)
                UpNextListModel.LoadRandomSong();
            else
                UpNextListModel.LoadNextSong();
        }
        public static void PlayPreviousSong()
        {
            if (CurrentProgress < 3)
            {
                UpNextListModel.LoadPreviousSong();
            }
            else
            {
                mediaPlayer.Position = new TimeSpan(0, 0, 0);
                IndexPositionLyric = 0;
            }
        }
        public static bool CheckValidPathMp3(string song_path)
        {
            if (File.Exists($@"{GeneralDataManagement.UserMusicFolderLocation}\{SongIsPlaying.SongLocation}") == true) return true; else return false;
        }
        static NowPlayingSong()
        {
            timer.Interval = TimeSpan.FromSeconds(0.05);
            mediaPlayer.Volume = appVolume;
            timer.Tick += Timer_Tick;
            SetupSong();
        }

        public static void SoundSmallDown()
        {
            int TimeDelayRemain = 30;
            while (mediaPlayer.Volume > 0 || TimeDelayRemain > 0)
            {
                TimeDelayRemain--;
                mediaPlayer.Volume -= 0.02;
                Thread.Sleep(20);
            }
        }

        public static void StopSong()
        {
            timer.Stop();
            mediaPlayer.Stop();
            IsPlaying = false;
            SongIsPlaying = null;
            CurrentProgress = 0;
            SongLength = "00:00";
            NewSongIsLoad();
            IsValidSongPath = false;
        }

        public static void SetupSong()
        {
            isValidSongPath = CheckValidPathMp3(SongIsPlaying.SongLocation);
            if (isValidSongPath == false) return;

            if (mediaPlayer.HasAudio)
            {
                //SoundSmallDown();
                mediaPlayer.Close();
                timer.Stop();
            }

            songLyric = SongLyricsModel.GetLyricsFromFile($@"{GeneralDataManagement.LyricFolderPath}\{songIsPlaying.LyricPath}");
            IndexPositionLyric = 0;
            SetupNewLyric();

            mediaPlayer.Open(new Uri($@"{GeneralDataManagement.UserMusicFolderLocation}\{SongIsPlaying.SongLocation}"));
            while (mediaPlayer.NaturalDuration.HasTimeSpan == false)
            {
            }

            //truong hop ko co loi bai hat
            if (songLyric == null)
            {
                LyricModel lyric = new LyricModel(0, (int)mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds, $"Bài hát: {songIsPlaying.Title} - {songIsPlaying.ContributingArtists}");
                songLyric = new List<LyricModel>();
                songLyric.Add(lyric);
            }

            //lấy thời lượng của bài hát
            SongDuration = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            SongLength = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            NewSongIsLoad();
            mediaPlayer.Volume = appVolume;
            CurrentProgress = 0;

        }


        public static void PlaySong()
        {
            if (isValidSongPath == false) return;
            timer.Start();
            mediaPlayer.Play();
        }
        private static void Timer_Tick(object sender, EventArgs e)
        {
            if (isValidSongPath == false) return;

            if (isMouseDown == true) return;

            CurrentProgress = mediaPlayer.Position.TotalSeconds;
            if (mediaPlayer.Position == mediaPlayer.NaturalDuration)
            {
                if (ReplayStatus == 1)
                {
                    //chơi lại bài hát từ đầu
                    IndexPositionLyric = 0;
                    mediaPlayer.Position = new TimeSpan(0, 0, 0);
                }
                else
                {
                    timer.Stop();
                    CurrentProgress = 0;
                    IndexPositionLyric = 0;
                    PlayNextSong();
                }


            }
        }
        public static void PauseSong()
        {
            if (isValidSongPath == false) return;
            timer.Stop();
            //SoundSmallDown();
            mediaPlayer.Pause();
            mediaPlayer.Volume = appVolume;
        }

        public static int FindIndexLyric(int TimePosition)
        {
            int left = 0;
            int right = songLyric.Count - 1;
            while (left + 1 < right)
            {
                int mid = (left + right) / 2;
                if (TimePosition >= songLyric[mid].TimeStart)
                {
                    left = mid;
                }
                else
                {
                    right = mid - 1;
                }
            }
            return left;
        }

        public static void SongProgressChange(double newTime)
        {
            string nextLyric = "";

            if (isValidSongPath == false) return;

            if (isMouseDown == true) return;


            if (Math.Abs(newTime - mediaPlayer.Position.TotalSeconds) >= 2)
            {
                //người dùng kéo thanh slider
                timer.Stop();
                mediaPlayer.Pause();
                mediaPlayer.Position = new TimeSpan(0, (int)(newTime / 60), (int)newTime % 60);

                if (songLyric != null && songLyric.Count > 0)
                {
                    if (IndexPositionLyric + 1 < songLyric.Count) nextLyric = songLyric[IndexPositionLyric + 1].Content;
                    IndexPositionLyric = FindIndexLyric((int)mediaPlayer.Position.TotalMilliseconds);
                    ChangeLyric(songLyric[IndexPositionLyric], nextLyric);
                    //IndexPositionLyric++;
                    SetupNewLyric();
                }

                if (isPlaying == true)
                {
                    timer.Start();
                    mediaPlayer.Play();
                }
            }
            //cập nhật thời gian
            SongStatusTime = mediaPlayer.Position.ToString(@"mm\:ss");

            //lyric animation
            if (songLyric != null)
            {
                if (IndexPositionLyric < songLyric.Count)
                {

                    if (songLyric[IndexPositionLyric].TimeStart < mediaPlayer.Position.TotalMilliseconds)
                    {
                        if (IndexPositionLyric + 1 < songLyric.Count) nextLyric = songLyric[IndexPositionLyric + 1].Content;
                        songLyric[IndexPositionLyric].TimeStart = (int)mediaPlayer.Position.TotalMilliseconds;
                        ChangeLyric(songLyric[IndexPositionLyric], nextLyric);
                        IndexPositionLyric++;
                    }
                }
            }
        }

    }
}
