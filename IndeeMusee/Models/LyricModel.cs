using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndeeMusee.Models
{
    public class LyricModel
    {

        public LyricModel(int timeStart, int timeEnd, string content)
        {
            TimeStart = timeStart;
            TimeEnd = timeEnd;
            Content = content;
        }

        public string Content { get; set; }
        public int TimeStart { get; set; }
        public int TimeEnd { get; set; }
    }



    public class SongLyricsModel
    {
        public static int TransferTimeSpan(string value)
        {
            double a;
            try
            {
                a = TimeSpan.ParseExact(value, @"mm\:ss\.ff", null).TotalMilliseconds;
            }
            catch (Exception)
            {
                a = TimeSpan.ParseExact(value, @"mm\:ss\:ff", null).TotalMilliseconds;
            }

            return (int)a;
        }

        public static List<LyricModel> GetLyricsFromFile(string path)
        {
            //try
            //{
            if (File.Exists(path) == false) return null;
            List<LyricModel> lyrics = new List<LyricModel>();

            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs);
            string raw_data = sr.ReadToEnd();
            string[] lyric_data = raw_data.Split('\n');

            int index = 0;
            while (index < lyric_data.Length)
            {
                string data = lyric_data[index];
                int i = data.IndexOf("[");
                int j = data.IndexOf("]");

                if (i < 0 || j < 0)
                {
                    index++;
                    continue;
                }

                string timeStart = data.Substring(i + 1, j - (i + 1));
                string content = "";

                //lấy lời bài hát
                if (j + 1 < data.Length)
                {
                    content = data.Substring(j + 1);
                    content = content.Trim();
                }

                index++;
                if (content == ""|| index == lyric_data.Length)
                {
                    continue;
                }

                data = lyric_data[index];
                i = data.IndexOf("[");
                j = data.IndexOf("]");
                if (i == -1 || j == -1) continue;
                string timeEnd = data.Substring(i + 1, j - (i + 1));

                LyricModel newLyric = new LyricModel(TransferTimeSpan(timeStart), TransferTimeSpan(timeEnd), content);
                lyrics.Add(newLyric);
            }
            return lyrics;
            //}
            //catch (Exception)
            //{
            //    //return null;
            //    throw;
            //}


        }
        public SongLyricsModel()
        {

        }
    }

}
