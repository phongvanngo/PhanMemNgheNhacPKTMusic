using IndeeMusee.DataManager.DataProvider;
using IndeeMusee.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace IndeeMusee.DataManager
{

    public static class UpNextListData
    {
        public static List<SongModel> GetUpNextList()
        {
            List<SongModel> song_list = new List<SongModel>();

            song_list = SongDataAccess.GetAllUserSongList();

            //String Folder = @"C:\Users\19520\Music\NovapoMusee";
            //String FileType = "*.mp3";
            //DirectoryInfo dinfo = new DirectoryInfo(Folder);
            //FileInfo[] Files = dinfo.GetFiles(FileType);
            //foreach (FileInfo file in Files)
            //{
            //    //ListAudio.Items.Add(file.Name);
            //    song_list.Add(new SongModel(file.FullName,file.Name,file.Name));
            //}
            Console.WriteLine();

            return song_list;
        }

    }
}
