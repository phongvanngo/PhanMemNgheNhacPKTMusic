using Dapper;
using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndeeMusee.DataManager.DataProvider
{
    class SongDataAccess
    {
        public static void CreateRequiredTable()
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                string sql = @"
                                CREATE TABLE if not exists Music(
                                    SongKey TEXT PRIMARY KEY NOT NULL,
                                    SongName TEXT,
                                    SongLocation TEXT,
                                    Title TEXT,
                                    ContributingArtists  TEXT,
                                    Album TEXT,
                                    Genre TEXT,
                                    Length TEXT,
                                    LyricPath TEXT,
                                    IsFavorite NUMERIC

                                 );       

                                CREATE TABLE if not exists Playlist(
                                    PlaylistKey TEXT PRIMARY KEY NOT NULL,
                                    PlaylistName TEXT,
                                    Description TEXT,
                                    ImagePath TEXT,
                                    Amount NUMBER
                                 );               

                                CREATE TABLE if not exists SongInPlaylist(
                                    SongKey TEXT,
                                    PlaylistKey TEXT,
                                    Description TEXT,
                                    FOREIGN KEY(SongKey) REFERENCES Music(SongKey),
                                    FOREIGN KEY(PlaylistKey) REFERENCES Playlist(PlaylistKey),
                                    PRIMARY KEY(SongKey,PlaylistKey)
                                 );

                                insert into Playlist(PlaylistKey,PlaylistName,Description,ImagePath) values('FAVORITE_PLAYLIST','Favorite Song','','FavoriteSongImage.jpg')
                            ";
                cnn.Execute(sql);
            }
        }
        public static void InsertSongToDatabase(SongModel song)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                cnn.Execute("insert into Music(SongKey,SongName,SongLocation,Title,ContributingArtists,Album,Genre,Length,LyricPath) " +
                            "values(@SongKey,@SongName,@SongLocation,@Title,@ContributingArtists,@Album,@Genre,@Length,@LyricPath)"
                    , song);
            }
        }

        public static void UpdateSong(SongModel song)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {

                cnn.Execute("UPDATE Music " +
                            "SET Title = @Title, Genre = @Genre, ContributingArtists = @ContributingArtists, Album = @Album,  LyricPath = @LyricPath " +
                            "WHERE SongKey = @SongKey",
                            song);
            }
        }        
        
        public static void UpdatePlaylist(PlaylistModel playlist)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {

                cnn.Execute("UPDATE Playlist " +
                            "SET PlaylistName = @PlaylistName, ImagePath = @ImagePath, Description = @Description " +
                            "WHERE PlaylistKey = @PlaylistKey",
                            playlist);
            }
        }

        public static void DeleteSongFromDatabase(string musicKey)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM MUSIC WHERE SongKey = '{musicKey}'"); ;
            }
        }

        public static SongModel GetSongFromDatabase(string SongKey)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                var output = cnn.Query<SongModel>($@"select * from Music where SongKey = '{SongKey}' ", new DynamicParameters());
                List<SongModel> list = output.ToList();
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            }
        }        
        public static SongModel GetSong_SongLocation(string SongLocation)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                var output = cnn.Query<SongModel>($@"select * from Music where SongLocation = '{SongLocation}' ", new DynamicParameters());
                List<SongModel> list = output.ToList();
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            }
        }

        public static bool CheckSongExistInDatabase(string SongName)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                var output = cnn.Query<SongModel>($@"select * from Music where SongName = '{SongName}' ", new DynamicParameters());
                List<SongModel> list = output.ToList();
                if (list.Count > 0)
                    return true;
                else
                    return false;
            }
        }        
        
        public static bool CheckSongExistInPlaylist(string SongKey,string PlaylistKey)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                var output = cnn.Query<SongModel>($@"select SongKey from SongInPlaylist where SongKey = '{SongKey}' and PlaylistKey = '{PlaylistKey}'", new DynamicParameters());
                List<SongModel> list = output.ToList();
                if (list.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public static List<SongModel> GetAllUserSongList()
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                var output = cnn.Query<SongModel>("select * from Music", new DynamicParameters());

                return output.ToList();
            }
        }
        public static List<PlaylistModel> GetAllPlaylist()
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                var output = cnn.Query<PlaylistModel>("select * from Playlist", new DynamicParameters());
                return output.ToList();
            }
        }

        public static PlaylistModel GetPlaylist(string playlistKey)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                var output = cnn.Query<PlaylistModel>($"select * from Playlist where PlaylistKey = '{playlistKey}'", new DynamicParameters());
                List<PlaylistModel> list = output.ToList();
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            }
        }

        public static void InsertNewPlaylistToDatabase(PlaylistModel playlist)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                cnn.Execute("insert into Playlist(PlaylistKey,PlaylistName,Description,ImagePath) " +
                            "values(@PlaylistKey,@PlaylistName, @Description,@ImagePath)"
                    , playlist);
            }
        }

        public static void DropPlaylistFromDatabase(string playlistKey)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM PLAYLIST WHERE PlaylistKey = '{playlistKey}'"); ;
                cnn.Execute($"DELETE FROM SongInPlaylist WHERE PlaylistKey = '{playlistKey}'");
            }
        }

        public static void InsertSongToPlaylist(string songKey, string playlistKey)
        {
            string sql = $@"insert into SongInPlaylist (SongKey, PlaylistKey) values ('{songKey}', '{playlistKey}')";
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
                {
                    cnn.Execute(sql);
                }
            }
            catch (Exception e)
            {
                
                return;
            }
        }
        public static void InsertSongToFavorite(string songKey)
        {
            string sql = $@"insert into SongInPlaylist (SongKey, PlaylistKey) values ('{songKey}', 'FAVORITE_PLAYLIST')";

            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                cnn.Execute(sql);
                cnn.Execute("UPDATE Music " +
                            "SET IsFavorite = 1 " +
                            $"WHERE SongKey = '{songKey}'");
            }
        }        
        public static void RemoveSongFromFavorite(string songKey)
        {
            string sql = $@"Delete from SongInPlaylist where SongKey = '{songKey}' and PlaylistKey = 'FAVORITE_PLAYLIST'";

            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                cnn.Execute(sql);
                cnn.Execute("UPDATE Music " +
                            "SET IsFavorite = 0 " +
                            $"WHERE SongKey = '{songKey}'");
            }
        }

        public static List<SongModel> GetAllSongInPlaylist(string playlistKey)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {

                var output = cnn.Query<SongModel>(
                    $@"
                        select * from Music,SongInPlaylist where Music.SongKey = SongInPlaylist.SongKey and SongInPlaylist.PlaylistKey = '{playlistKey}'
                      "
                    , new DynamicParameters());
                List<SongModel> list = output.ToList();
                return list;
            }
        }
        public static List<SongModel> GetAllSongListWithPlaylistInfo(string playlistKey)
        {
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {

                var output = cnn.Query<SongModel>(
                    $@"
                        SELECT * 
                        FROM Music 
                        LEFT JOIN
                        (
                        select * from SongInPlaylist
                        where PlaylistKey = '{playlistKey}'
                        ) as AllSongInPL
                        ON Music.SongKey = AllSongInPL.SongKey
                        ORDER by PlaylistKey desc   
                      "
                    , new DynamicParameters());
                List<SongModel> list = output.ToList();
                return list;
            }
        }

        public static void DeleteSongFromPlaylist(string songKey, string playlistKey)
        {
            string sql = $@"Delete from SongInPlaylist where SongKey = '{songKey}' and PlaylistKey = '{playlistKey}'";
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {
                cnn.Execute(sql);
            }
        }

        public static int GetAmountOfSongInPlaylist(string playlistKey)
        {
            string sql = $@"select count(*) from SongInPlaylist where PlaylistKey = '{playlistKey}'";
            using (IDbConnection cnn = new SQLiteConnection(BaseDataProvider.LoadConnectionString()))
            {

                var output = cnn.ExecuteScalar(sql);
                return Convert.ToInt32(output);
            }
        }

    }
}
