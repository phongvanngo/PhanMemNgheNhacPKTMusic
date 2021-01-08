using IndeeMusee.DataManager;
using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using IndeeMusee.Views;
using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace IndeeMusee.ViewModels
{

    public class DownloadRequestInfo
    {
        public string LinkDownload { get; set; }
        public string DestinationFilePath { get; set; }

        public DownloadRequestInfo(string linkDownload, string destinationFilePath)
        {
            LinkDownload = linkDownload;
            DestinationFilePath = destinationFilePath;
        }
    }

    public class NewThreadClass
    {
        public static bool Done { get; set; }

        public static bool IsDownLoad = false;
        public static int StatusDownloadAndPlay { get; set; }
        //0: chua tai xong, 200: tai thanh cong, 500: khong tai duoc, 400: khong cho tai

        public static List<OnlineSong> ListSong;

        public static void CrawlData(object searchContent)
        {
            ListSong = null;
            string searching = searchContent as string;
            HttpRequest http = new HttpRequest();
            string searchUrl = @"https://nhac.vn/search?q=" + searching;
            string htmlSearch = "";
            try
            {
                htmlSearch = http.Get(searchUrl).ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi kết nối mạng");
            }
            string dataPattern = @"<div class=""box_content"">(.*?)<div class=""video_widget video_list_large box""";
            var dsData = Regex.Matches(htmlSearch, dataPattern, RegexOptions.Singleline);
            List<OnlineSong> newListSong = new List<OnlineSong>();
            //Lay cai bang dau tien thoi 
            try
            {
                string dsBaiHat = dsData[0].ToString();
                dataPattern = @"<li class=""song-list-new-item""(.*?)</li>";
                var listSongHTML = Regex.Matches(dsBaiHat, dataPattern, RegexOptions.Singleline);

                foreach (var item in listSongHTML)
                {
                    dataPattern = @"<a\s\S*title=""(.*?)""";

                    // Loc ra link nhac 
                    var linkMusic = Regex.Matches(item.ToString(), @"href=""(.*?)""", RegexOptions.Singleline);
                    string stringMusic = linkMusic[0].ToString();
                    int indexMusic = stringMusic.IndexOf("href=");
                    string musicLinkfinal = stringMusic.Substring(indexMusic, stringMusic.Length - indexMusic - 1).Replace("href=\"", "");

                    //Lay link download
                    string downloadLink = GetDownloadLink(musicLinkfinal);

                    if (downloadLink == null) return;


                    //Lay ra ten bai hat va ca si
                    var songandsinger = Regex.Matches(item.ToString(), dataPattern, RegexOptions.Singleline);


                    // Loc ra ten bai hat
                    string songString = songandsinger[0].ToString();
                    int indexSong = songString.IndexOf("title=\"");
                    string songName = songString.Substring(indexSong, songString.Length - indexSong - 1).Replace("title=\"", "");

                    //Loc ra ten ca si
                    string singerString = songandsinger[1].ToString();
                    int indexSinger = singerString.IndexOf("title=\"");
                    string singerName = singerString.Substring(indexSinger, singerString.Length - indexSinger - 1).Replace("title=\"", "");

                    //Loc ra link hinh anh
                    var imageSource = Regex.Matches(item.ToString(), @"<img src=""(.*?)""", RegexOptions.Singleline);
                    string imageString = imageSource[0].ToString();
                    int indexImage = imageString.IndexOf("<img src=");
                    string imageLink = imageString.Substring(indexImage, imageString.Length - indexImage - 1).Replace("<img src=\"", "");

                    newListSong.Add(new OnlineSong(songName, singerName, imageLink, musicLinkfinal, downloadLink));
                }
            }
            catch (Exception)
            {

            }
            ListSong = newListSong;
            Done = true;
        }

        public static string GetDownloadLink(string link)
        {
            HttpRequest http = new HttpRequest();
            string htmlResult = http.Get(link).ToString();

            var htmlDownloadLink = Regex.Match(htmlResult, @"""file"":""(.*?)""", RegexOptions.Singleline);
            string tag_DownloadLink = htmlDownloadLink.ToString();
            string final_DownloadString = tag_DownloadLink.Replace("\"file\":\"", "");
            final_DownloadString = final_DownloadString.Replace(@"\", "");

            if (!String.IsNullOrEmpty(final_DownloadString))
                final_DownloadString = final_DownloadString.Substring(0, final_DownloadString.Length - 1);

            return final_DownloadString;
        }

        public static void DownloadSong(object DownloadInfo)
        {
            IsDownLoad = true;

            string LinkDownload = (DownloadInfo as DownloadRequestInfo).LinkDownload;
            string filePath = (DownloadInfo as DownloadRequestInfo).DestinationFilePath;

            if (!String.IsNullOrEmpty(LinkDownload))
            {
                WebClient webClient = new WebClient();
                //Thay doi path o day
                try
                {
                    webClient.DownloadFile(LinkDownload, filePath);
                    //tai thanh cong
                    StatusDownloadAndPlay = 200;
                }
                catch (Exception)
                {
                    //khong tai duoc
                    StatusDownloadAndPlay = 500;
                }

            }
            else
            {
                //khong cho tai
                StatusDownloadAndPlay = 400;
            }

            IsDownLoad = false;
        }
    }


    public class OnlineSong
    {
        private string songName;
        private string singerName;
        private string imageSource;
        private string linkMusic;
        private string linkDownload;

        public OnlineSong(string songname, string singer, string image, string musicLink, string downloadLink)
        {
            SongName = songname;
            SingerName = singer;
            ImageSource = image;
            LinkMusic = musicLink;
            linkDownload = downloadLink;
        }

        public string SongName { get => songName; set => songName = value; }
        public string SingerName { get => singerName; set => singerName = value; }
        public string ImageSource { get => imageSource; set => imageSource = value; }
        public string LinkMusic { get => linkMusic; set => linkMusic = value; }
        public string LinkDownload { get => linkDownload; set => linkDownload = value; }

    }
    class ExploreMusicViewModel : BaseViewModel
    {
        List<OnlineSong> listSong = new List<OnlineSong>();

        DispatcherTimer TimerSearching;
        DispatcherTimer LoadingTimer;

        public ICommand PlaySongCommand { get; set; }
        public ICommand DownloadCommand { get; set; }

        public bool IsLoading { get; set; }

        public static event NotifyHandler ToggleLoadingModeRequest;
        public List<OnlineSong> ListSong { get => listSong; set => listSong = value; }

        public ExploreMusicViewModel()
        {
            HeaderBar.SearchRequest += HeaderBar_SearchRequest;

            IsLoading = false;

            PlaySongCommand = new RelayCommand<object>((p) => { return !NewThreadClass.IsDownLoad; },
                (p) =>
                {


                    OnlineSong songSelected = p as OnlineSong;

                    string songName = $@"{songSelected.SongName} - {songSelected.SingerName}.mp3";
                    string filePath = $@"{GeneralDataManagement.UserMusicFolderLocation}\{songName}";
                    SongModel songToPlay;


                    if (File.Exists(filePath) == true)
                    {
                        //nếu bài hát đã tải
                        songToPlay = SongDataAccess.GetSong_SongLocation(filePath.Replace($@"{GeneralDataManagement.UserMusicFolderLocation}\", ""));
                        if (songToPlay != null)
                        {
                            //bài hát còn trong chương trình
                            UpNextListModel.AddSongToUpNextListAndPlay(songToPlay);
                        }
                        else
                        {
                            //bài hát đã xóa khỏi chương trình
                            songToPlay = InsertSongToDatabase(filePath);
                            UpNextListModel.AddSongToUpNextListAndPlay(songToPlay);
                        }

                    }
                    else
                    {
                        //nếu bài hát chưa tải
                        NowPlayingSong.IsPlaying = false;
                        DownloadSongAndPlay(songSelected.LinkDownload, filePath, songSelected.LinkMusic);
                    }
                });

            DownloadCommand = new RelayCommand<object>((p) => { return !NewThreadClass.IsDownLoad; },
                (p) =>
                {
                    if (NewThreadClass.IsDownLoad == true) return;

                    OnlineSong songSelected = p as OnlineSong;

                    string songName = $@"{songSelected.SongName} - {songSelected.SingerName}.mp3";
                    string filePath = $@"{GeneralDataManagement.UserMusicFolderLocation}\{songName}";

                    if (File.Exists(filePath) == true)
                    {
                        MessageBox.Show("Tải xuống thành công");
                    }

                    else
                    {

                        DownloadSongToApp(songSelected.LinkDownload, filePath);
                    }
                });
        }


        #region download song and play

        DispatcherTimer WaitForDownloadTimer;
        string DownloadedFilePath;
        string CurrentLinkSongSelected;
        string OnlineLinkMusic;
        private void DownloadSongAndPlay(string LinkDownload, string DestinationFilePath, string onlineLinkMusic)
        {

            IsLoading = true;
            ToggleLoadingModeRequest();

            DownloadedFilePath = DestinationFilePath;
            OnlineLinkMusic = onlineLinkMusic;

            NewThreadClass.StatusDownloadAndPlay = 0;
            Thread ThreadDownloadSong = new Thread(NewThreadClass.DownloadSong);
            ThreadDownloadSong.Start(new DownloadRequestInfo(LinkDownload, DestinationFilePath));

            WaitForDownloadTimer = new DispatcherTimer();
            WaitForDownloadTimer.Tick += WaitForDownloadTimer_Tick;
            WaitForDownloadTimer.Start();


        }
        private void WaitForDownloadTimer_Tick(object sender, EventArgs e)
        {
            if (NewThreadClass.StatusDownloadAndPlay == 0) return;

            //download xong
            WaitForDownloadTimer.Stop();

            switch (NewThreadClass.StatusDownloadAndPlay)
            {
                case 200:
                    SongModel songToPlay = InsertSongToDatabase(DownloadedFilePath);
                    UpNextListModel.AddSongToUpNextListAndPlay(songToPlay);
                    break;
                case 500:
                    break;
                case 400:
                    IsLoading = false;
                    ToggleLoadingModeRequest();
                    MessageBoxResult messageBoxResult = MessageBox.Show($@"Bài hát hiện tại chưa nghe được trên ứng dụng, mở bài hát trên trình duyệt web", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Process.Start(OnlineLinkMusic);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    break;
                default:
                    break;
            }

            IsLoading = false;
            ToggleLoadingModeRequest();
        }
        #endregion

        #region download song to app

        DispatcherTimer WaitForDownloadSongToAppTimer;
        string DownloadedSongFilePath;
        private void DownloadSongToApp(string LinkDownload, string DestinationFilePath)
        {

            IsLoading = true;
            ToggleLoadingModeRequest();

            DownloadedSongFilePath = DestinationFilePath;

            NewThreadClass.StatusDownloadAndPlay = 0;
            Thread ThreadDownloadSong = new Thread(NewThreadClass.DownloadSong);
            ThreadDownloadSong.Start(new DownloadRequestInfo(LinkDownload, DestinationFilePath));

            WaitForDownloadSongToAppTimer = new DispatcherTimer();
            WaitForDownloadSongToAppTimer.Tick += WaitForDownloadSongToAppTimer_Tick;
            WaitForDownloadSongToAppTimer.Start();


        }
        private void WaitForDownloadSongToAppTimer_Tick(object sender, EventArgs e)
        {
            if (NewThreadClass.StatusDownloadAndPlay == 0) return;
            //download xong
            WaitForDownloadSongToAppTimer.Stop();

            switch (NewThreadClass.StatusDownloadAndPlay)
            {
                case 200:
                    InsertSongToDatabase(DownloadedSongFilePath);
                    IsLoading = false;
                    ToggleLoadingModeRequest();
                    MessageBox.Show("Tải bài hát xuống thành công");
                    break;
                case 500:
                    MessageBox.Show("Tải xuống thất bại");
                    break;
                case 400:
                    IsLoading = false;
                    ToggleLoadingModeRequest();
                    MessageBox.Show("Bài hát không được tải vì lý do bản quyền");
                    break;
                default:
                    break;
            }

            IsLoading = false;
            ToggleLoadingModeRequest();
        }
        #endregion
        private static SongModel InsertSongToDatabase(string filePath)
        {
            //dua bai hat vao chuong trình
            try
            {
                SongModel newSong = new SongModel();

                var tfile = TagLib.File.Create(filePath);

                newSong.SongName = Path.GetFileName(filePath);

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

                tfile.Save();
                newSong.SongLocation = filePath.Replace($@"{GeneralDataManagement.UserMusicFolderLocation}\", "");
                SongDataAccess.InsertSongToDatabase(newSong);

                return newSong;
            }
            catch (Exception)
            {
                return null;
            }
        }


        #region search song

        DispatcherTimer WaitSearching_Timer;
        private void HeaderBar_SearchRequest(string searchContent)
        {
            IsLoading = true;
            ToggleLoadingModeRequest();

            Thread threadCrawlData = new Thread(NewThreadClass.CrawlData);
            NewThreadClass.Done = false;
            threadCrawlData.Start(searchContent);

            WaitSearching_Timer = new DispatcherTimer();
            WaitSearching_Timer.Tick += WaitSearching_Timer_Tick;
            WaitSearching_Timer.Start();

        }
        private void WaitSearching_Timer_Tick(object sender, EventArgs e)
        {
            if (NewThreadClass.Done == true)
            {
                WaitSearching_Timer.Stop();
                ListSong = NewThreadClass.ListSong;
                OnPropertyChanged("ListSong");
                IsLoading = false;
                ToggleLoadingModeRequest();
            }
        }
        #endregion


    }
}
