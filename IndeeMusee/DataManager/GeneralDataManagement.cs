using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace IndeeMusee.DataManager
{
    //quản lý dữ liệu tổng quát, chung nhất
    public static class GeneralDataManagement
    {
        public static string DatabaseFileName = ".IndeeMuseeDatabase.db";

        public static string DatabaseFolderName = ".IndeeMuseeDatabase";

        public static string ProjectDirectoryPath = "";
        public static string UserMusicFolderLocation = "";
        public static string DatabaseFilePath = "";
        public static string DatabaseFolderPath = "";
        public static string ImageFolderPath = "";
        public static string LyricFolderPath = "";

        public static List<CSong> UserSongList = new List<CSong>();

        //link file MusicLocation.txt
        public static string FileLinkMusicLocationPath = "";

        //link file recentSong
        public static string FileRecentSongPath = "";

        #region Công việc khi khởi động chương trình
        public static void OnStartup()
        {
            ProjectDirectoryPath = System.Environment.CurrentDirectory;
            ProjectDirectoryPath = Directory.GetParent(ProjectDirectoryPath).Parent.FullName;
            FileLinkMusicLocationPath = ProjectDirectoryPath + @"\DataManager\MusicLocation.txt";
            FileRecentSongPath = ProjectDirectoryPath + @"\DataManager\RecentSong.txt";

            //lấy link thư mục chứa nhạc trong thư mục MusicLocation.txt
            UserMusicFolderLocation = System.IO.File.ReadAllText(FileLinkMusicLocationPath);
        }
        #endregion

        #region Kiểm tra link thư mục trước đó còn tồn tại không, nếu chưa sẽ chọn lại folder
        public static void CheckUserMusicFolderLocation()
        {
            //nếu tồn tại thư mục của người dùng
            if (Directory.Exists(UserMusicFolderLocation)) return;

            //chưa có thư mục hoặc thư mục không hợp lệ --> chọn thư mục khác
            ChooseMusicFolderForm form_choose_music_folder = new ChooseMusicFolderForm();
            if (form_choose_music_folder.ShowDialog() == true)
            {
                //ghi đường dẫn thư mục vào file MusicLocation.txt;
                System.IO.File.WriteAllText(FileLinkMusicLocationPath, UserMusicFolderLocation);
            }
            else
            //nếu không chọn thư mục mà tắt form đi
            {
                Environment.Exit(0);
            }
        }
        #endregion

        #region Kiểm tra database có trong thư mục người dùng không
        public static bool IsHavingDatabaseInUserMusicFolder()
        {
            //Nếu thư mục đó không tồn tại
            if (!Directory.Exists(UserMusicFolderLocation)) return false;
            string[] subfolders = Directory.GetDirectories(UserMusicFolderLocation);
            foreach (var folder in subfolders)
            {
                var dir = new DirectoryInfo(folder);
                if (dir.Name == DatabaseFolderName)
                {
                    FileInfo[] files = dir.GetFiles();
                    foreach (var file in files)
                    {
                        if (file.Name == DatabaseFileName)
                        {
                            DatabaseFolderPath = $@"{UserMusicFolderLocation}\{DatabaseFolderName}";
                            DatabaseFilePath = $@"{UserMusicFolderLocation}\{DatabaseFolderName}\{DatabaseFileName}";
                            LyricFolderPath = $@"{UserMusicFolderLocation}\{DatabaseFolderName}\Lyric";
                            ImageFolderPath = $@"{UserMusicFolderLocation}\{DatabaseFolderName}\Image";
                            //MessageBox.Show("đã có file database");
                            return true;
                        }

                    }
                }
            }
            return false;
        }
        #endregion

        #region Tạo database trong thư mục người dùng


        public static void InitializeDatabase()
        {
            if (!Directory.Exists(UserMusicFolderLocation)) return;

            //MessageBox.Show("Tạo Database");

            DatabaseFolderPath = $@"{UserMusicFolderLocation}\{DatabaseFolderName}";
            DatabaseFilePath = $@"{UserMusicFolderLocation}\{DatabaseFolderName}\{DatabaseFileName}";
            ImageFolderPath = $@"{UserMusicFolderLocation}\{DatabaseFolderName}\Image";
            LyricFolderPath = $@"{UserMusicFolderLocation}\{DatabaseFolderName}\Lyric";

            //tạo thư mục hình ảnh, lyric trong thư mục nhạc của người dùng
            Directory.CreateDirectory(LyricFolderPath);
            Directory.CreateDirectory(ImageFolderPath);

            //đưa hình ảnh favorite song vào folder image
            string sourceFile = $@"{ProjectDirectoryPath}\Assets\FavoriteSongImage.jpg";
            string destFile = $@"{GeneralDataManagement.ImageFolderPath}\{System.IO.Path.GetFileName(sourceFile)}";
            System.IO.File.Copy(sourceFile, destFile, true);



            //tạo thư mục database trong thư mục âm nhạc của người dùng
            Directory.CreateDirectory(DatabaseFolderPath);

            DirectoryInfo di = Directory.CreateDirectory(DatabaseFolderPath);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            //tạo database
            SQLiteConnection.CreateFile($@"{DatabaseFilePath}");
            SongDataAccess.CreateRequiredTable();
        }
        #endregion

        #region Quét thư mục và đưa dữ liệu bài hát vào database

        public static void AddNewSong(string song_path)
        {
            SongModel newSong = new SongModel();
            var tfile = TagLib.File.Create(song_path);

            newSong.SongName = Path.GetFileName(song_path);
            newSong.SongLocation = song_path;

            if (tfile.Tag.Title == null) tfile.Tag.Title = newSong.SongName;
            if (tfile.Tag.Title.Trim() == "") tfile.Tag.Title = newSong.SongName;

            newSong.Title = tfile.Tag.Title;

            newSong.Album = tfile.Tag.Album;

            try
            {
                newSong.ContributingArtists = tfile.Tag.Artists[0];
            }
            catch (Exception)
            {
                newSong.ContributingArtists = "unknow";
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
                newSong.SongLocation = newSong.SongLocation.Replace($@"{UserMusicFolderLocation}\", "");
                SongDataAccess.InsertSongToDatabase(newSong);
            }
            catch (Exception)
            {

            }
        }

        public static void FindAndImportSongToDatabase()
        {
            Cursor.Current = Cursors.WaitCursor;
            List<string> song_path_list = new List<string>();
            Stack<string> SubFolderStack = new Stack<string>();

            SubFolderStack.Push(UserMusicFolderLocation);
            while (SubFolderStack.Count > 0)
            {
                string folder_path = SubFolderStack.Pop();
                foreach (string song_path in Directory.GetFiles(folder_path, "*.mp3"))
                {
                    try
                    {
                        AddNewSong(song_path);
                    }
                    catch (Exception)
                    {

                    }
                }
                foreach (string subfolder in Directory.GetDirectories(folder_path))
                {
                    SubFolderStack.Push(subfolder);
                }
            }
            Cursor.Current = Cursors.Default;

        }
        #endregion

        #region Kiểm tra nếu file nhạc nào ko còn thì sẽ loại khỏi db

        public static void RemoveInvalidSongFromDatabase()
        {
            List<SongModel> ListSong = SongDataAccess.GetAllUserSongList();
            foreach (SongModel song in ListSong)
            {
                if (File.Exists($@"{GeneralDataManagement.UserMusicFolderLocation}\{song.SongLocation}") == false)
                {
                    SongDataAccess.DeleteSongFromDatabase(song.SongKey);
                }
            }

        }


        #endregion


        //kiểm tra trong thư mục người dùng đã có database quản lý chưa, nếu chưa có thì sẽ tạo
        public static void CheckDatabaseInFolder()
        {

        }


        public static string GetRecentSong()
        {
            return System.IO.File.ReadAllText(FileRecentSongPath);
        }
        public static void InsertSongToRecentPlay(string songKey)
        {
            System.IO.File.WriteAllText(FileRecentSongPath, songKey);
        }
    }
}
